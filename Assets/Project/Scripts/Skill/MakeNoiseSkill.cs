namespace ReGaSLZR.Dare.Skill
{

    using Enum;
    using Misc;
    using Model;
    
    using NaughtyAttributes;
    using UnityEngine;
    using Zenject;

    public class MakeNoiseSkill : BaseSkill
    {

        [Inject]
        private readonly INoiseActionGetter noiseActions;

        [SerializeField]
        [MinMaxSlider(0, 5)]
        private Vector2 animVariation;

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
                (int)animVariation.x, (int)animVariation.y + 1);

            animator.SetInteger(animTrigger, idleVariation);
            SetFXActive(true);

            noiseMaker.SetNoise(noiseActions.OtherNoise(noiseType));

            return true;
        }

    }


}