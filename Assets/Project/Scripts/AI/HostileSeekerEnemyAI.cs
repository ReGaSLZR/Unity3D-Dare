namespace ReGaSLZR.Dare.AI
{

    using Dare.Stats;

    using UnityEngine;
    using UniRx;

    /// <summary>
    /// As a hostile, inflicts damage to the target sought.
    /// </summary>
    public class HostileSeekerEnemyAI : SeekerEnemyAI
    {

        [SerializeField]
        [Range(1, 100)]
        private int damage;

        protected override void OnEnable()
        {
            base.OnEnable();

            skillMain.IsInEffect()
                .Where(isInEffect => isInEffect)
                .Subscribe(_ => OnSkillUse())
                .AddTo(disposableSkill);
        }

        private void OnSkillUse()
        {
            if (skillUseOnRangeDetector.CollidedObject != null)
            {
                var status = skillUseOnRangeDetector.CollidedObject
                    .GetComponent<BaseStats>();

                if (status != null)
                {
                    status.DamageHealth(damage);
                }
            }
        }

    }

}