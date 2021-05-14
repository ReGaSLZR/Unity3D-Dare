namespace ReGaSLZR.Dare.Skill
{

    using NaughtyAttributes;
    using UnityEngine;

    public abstract class BaseSkill : MonoBehaviour
    {

        #region Inspector Variables

        [Header("Base Config")]

        [SerializeField]
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
        [Range(0.001f, 5f)]
        protected float activationDamping = 0.1f;

        [SerializeField]
        [Range(0.1f, 5f)]
        protected float delayOnActivate = 1f;

        [SerializeField]
        protected GameObject[] effectsObj;

        #endregion

        public bool isInEffect { get; protected set; } = false;

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
                fx.SetActive(isActive);
            }
        }

    }

}