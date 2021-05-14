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
                isInEffect = false;
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(CorUpdateShield(trigger));
            }

            return isInEffect;
        }

        private IEnumerator CorUpdateShield(bool isShielding)
        {
            SetFXActive(false);
            shieldRenderer.gameObject.SetActive(true);

            if (isShielding)
            {        
                animator.SetTrigger(animTrigger);
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

            isInEffect = isShielding;
        }

        #endregion

    }

}