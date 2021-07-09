namespace ReGaSLZR.Dare.Stats
{

    using UnityEngine;

    public class NoiseBaitStats : BaseStats
    {

        [SerializeField]
        [Range(0, 100)]
        protected int startingHealth;

        private void OnEnable()
        {
            parent.SetActive(true);
            InitHealthValue(startingHealth);
        }

    }

}