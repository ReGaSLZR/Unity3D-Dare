namespace ReGaSLZR.Dare.Stats
{

    using Enum;
    using Model;

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class BaseStats : MonoBehaviour
    {

        [Inject]
        private readonly IRoundTimerGetter roundTimer;

        [SerializeField]
        protected bool isDisabledOnRoundFinish;

        [SerializeField]
        [Required]
        protected GameObject parent;

        protected ReactiveProperty<int> health = new ReactiveProperty<int>(100);
        protected CompositeDisposable disposables = new CompositeDisposable();

        protected virtual void Start()
        {
            health.Where(healthVal => healthVal <= 0)
                .Subscribe(_ => parent.SetActive(false))
                .AddTo(disposables);

            if (isDisabledOnRoundFinish)
            {
                roundTimer.RoundState()
                    .Subscribe(state =>
                    {
                        if (RoundState.Finished == state)
                        {
                            parent.SetActive(false);
                        }
                        else if(RoundState.CountdownToStart == state) 
                        {
                            parent.SetActive(true);
                        }
                    }).AddTo(disposables);
            }
        }

        protected virtual void OnDestroy()
        {
            disposables.Clear();
        }

        public virtual void DamageHealth(int damage)
        {
            health.Value = Mathf.Clamp(health.Value - damage, 0, 100);
        }

        public IReadOnlyReactiveProperty<int> Health()
        {
            return health;
        }

        public virtual void InitHealthValue(int health)
        {
            this.health.Value = health;
        }

        public void DisableParentGameObject()
        {
            parent.SetActive(false);
        }

    }

}