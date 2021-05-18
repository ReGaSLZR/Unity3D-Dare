namespace ReGaSLZR.Dare.AI
{

    using UniRx;

    /// <summary>
    /// As a hostile, inflicts damage to the target sought.
    /// </summary>
    public class HostileSeekerEnemyAI : SeekerEnemyAI
    {

        protected override void OnEnable()
        {
            base.OnEnable();

            skillMain.IsInEffect()
                .Subscribe(isInEffect => OnSkillUse(isInEffect))
                .AddTo(disposableSkill);
        }

        private void OnSkillUse(bool isSkillInUse)
        { 
            
        }

    }

}