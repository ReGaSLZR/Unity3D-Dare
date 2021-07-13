namespace ReGaSLZR.Dare.Model
{

    using Enum;

    using UnityEngine;
    using Zenject;

    public interface IGeneralSettingsGetter
    {
        public LightingSetting StartingLightingSetting();
        public int StartingRoundNumber();
    }

    [CreateAssetMenu(fileName = "New GeneralSettings", menuName = "Dare/General Settings")]
    public class GeneralSettingsModel : ScriptableObjectInstaller,
        IGeneralSettingsGetter
    {

        [SerializeField]
        private LightingSetting startingLightingSetting;

        [SerializeField]
        private int startingRoundNumber = 1;

        public override void InstallBindings()
        {
            Container.Bind<IGeneralSettingsGetter>().FromInstance(this);
        }

        #region Getter

        public LightingSetting StartingLightingSetting()
        {
            return startingLightingSetting;
        }
        public int StartingRoundNumber()
        {
            return startingRoundNumber;
        }

        #endregion

    }

}