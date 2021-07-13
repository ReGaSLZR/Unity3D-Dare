namespace ReGaSLZR.Dare.Model
{

    using Enum;

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public interface ILightingSetter
    {
        public void SetSetting(LightingSetting setting);
        public void ToggleSetting();
    }

    public interface ILightingGetter
    {
        public IReadOnlyReactiveProperty<LightingSetting> GetSetting();
    }

    public class LightingModel : MonoInstaller,
        ILightingSetter, ILightingGetter
    {

        [System.Serializable]
        public class Setting
        {

            [SerializeField]
            private Light[] lights;

            [SerializeField]
            [Required]
            private Material skybox;

            public void SetEnabled(bool isEnabled)
            {
                foreach (var light in lights)
                {
                    if (light != null)
                    {
                        light.enabled = isEnabled;
                        light.gameObject.SetActive(isEnabled);
                    }
                }

                if (isEnabled)
                {
                    RenderSettings.skybox = skybox;
                }
                
            }

        }

        #region Inspector Variables

        [SerializeField]
        private Setting settingDay;

        [SerializeField]
        private Setting settingNight;

        #endregion

        private ReactiveProperty<LightingSetting> lightingSetting
            = new ReactiveProperty<LightingSetting>();

        public override void InstallBindings()
        {
            Container.Bind<ILightingGetter>().FromInstance(this);
            Container.Bind<ILightingSetter>().FromInstance(this);
        }

        #region Setter

        public void SetSetting(LightingSetting setting)
        {
            var isDay = (setting == LightingSetting.Day);
            lightingSetting.SetValueAndForceNotify(
                isDay ? LightingSetting.Day : LightingSetting.Night);

            settingDay.SetEnabled(isDay);
            settingNight.SetEnabled(!isDay);
        }

        [Button]
        public void ToggleSetting()
        {
            SetSetting((lightingSetting.Value == LightingSetting.Day)
                ? LightingSetting.Night : LightingSetting.Day);
        }

        #endregion

        #region Getter

        public IReadOnlyReactiveProperty<LightingSetting> GetSetting()
        {
            return lightingSetting;
        }

        #endregion

    }

}