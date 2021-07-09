namespace ReGaSLZR.Dare.Skill
{

    using Model;
    using Model.Player;

    using NaughtyAttributes;
    using UnityEngine;
    using Zenject;

    public class MakeNoiseSkill : BaseSkill
    {

        [Inject]
        private readonly IPlayerStatusGetter playerStats;

        [SerializeField]
        [MinMaxSlider(0, 5)]
        private Vector2 animIdleVarValues;

        [Space]

        [SerializeField]
        private NoiseMaker noiseMaker;

        [SerializeField]
        private NoiseType noiseType;

        public override void Aim()
        {
            //NOTE: No need for aiming on this one.
        }

        public override bool Execute(bool trigger = false)
        {
            int idleVariation = Random.Range(
                (int)animIdleVarValues.x, (int)animIdleVarValues.y + 1);

            animator.SetInteger(animTrigger, idleVariation);

            noiseMaker.SetNoise(playerStats
                .GetNoiseActions().GetOtherNoise(noiseType));

            return true;
        }

    }


}