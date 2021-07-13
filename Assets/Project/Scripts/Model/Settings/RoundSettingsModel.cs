namespace ReGaSLZR.Dare.Model
{

    using UnityEngine;
    using Zenject;

    public interface IRoundSettingsSetter
    {
        //TODO
    }

    public interface IRoundSettingsGetter
    {
        public int RoundDuration();
        public int PreRoundCountdown();
        public int StartingRoundNumber();
    }

    [CreateAssetMenu(fileName = "New Round Settings", menuName = "Dare/Round Settings")]
    public class RoundSettingsModel : ScriptableObjectInstaller,
        IRoundSettingsGetter, IRoundSettingsSetter
    {

        [SerializeField]
        [Tooltip("The countdown (in seconds) for each round.")]
        [Range(10, 100)]
        private int roundDuration;

        [SerializeField]
        [Tooltip("The countdown (in seconds) before a round starts.")]
        [Range(3, 10)]
        private int preRoundCountdown = 3;

        [SerializeField]
        private int startingRoundNumber = 1;


        public override void InstallBindings()
        {
            Container.Bind<IRoundSettingsGetter>().FromInstance(this);
            Container.Bind<IRoundSettingsSetter>().FromInstance(this);
        }

        #region Getter Implementation

        public int RoundDuration()
        {
            return roundDuration;
        }
        public int PreRoundCountdown()
        {
            return preRoundCountdown;
        }

        public int StartingRoundNumber()
        {
            return startingRoundNumber;
        }

        #endregion

    }


}