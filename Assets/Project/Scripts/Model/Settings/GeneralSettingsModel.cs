namespace ReGaSLZR.Dare.Model
{

    using Enum;

    using UnityEngine;
    using Zenject;

    public interface IGeneralSettingsGetter
    {
        public LightingSetting StartingLightingSetting();
    }

    [CreateAssetMenu(fileName = "New GeneralSettings", menuName = "Dare/General Settings")]
    public class GeneralSettingsModel : ScriptableObjectInstaller,
        IGeneralSettingsGetter
    {

        [SerializeField]
        private LightingSetting startingLightingSetting;

        public override void InstallBindings()
        {
            Container.Bind<IGeneralSettingsGetter>().FromInstance(this);
        }

        #region Getter

        public LightingSetting StartingLightingSetting()
        {
            return startingLightingSetting;
        }

        #endregion

    }

}