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
        public int GetCriticalStamina();
        public IReadOnlyReactiveProperty<int> Health();
        public IReadOnlyReactiveProperty<int> Stamina();
        
        public IReadOnlyReactiveProperty<bool> IsRunning();
        public IReadOnlyReactiveProperty<bool> IsOnGround();
        public IReadOnlyReactiveProperty<bool> IsCrouching();
        public IReadOnlyReactiveProperty<bool> OnTakeDamage();

    }

    public interface IPlayerStatusSetter
    {

        public void SetIsRunning(bool isRunning);
        public void SetIsOnGround(bool isOnGround);
        public void ToggleIsCrouching();
        public void Damage(int damage);
        public void CostStamina(int cost);

    }

    #endregion

    public class PlayerStatus : MonoBehaviour, 
        IPlayerStatusGetter, IPlayerStatusSetter, 
        IPlayerSkillGetter, IPlayerSkillSetter
    {

        #region Inspector Variables

        [SerializeField]
        [Range(1, MAX_STAMINA)]
        private int staminaCostSummonBait = 85;

        [SerializeField]
        [Range(0.001f, 0.25f)]
        private float staminaRunTick = 0.25f;

        [SerializeField]
        [Range(0.001f, 0.25f)]
        private float staminaShieldTick = 0.005f;

        [SerializeField]
        [Range(0.01f, 0.25f)]
        private float staminaRefillTick = 0.25f;

        [SerializeField]
        private int debugDamage = 25;

        #endregion

        #region Private Variables

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private const int MAX_HEALTH = 100;
        private const int MAX_STAMINA = 100;

        //Base Stats
        private ReactiveProperty<int> health = new ReactiveProperty<int>(MAX_HEALTH);
        private ReactiveProperty<int> stamina = new ReactiveProperty<int>(MAX_STAMINA);

        //Derived Base Stats
        private ReactiveProperty<bool> hasTakenDamage = new ReactiveProperty<bool>(false);

        //Locomotion Stats
        private ReactiveProperty<bool> isOnGround = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isRunning = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isCrouching = new ReactiveProperty<bool>(false);

        //Skills
        private ReactiveProperty<bool> isShielding = new ReactiveProperty<bool>(false);

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            stamina.Select(_ => stamina.Value)
                .Where(val => val <= 0)
                .Subscribe(_ =>
                {
                    isRunning.SetValueAndForceNotify(false);
                    isShielding.SetValueAndForceNotify(false);
                })
               .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaRunTick))
                .Where(_ => isRunning.Value && (stamina.Value > 0))
                .Subscribe(_ => {
                    stamina.Value--;
                })
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaShieldTick))
                .Where(_ => isShielding.Value && (stamina.Value > 0))
                .Subscribe(_ => {
                    stamina.Value--;
                })
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaRefillTick))
                .Where(_ => !isRunning.Value && (stamina.Value < MAX_STAMINA))
                .Subscribe(_ => {
                    stamina.Value++;
                })
                .AddTo(disposables);
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

        [Button]
        private void TestDamage()
        {
            Damage(debugDamage);
        }

        [Button]
        private void ResetHealthAndStamina()
        {
            health.Value = MAX_HEALTH;
            stamina.Value = MAX_STAMINA;
        }

        #endregion

        #region Skill Setter Interface Implementation

        public void ToggleShielding()
        {
            isShielding.Value = !isShielding.Value;
        }

        #endregion

        #region Status Setter Interface Implementation

        public void SetIsOnGround(bool isOnGround)
        {
            this.isOnGround.Value = isOnGround;
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

            health.Value = Mathf.Clamp(
                health.Value - damage, 0, MAX_HEALTH);
            hasTakenDamage.SetValueAndForceNotify(
                (health.Value < 100) && (health.Value > 0));
            isCrouching.SetValueAndForceNotify(false);
        }

        public void CostStamina(int cost)
        {
            stamina.Value = Mathf.Clamp(
                stamina.Value - cost, 0, MAX_STAMINA);
        }

        #endregion

        #region Skill Getter Interface Implementation

        public IReadOnlyReactiveProperty<bool> IsShielding()
        {
            return isShielding;
        }

        public int GetStaminaCostSummonBait()
        {
            return staminaCostSummonBait;
        }

        #endregion

        #region Status Getter Interface Implementation

        public IReadOnlyReactiveProperty<bool> IsOnGround()
        {
            return isOnGround;
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
            return health;
        }

        public IReadOnlyReactiveProperty<int> Stamina()
        {
            return stamina;
        }

        public int GetMaxHealth()
        {
            return MAX_HEALTH;
        }

        public int GetMaxStamina()
        {
            return MAX_STAMINA;
        }

        public int GetCriticalStamina()
        {
            return MAX_STAMINA / 10;
        }

        #endregion

    }

}