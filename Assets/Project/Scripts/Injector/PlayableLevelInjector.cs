namespace ReGaSLZR.Dare.Injector
{

    using Model.Player;

    using NaughtyAttributes;
    using UnityEngine;
    using Zenject;

    public class PlayableLevelInjector : MonoInstaller
    {

        #region Inspector Variables

        [SerializeField]
        [Required]
        private PlayerStatus playerStatus;

        #endregion

        public override void InstallBindings()
        {
            Inject();
        }

        #region Class Implementation

        private void Inject()
        {
            Container.Bind<IPlayerStatusGetter>().FromInstance(playerStatus);
            Container.Bind<IPlayerStatusSetter>().FromInstance(playerStatus);
            Container.Bind<IPlayerSkillGetter>().FromInstance(playerStatus);
            Container.Bind<IPlayerSkillSetter>().FromInstance(playerStatus);
        }

        #endregion

    }


}