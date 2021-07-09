namespace ReGaSLZR.Dare.Stats
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    public class BaseStats : MonoBehaviour
    {

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