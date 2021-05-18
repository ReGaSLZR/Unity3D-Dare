namespace ReGaSLZR.Dare.Skill
{

    using NaughtyAttributes;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Playables;

    public class ShieldSkill : BaseSkill
    {

        #region Inspector Variables

        [Header("Shield Config")]

        [SerializeField]
        [Required]
        private Renderer shieldRenderer;

        [SerializeField]
        [Required]
        private PlayableDirector shieldOnEntrance;

        [SerializeField]
        [Required]
        private PlayableDirector shieldOnExit;

        #endregion

        #region Class Implementation

        public override void Aim()
        {
            //No Aim feature for this skill.
            return;
        }

        public override bool Execute(bool trigger = false)
        {
            if (!trigger && 
                !shieldRenderer.gameObject.activeInHierarchy)
            {
                isInEffect.Value = false;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(CorUpdateShield(trigger));
            }

            return isInEffect.Value;
        }

        private IEnumerator CorUpdateShield(bool isShielding)
        {
            SetFXActive(false);
            shieldRenderer.gameObject.SetActive(true);

            if (isShielding)
            {
                PlayAnimation();
                yield return new WaitForSeconds(delayOnActivate);
                SetFXActive(true);
                shieldOnEntrance.Play();
            }
            else 
            {
                shieldOnExit.Play();
                yield return new WaitForSeconds((float)shieldOnExit.duration);
                shieldRenderer.gameObject.SetActive(false);
            }

            isInEffect.Value = isShielding;
        }

        #endregion

    }

}