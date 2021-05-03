namespace ReGaSLZR.Dare.Model.Status
{

    using UniRx;
    using UnityEngine;

    #region Interfaces

    public interface PlayerStatusGetter
    {

        public IReadOnlyReactiveProperty<bool> IsRunning();
        public IReadOnlyReactiveProperty<bool> IsOnGround();
        public IReadOnlyReactiveProperty<bool> IsCrouching();

    }

    public interface PlayerStatusSetter
    {

        public void SetIsRunning(bool isRunning);
        public void SetIsOnGround(bool isOnGround);
        public void ToggleIsCrouching();

    }

    #endregion

    public class PlayerStatus : MonoBehaviour, 
        PlayerStatusGetter, PlayerStatusSetter
    {

        #region Variables

        private ReactiveProperty<bool> isOnGround = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isRunning = new ReactiveProperty<bool>(false);
        private ReactiveProperty<bool> isCrouching = new ReactiveProperty<bool>(false);

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

        #endregion

    }

}