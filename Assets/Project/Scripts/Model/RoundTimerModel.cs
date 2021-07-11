namespace ReGaSLZR.Dare.Model
{

    using Enum;
    using Util;

    using UniRx;
    using UnityEngine;
    using Zenject;

    public interface IRoundTimerSetter
    {
        public void StartRound(int preRoundCountdown, int roundDuration);
        public void ToggleRoundPaused();
        public void TriggerGameOver();
        public void TriggerNewRound();

    }

    public interface IRoundTimerGetter
    {
        public IReadOnlyReactiveProperty<int> Countdown();
        public IReadOnlyReactiveProperty<int> RoundNumber();
        public IReadOnlyReactiveProperty<RoundState> RoundState();
    }

    public class RoundTimerModel : MonoInstaller,
        IRoundTimerSetter, IRoundTimerGetter
    {

        #region Reactive Variables

        private ReactiveProperty<int> roundPreCountdown = new ReactiveProperty<int>();
        private ReactiveProperty<int> roundCountdown = new ReactiveProperty<int>();
        private ReactiveProperty<int> roundNumber = new ReactiveProperty<int>();
        private ReactiveProperty<RoundState> roundState = new ReactiveProperty<RoundState>();

        private CompositeDisposable disposables = new CompositeDisposable();

        #endregion

        private readonly System.TimeSpan TICK = System.TimeSpan.FromSeconds(1);

        public override void InstallBindings()
        {
            Container.Bind<IRoundTimerSetter>().FromInstance(this);
            Container.Bind<IRoundTimerGetter>().FromInstance(this);
        }

        #region Class Implementation

        private void Awake()
        {
            roundState.SetValueAndForceNotify(Enum.RoundState.NotStarted);
        }

        private void OnEnable()
        {
            Observable.Interval(TICK)
                .Where(_ => roundState.Value == Enum.RoundState.CountdownToStart)
                .Where(_ => roundPreCountdown.Value > 0)
                .Subscribe(_ =>
                {
                    roundPreCountdown.Value--;

                    if (roundPreCountdown.Value == 0)
                    {
                        roundState.SetValueAndForceNotify(Enum.RoundState.InPlay);
                    }
                })
                .AddTo(disposables);

            Observable.Interval(TICK)
                .Where(_ => roundState.Value == Enum.RoundState.InPlay)
                .Where(_ => roundCountdown.Value > 0)
                .Subscribe(_ =>
                {
                    roundCountdown.Value--;

                    if (roundCountdown.Value == 0)
                    {
                        roundState.SetValueAndForceNotify(Enum.RoundState.Finished);
                    }
                })
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear(); 
        }

        #endregion

        #region Setter

        public void StartRound(int preRoundCountdown, int roundDuration)
        {
            if (Enum.RoundState.NotStarted == roundState.Value)
            {
                roundPreCountdown.SetValueAndForceNotify(preRoundCountdown);
                roundCountdown.SetValueAndForceNotify(roundDuration);
                roundState.SetValueAndForceNotify(Enum.RoundState.CountdownToStart);
            }
            else 
            {
                LogUtil.PrintInfo(GetType(), "StartRound()" +
                    " Call skipped.");
            }
        }

        public void ToggleRoundPaused()
        {
            switch (roundState.Value)
            {
                case Enum.RoundState.InPlay:
                    {
                        roundState.Value = Enum.RoundState.Paused;
                        Time.timeScale = 0;
                        break;
                    }
                case Enum.RoundState.Paused:
                    {
                        roundState.Value = Enum.RoundState.InPlay;
                        Time.timeScale = 1;
                        break;
                    }
                default:
                    {
                        LogUtil.PrintInfo(GetType(), "ToggleRoundPaused()" +
                            " Cannot toggle Paused / UnPaused round state.");
                        break;
                    }
            }
        }

        public void TriggerGameOver()
        {
            if (Enum.RoundState.InPlay == roundState.Value)
            {
                roundState.SetValueAndForceNotify(Enum.RoundState.GameOver);
            }
        }
        public void TriggerNewRound()
        {
            if (Enum.RoundState.Finished == roundState.Value)
            {
                roundNumber.Value++;
                roundState.SetValueAndForceNotify(Enum.RoundState.NotStarted);
            }
        }

        #endregion

        #region Getter

        public IReadOnlyReactiveProperty<int> Countdown()
        {
            return roundCountdown;
        }
        
        public IReadOnlyReactiveProperty<int> RoundNumber()
        {
            return roundNumber;
        }

        public IReadOnlyReactiveProperty<RoundState> RoundState()
        {
            return roundState;
        }

        #endregion

    }

}