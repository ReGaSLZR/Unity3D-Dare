namespace ReGaSLZR.Dare.Stats
{

    using Dare.Model;

    using UnityEngine;

    public class NoiseBaitStats : BaseStats
    {

        [SerializeField]
        private NoiseMaker noiseMaker;

        private void OnEnable()
        {
            InitHealthValue(startingHealth);

            //TODO set noiseMaker level from upgrades
            //noiseMaker.SetNoise(val);
        }

    }

}