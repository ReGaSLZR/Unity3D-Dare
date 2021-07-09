namespace ReGaSLZR.Dare.Model
{
    
    using UnityEngine;

    [CreateAssetMenu(menuName = "Dare/Stamina Costs", fileName = "New StaminaCosts")]
    public class StaminaCosts : ScriptableObject
    {

        public const float TICK_RUN_MAX = 0.1f;
        public const float TICK_RUN_MIN = 0.01f;

        public const int COST_SKILL_SUMMON_BAIT_MAX = 100;
        public const int COST_SKILL_SUMMON_BAIT_MIN = 1;

        [Tooltip("Lower value = lower cost")]
        [Range(COST_SKILL_SUMMON_BAIT_MIN, COST_SKILL_SUMMON_BAIT_MAX)]
        public int skillSummonBait = 85;

        [Tooltip("Higher value = slower tick")]
        [Range(TICK_RUN_MIN, TICK_RUN_MAX)]
        public float runTick = 0.25f;

        [Range(0.001f, 0.25f)]
        public float skillShieldTick = 0.005f;

        [Range(0.01f, 0.25f)]
        public float refillTick = 0.25f;

    }

}