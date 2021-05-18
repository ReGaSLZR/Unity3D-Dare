namespace ReGaSLZR.Dare.Skill
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    public abstract class BaseSkill : MonoBehaviour
    {

        #region Inspector Variables

        [Header("Base Config")]

        [SerializeField]
        private bool isPlayerSkill = false;

        [SerializeField]
        [ShowIf("isPlayerSkill")]
        [InputAxis]
        protected string skillButton;

        [Space]

        [SerializeField]
        [Required]
        protected Animator animator;

        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTrigger;

        [Space]

        [SerializeField]
        [Range(0.1f, 5f)]
        protected float delayOnActivate = 1f;

        [SerializeField]
        protected GameObject[] effectsObj;

        #endregion

        protected ReactiveProperty<bool> isInEffect = new ReactiveProperty<bool>(false);

        #region Unity Callbacks

        protected virtual void Start()
        {
            SetFXActive(false);
        }

        #endregion

        #region Base Methods

        public virtual string GetSkillButton()
        {
            return skillButton;
        }

        public IReadOnlyReactiveProperty<bool> IsInEffect()
        {
            return isInEffect;
        }

        public abstract void Aim();

        /// <summary>
        /// Returns true if the call was completed.
        /// </summary>
        public abstract bool Execute(bool trigger = false); 

        #endregion

        protected void SetFXActive(bool isActive)
        {
            foreach (var fx in effectsObj)
            {
                fx?.SetActive(isActive);
            }
        }

        protected void PlayAnimation()
        {
            animator.ResetTrigger(animTrigger);
            animator.SetTrigger(animTrigger);
        }

    }

}