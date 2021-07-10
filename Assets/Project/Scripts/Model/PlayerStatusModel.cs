namespace ReGaSLZR.Dare.Model.Player
{

    using UniRx;
    using UnityEngine;
    using Zenject;

    #region Interfaces

    public interface IPlayerSkillGetter
    {
        public IReadOnlyReactiveProperty<bool> IsShielding();
    }

    public interface IPlayerSkillSetter
    {
        public void ToggleShielding();
        public void ForceShieldOff();
    }

    public interface IPlayerStatusGetter
    {

        public int GetMaxHealth();
        public int GetMaxStamina();
        public int GetMaxNoise();
        public int GetCriticalStamina();
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
        public void ForceCrouchOff();
        public void Damage(int damage);
        public void CostStamina(int cost);
        public void DecreaseStaminaBy1();
        public void IncreaseStaminaBy1();
        public void SetNoise(bool isAdd, int value, bool isForced = false);
        public void SetToMaxStamina();
        public void SetToMaxHealth();

    }

    #endregion

    public class PlayerStatusModel : MonoInstaller,
        IPlayerStatusGetter, IPlayerStatusSetter,
        IPlayerSkillGetter, IPlayerSkillSetter
    {

        #region Private Variables

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

        public override void InstallBindings()
        {
            Container.Bind<IPlayerStatusGetter>().FromInstance(this);
            Container.Bind<IPlayerStatusSetter>().FromInstance(this);
            Container.Bind<IPlayerSkillGetter>().FromInstance(this);
            Container.Bind<IPlayerSkillSetter>().FromInstance(this);
        }

        #region Skill Setter Interface Implementation

        public void ToggleShielding()
        {
            isShielding.Value = !isShielding.Value;
            CostStamina(0);
        }

        public void ForceShieldOff()
        {
            isShielding.SetValueAndForceNotify(false);
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

        public void ForceCrouchOff()
        {
            isCrouching.SetValueAndForceNotify(false);
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

        public void DecreaseStaminaBy1()
        {
            statStamina.Value--;
        }

        public void IncreaseStaminaBy1()
        {
            statStamina.Value++;
        }

        public void SetNoise(bool isAdd, int value, bool isForced = false)
        {
            int val = isForced ? value 
                : (statNoise.Value + (isAdd ? value : -value));
            statNoise.Value = Mathf.Clamp(val, 0, MAX_NOISE);
        }

        public void SetToMaxStamina()
        {
            statStamina.SetValueAndForceNotify(MAX_STAMINA);
        }

        public void SetToMaxHealth()
        {
            statHealth.SetValueAndForceNotify(MAX_HEALTH);
        }

        #endregion

        #region Skill Getter Interface Implementation

        public IReadOnlyReactiveProperty<bool> IsShielding()
        {
            return isShielding;
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

        #endregion

    }

}