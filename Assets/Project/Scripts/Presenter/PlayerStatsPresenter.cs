namespace ReGaSLZR.Dare.Presenter
{

    using Model;

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class PlayerStatsPresenter : MonoBehaviour
    {

        #region Inspector Variables

        [SerializeField]
        private int debugDamage = 25;

        #endregion

        [Inject]
        private readonly IPlayerStatusGetter statGetter;
        [Inject]
        private readonly IPlayerStatusSetter statSetter;
        [Inject]
        private readonly IPlayerSkillGetter skillGetter;
        [Inject]
        private readonly IPlayerSkillSetter skillSetter;

        [Inject]
        private readonly IStaminaCostsGetter staminaCosts;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

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
            skillSetter.ForceShieldOff();
        }

        #endregion

        #region Class Implementation

        private void RegisterDisposables()
        {
            statGetter.Stamina()
                    .Where(val => val <= 0)
                    .Subscribe(_ =>
                    {
                        statSetter.SetIsRunning(false);
                        statSetter.ForceCrouchOff();
                        skillSetter.ForceShieldOff();
                    })
                   .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.RunTick()))
                .Where(_ => statGetter.IsRunning().Value && (statGetter.Stamina().Value > 0))
                .Subscribe(_ => statSetter.DecreaseStaminaBy1())
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.SkillShieldTick()))
                .Where(_ => skillGetter.IsShielding().Value && 
                    (statGetter.Stamina().Value > 0))
                .Subscribe(_ => statSetter.DecreaseStaminaBy1())
                .AddTo(disposables);

            Observable.Interval(System.TimeSpan.FromSeconds(staminaCosts.RefillTick()))
                .Where(_ => !statGetter.IsRunning().Value 
                    && (statGetter.Stamina().Value < statGetter.GetMaxStamina()))
                .Subscribe(_ => statSetter.IncreaseStaminaBy1())
                .AddTo(disposables);
        }

        [Button]
        private void ResetDisposables()
        {
            disposables.Clear();
            RegisterDisposables();

            ResetHealthAndStamina();
            statSetter.SetNoise(false, 0, true);
        }

        [Button]
        private void TestDamage()
        {
            statSetter.Damage(debugDamage);
        }

        [Button]
        private void ResetHealthAndStamina()
        {
            statSetter.SetToMaxHealth();
            statSetter.SetToMaxStamina();
        }

        #endregion

    }

}