namespace ReGaSLZR.Dare.Skill
{

    using System.Collections;
    using UnityEngine;

    public class LoopingSkill : BaseSkill
    {

        #region Inspector Variables

        [SerializeField]
        [Range(0.5f, 5f)]
        private float executeInterval = 1f;

        #endregion

        #region Class Overrides

        public override void Aim()
        {
            //TODO There's no Aim mechanic for this skill.
            return;
        }

        /// <summary>
        /// Use the IsInEffect(), instead of the return value of Execute() method
        /// for checking if the attack landed.
        /// </summary>
        public override bool Execute(bool trigger = false)
        {
            isInEffect.Value = false;
            StopAllCoroutines();

            if (trigger)
            {
                StartCoroutine(CorExecute());
            }

            return true;
        }

        #endregion

        #region Class Implementation

        private IEnumerator CorExecute()
        {
            while (true)
            {
                PlayAnimation();

                isInEffect.Value = false;
                SetFXActive(false);

                yield return new WaitForSeconds(delayOnActivate);
                
                isInEffect.Value = true;
                SetFXActive(true);

                yield return new WaitForSeconds(executeInterval);
            }
        }

        #endregion

    }

}