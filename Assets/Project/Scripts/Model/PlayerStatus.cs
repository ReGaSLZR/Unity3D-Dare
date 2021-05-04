namespace ReGaSLZR.Dare.Model.Status
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    #region Interfaces

    public interface IPlayerStatusGetter
    {

        public IReadOnlyReactiveProperty<bool> IsRunning();
        public IReadOnlyReactiveProperty<bool> IsOnGround();
        public IReadOnlyReactiveProperty<bool> IsCrouching();
        public IReadOnlyReactiveProperty<bool> OnTakeDamage();
        public IReadOnlyReactiveProperty<int> Health();

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
        private int debugDamage = 25;

        #endregion

        #region Private Variables

        private const int HEALTH_MAX = 100;

        private ReactiveProperty<int> health = new ReactiveProperty<int>(HEALTH_MAX);
        private ReactiveProperty<bool> hasTakenDamage = new ReactiveProperty<bool>(false);

        private ReactiveProperty<bool> isOnGround = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isRunning = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isCrouching = new ReactiveProperty<bool>(false);

        #endregion

        #region Class Implementation

        [Button]
        private void TestDamage()
        {
            Damage(debugDamage);
        }

        [Button]
        private void ResetHealth()
        {
            health.Value = HEALTH_MAX;
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
            health.Value =Mathf.Clamp(
                health.Value - damage, 0, HEALTH_MAX);
            hasTakenDamage.SetValueAndForceNotify(
                (health.Value < 100) && (health.Value > 0));
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

        #endregion

    }

}