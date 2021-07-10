namespace ReGaSLZR.Dare.Stats
{

    using Dare.Misc;
    using Dare.Model.Player;

    using UnityEngine;
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

        [SerializeField]
        private NoiseMaker noiseMaker;

        protected override void Start()
        {
            InitHealthValue(playerStatusGetter.Health().Value);

            playerStatusGetter.Health()
                .Where(healthVal => healthVal > health.Value)
                .Subscribe(healthVal => health.Value = healthVal)
                .AddTo(disposables);

            playerStatusGetter.Noise()
                .Subscribe(noise => noiseMaker.SetNoise(noise))
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