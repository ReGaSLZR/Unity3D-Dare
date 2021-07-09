namespace ReGaSLZR.Dare.Model
{

    using UnityEngine;

    [CreateAssetMenu(menuName = "Dare/Noise Actions", fileName = "New NoiseActions")]
    public class NoiseActions : ScriptableObject
    {

        [Header("Player Noise Actions")]

        [SerializeField]
        [Range(0, 100)]
        private int crouchIdle = 0;
        public int CrouchIdle { get { return crouchIdle; }  }

        [SerializeField]
        [Range(0, 100)]
        private int standingIdle = 5;
        public int StandingIdle { get { return standingIdle; } }

        [SerializeField]
        [Range(0, 100)]
        private int crouchWalk = 10;
        public int CrouchWalk { get { return crouchWalk; }  }

        [SerializeField]
        [Range(0, 100)]
        private int walk = 30;
        public int Walk { get { return walk; }  }

        [SerializeField]
        [Range(0, 100)]
        private int run = 75;
        public int Run { get { return run; } }

        [Header("Other Noise Sources")]

        [SerializeField]
        [Range(0, 100)]
        private int bait = 75;

        [SerializeField]
        [Range(0, 100)]
        private int enemyMinion = 75;

        [SerializeField]
        [Range(0, 100)]
        private int environmentTrap = 25;

        public int GetOtherNoise(NoiseType noiseType)
        {
            switch (noiseType)
            {
                case NoiseType.Bait:
                    {
                        return bait;
                    }
                case NoiseType.EnemyMinion:
                    {
                        return enemyMinion;
                    }
                default:
                case NoiseType.EnvironmentTrap:
                    {
                        return environmentTrap;
                    }
            }
        }

    }

}