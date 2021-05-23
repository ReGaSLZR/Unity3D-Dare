namespace ReGaSLZR.Dare.Model.Player
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    #region Interfaces

    public interface IPlayerSkillGetter
    {
        public IReadOnlyReactiveProperty<bool> IsShielding();
        public int GetStaminaCostSummonBait();
    }

    public interface IPlayerSkillSetter
    {
        public void ToggleShielding();
    }

    public interface IPlayerStatusGetter
    {

        public int GetMaxHealth();
        public int GetMaxStamina();
        public int GetMaxNoise();
        public int GetCriticalStamina();
        public NoiseActions GetNoiseActions();
        public IReadOnlyReactiveProperty<int> Health();
        public IReadOnlyReactiveProperty<int> Stamina();
        public IReadOnlyReactiveProperty<int> Noise();
        
        /// <summary>
        /// Caters to both walking and running actions.
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsMoving(); 
        public IReadOnlyReactiveProperty<bool> IsRunning();
        public IReadOnlyReactiveProperty<bool> IsOnGround();
        public IReadOnlyReactiveProperty<bool> IsCrouching();
        public IReadOnlyReactiveProperty<bool> OnTakeDamage();

    }

    public interface IPlayerStatusSetter
    {

        public void SetIsMoving(bool isMoving);
        public void SetIsRunning(bool isRunning);
        public void SetIsOnGround(bool isOnGround);
        public void ToggleIsCrouching();
        public void Damage(int damage);
        public void CostStamina(int cost);
        public void SetNoise(bool isAdd, int value, bool isForced = false);

    }

    #endregion

    public class PlayerStatus : MonoBehaviour,
        IPlayerStatusGetter, IPlayerStatusSetter,
        IPlayerSkillGetter, IPlayerSkillSetter
    {

        #region Inspector Variables

        [SerializeField]
        private int debugDamage = 25;

        [SerializeField]
        [Expandable]
        private StaminaCosts staminaCosts;

        [SerializeField]
        [Expandable]
        private NoiseActions noiseActions;

        #endregion

        #region Private Variables

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private const int MAX_HEALTH = 100;
        private const int MAX_STAMINA = 100;
        private const int MAX_NOISE = 100;

        //Base Stats
        private ReactiveProperty<int> statHealth = new ReactiveProperty<int>(MAX_HEALTH);
        private ReactiveProperty<int> statStamina = new ReactiveProperty<int>(MAX_STAMINA);
        private ReactiveProperty<int> statNoise = new ReactiveProperty<int>(0);

        //Derived Base Stats
        private ReactiveProperty<bool> hasTakenDamage = new ReactiveProperty<bool>(false);

        //Locomotion Stats
        private ReactiveProperty<bool> isOnGround = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isMoving = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isRunning = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isCrouching = new ReactiveProperty<bool>(false);

        //Skills
        private ReactiveProperty<bool> isShielding = new ReactiveProperty<bool>(false);

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            RegisterDisposables();
        }

        private void OnDisable()
        {
            disposables.Clear();
        }

        private void Start()
        {
            isShielding.SetValueAndForceNotify(false);
        }

        #endregion

        #region Class Implementation

        private void RegisterDisposables()
        {
            statStamina.Select(_ => statStamina.Value)
                    .Where(val => val <= 0)
                    .Subscribe(_ =>
                    {
                        isRunning.SetValueAndForceNotify(false);
                        isShielding.SetValueAndForceNotify(false);
                        isCrouching.SetValueAndForceNotify(false);
                    })
                   .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.runTick))
                .Where(_ => isRunning.Value && (statStamina.Value > 0))
                .Subscribe(_ =>
                {
                    statStamina.Value--;
                })
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.skillShieldTick))
                .Where(_ => isShielding.Value && (statStamina.Value > 0))
                .Subscribe(_ =>
                {
                    statStamina.Value--;
                })
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.refillTick))
                .Where(_ => !isRunning.Value && (statStamina.Value < MAX_STAMINA))
                .Subscribe(_ =>
                {
                    statStamina.Value++;
                })
                .AddTo(disposables);
        }

        [Button]
        public void ResetDisposables()
        {
            disposables.Clear();
            RegisterDisposables();

            statStamina.Value = MAX_STAMINA;
            statHealth.Value = MAX_HEALTH;
            statNoise.Value = 0;
        }

        [Button]
        private void TestDamage()
        {
            Damage(debugDamage);
        }

        [Button]
        private void ResetHealthAndStamina()
        {
            statHealth.Value = MAX_HEALTH;
            statStamina.Value = MAX_STAMINA;
        }

        #endregion

        #region Skill Setter Interface Implementation

        public void ToggleShielding()
        {
            isShielding.Value = !isShielding.Value;
            CostStamina(0);
        }

        #endregion

        #region Status Setter Interface Implementation

        public void SetIsOnGround(bool isOnGround)
        {
            this.isOnGround.Value = isOnGround;
        }

        public void SetIsMoving(bool isMoving)
        {
            this.isMoving.Value = isMoving;
        }

        public void SetIsRunning(bool isRunning)
        {
            this.isRunning.Value = isRunning;
        }

        public void ToggleIsCrouching()
        {
            isCrouching.Value = !isCrouching.Value;
        }

        public void Damage(int damage)
        {
            if (isShielding.Value)
            {
                //TODO apply damage deflection FX
                return;
            }

            statHealth.Value = Mathf.Clamp(
                statHealth.Value - damage, 0, MAX_HEALTH);
            hasTakenDamage.SetValueAndForceNotify(
                (statHealth.Value < 100) && (statHealth.Value > 0));
            isCrouching.SetValueAndForceNotify(false);
        }

        public void CostStamina(int cost)
        {
            statStamina.Value = Mathf.Clamp(
                statStamina.Value - cost, 0, MAX_STAMINA);

            isCrouching.Value = false;
        }

        public void SetNoise(bool isAdd, int value, bool isForced = false)
        {
            int val = isForced ? value 
                : (statNoise.Value + (isAdd ? value : -value));
            statNoise.Value = Mathf.Clamp(val, 0, MAX_NOISE);
        }

        #endregion

        #region Skill Getter Interface Implementation

        public IReadOnlyReactiveProperty<bool> IsShielding()
        {
            return isShielding;
        }

        public int GetStaminaCostSummonBait()
        {
            return staminaCosts.skillSummonBait;
        }

        #endregion

        #region Status Getter Interface Implementation

        public IReadOnlyReactiveProperty<bool> IsOnGround()
        {
            return isOnGround;
        }

        public IReadOnlyReactiveProperty<bool> IsMoving()
        {
            return isMoving;
        }

        public IReadOnlyReactiveProperty<bool> IsRunning()
        {
            return isRunning;
        }

        public IReadOnlyReactiveProperty<bool> IsCrouching()
        {
            return isCrouching;
        }

        public IReadOnlyReactiveProperty<bool> OnTakeDamage()
        {
            return hasTakenDamage;
        }

        public IReadOnlyReactiveProperty<int> Health()
        {
            return statHealth;
        }

        public IReadOnlyReactiveProperty<int> Stamina()
        {
            return statStamina;
        }

        public IReadOnlyReactiveProperty<int> Noise()
        {
            return statNoise;
        }

        public int GetMaxHealth()
        {
            return MAX_HEALTH;
        }

        public int GetMaxStamina()
        {
            return MAX_STAMINA;
        }

        public int GetMaxNoise()
        {
            return MAX_NOISE;
        }

        public int GetCriticalStamina()
        {
            return MAX_STAMINA / 10;
        }

        public NoiseActions GetNoiseActions()
        {
            return noiseActions;
        }

        #endregion

    }

}