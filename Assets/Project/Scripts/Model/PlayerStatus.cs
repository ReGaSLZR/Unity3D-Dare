namespace ReGaSLZR.Dare.Model.Status
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    #region Interfaces

    public interface IPlayerStatusGetter
    {

        public int GetMaxHealth();
        public int GetMaxStamina();
        public IReadOnlyReactiveProperty<bool> IsRunning();
        public IReadOnlyReactiveProperty<bool> IsOnGround();
        public IReadOnlyReactiveProperty<bool> IsCrouching();
        public IReadOnlyReactiveProperty<bool> OnTakeDamage();
        public IReadOnlyReactiveProperty<int> Health();
        public IReadOnlyReactiveProperty<int> Stamina();
        public int GetCriticalStamina();

    }

    public interface IPlayerStatusSetter
    {

        public void SetIsRunning(bool isRunning);
        public void SetIsOnGround(bool isOnGround);
        public void ToggleIsCrouching();
        public void Damage(int damage);

    }

    #endregion

    public class PlayerStatus : MonoBehaviour, 
        IPlayerStatusGetter, IPlayerStatusSetter
    {

        #region Inspector Variables

        [SerializeField]
        [Range(0.01f, 0.25f)]
        private float staminaRunTick = 0.25f;

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

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            stamina.Select(_ => stamina.Value)
                .Where(val => val <= 0)
                .Subscribe(_ => isRunning.SetValueAndForceNotify(false))
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaRunTick))
                .Where(_ => isRunning.Value && (stamina.Value > 0))
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

        #region Setter Interface Implementation

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
            health.Value = Mathf.Clamp(
                health.Value - damage, 0, MAX_HEALTH);
            hasTakenDamage.SetValueAndForceNotify(
                (health.Value < 100) && (health.Value > 0));
            isCrouching.SetValueAndForceNotify(false);
        }

        #endregion

        #region Getter Interface Implementation

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