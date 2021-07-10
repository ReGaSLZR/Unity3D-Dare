namespace ReGaSLZR.Dare.Model
{
    
    using UnityEngine;
    using Zenject;

    public interface IStaminaCostsSetter
    {
        //TODO
    }

    public interface IStaminaCostsGetter
    {
        public int SkillSummonBait();
        public float RunTick();
        public float SkillShieldTick();
        public float RefillTick();
    }

    [CreateAssetMenu(menuName = "Dare/Stamina Costs", fileName = "New StaminaCosts")]
    public class StaminaCostsModel : ScriptableObjectInstaller<StaminaCostsModel>,
        IStaminaCostsGetter, IStaminaCostsSetter
    {

        public const float TICK_RUN_MAX = 0.1f;
        public const float TICK_RUN_MIN = 0.01f;

        public const int COST_SKILL_SUMMON_BAIT_MAX = 100;
        public const int COST_SKILL_SUMMON_BAIT_MIN = 1;

        [SerializeField]
        [Tooltip("Lower value = lower cost")]
        [Range(COST_SKILL_SUMMON_BAIT_MIN, COST_SKILL_SUMMON_BAIT_MAX)]
        private int skillSummonBait = 85;

        [SerializeField]
        [Tooltip("Higher value = slower tick")]
        [Range(TICK_RUN_MIN, TICK_RUN_MAX)]
        private float runTick = 0.25f;

        [SerializeField]
        [Range(0.001f, 0.25f)]
        private float skillShieldTick = 0.005f;

        [SerializeField]
        [Range(0.01f, 0.25f)]
        private float refillTick = 0.25f;

        public override void InstallBindings()
        {
            Container.Bind<IStaminaCostsGetter>().FromInstance(this);
            Container.Bind<IStaminaCostsSetter>().FromInstance(this);
        }

        #region Getter Implementation

        public int SkillSummonBait()
        {
                return skillSummonBait;
        }

        public float RunTick()
        {
            return runTick;
        }

        public float SkillShieldTick()
        {
            return skillShieldTick;
        }

        public float RefillTick()
        {
            return refillTick;
        }

        #endregion

    }

}