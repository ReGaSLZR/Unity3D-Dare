namespace ReGaSLZR.Dare.Model
{
    
    using Enum;

    using UnityEngine;
    using Zenject;

    public interface INoiseActionSetter
    { 
        //TODO
    }

    public interface INoiseActionGetter
    {
        public int CrouchIdle();
        public int StandingIdle();
        public int CrouchWalk();
        public int Walk();
        public int Run();
        public int OtherNoise(NoiseType noiseType);
    }

    [CreateAssetMenu(menuName = "Dare/Noise Actions", fileName = "New NoiseActions")]
    public class NoiseActionsModel : ScriptableObjectInstaller<NoiseActionsModel>,
        INoiseActionGetter, INoiseActionSetter
    {

        [Header("Player Noise Actions")]

        [SerializeField]
        [Range(0, 100)]
        private int crouchIdle = 0;

        [SerializeField]
        [Range(0, 100)]
        private int standingIdle = 5;

        [SerializeField]
        [Range(0, 100)]
        private int crouchWalk = 10;

        [SerializeField]
        [Range(0, 100)]
        private int walk = 30;

        [SerializeField]
        [Range(0, 100)]
        private int run = 75;

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

        public override void InstallBindings()
        {
            Container.Bind<INoiseActionGetter>().FromInstance(this);
            Container.Bind<INoiseActionSetter>().FromInstance(this);
        }

        #region Getter Implementation

        public int StandingIdle()
        {
            return standingIdle;
        }
        public int CrouchIdle()
        {
            return crouchIdle;
        }

        public int CrouchWalk()
        {
            return crouchWalk;
        }

        public int Walk()
        {
            return walk;
        }

        public int Run()
        {
            return run;
        }

        public int OtherNoise(NoiseType noiseType)
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

        #endregion

    }

}