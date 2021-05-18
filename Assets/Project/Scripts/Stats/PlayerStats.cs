namespace ReGaSLZR.Dare.Stats
{

    using Dare.Model.Player;

    using UniRx;
    using Zenject;

    public class PlayerStats : BaseStats
    {

        [Inject]
        private IPlayerStatusGetter playerStatusGetter;

        [Inject]
        private IPlayerStatusSetter playerStatusSetter;

        [Inject]
        private IPlayerSkillGetter playerSkill;

        protected override void Start()
        {
            InitHealthValue(playerStatusGetter.Health().Value);

            playerStatusGetter.Health()
                .Where(healthVal => healthVal > health.Value)
                .Subscribe(healthVal => health.Value = healthVal)
                .AddTo(disposables);
        }

        public override void DamageHealth(int damage)
        {
            if (playerSkill.IsShielding().Value)
            {
                return;
            }

            base.DamageHealth(damage);
            playerStatusSetter.Damage(damage);
        }

    }

}