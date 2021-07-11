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


    }

}