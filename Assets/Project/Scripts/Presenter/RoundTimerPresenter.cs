namespace ReGaSLZR.Dare.Presenter
{

    using Model;

    using UnityEngine;
    using Zenject;

    public class RoundTimerPresenter : MonoBehaviour
    {

        [Inject]
        private readonly IRoundTimerGetter roundGetter;
        [Inject]
        private readonly IRoundTimerSetter roundSetter;
        [Inject]
        private readonly IRoundSettingsGetter roundSettingsGetter;

        #region Unity Callbacks

        private void Start()
        {
            roundSetter.StartRound(
                roundSettingsGetter.PreRoundCountdown(), 
                roundSettingsGetter.RoundDuration());
        }

        #endregion

    }

}