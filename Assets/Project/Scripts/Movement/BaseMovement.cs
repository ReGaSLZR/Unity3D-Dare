namespace ReGaSLZR.Dare.Movement
{

    using NaughtyAttributes;
    using UnityEngine;

    public abstract class BaseMovement : MonoBehaviour
    {

        #region Inspector Variables

        //Components

        [Foldout("Components")]
        [SerializeField]
        [Required]
        protected new Rigidbody rigidbody;

        [Foldout("Components")]
        [SerializeField]
        [Required]
        protected Animator animator;

        //Animation Params

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animFloatSpeed;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerCover;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerUnCover;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerFall;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animBoolOnGround;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerStaminaOut;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animBoolStaminaOut;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerStagger;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        protected string animTriggerDie;

        //Movement Calibration

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        protected float speedRotation = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        protected float speedCrouch = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        protected float speedWalk = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        protected float speedRunAdditional = 5f;

        #endregion

        public abstract bool OnMove(bool isMoving, 
            bool isCrouching, bool isStaminaOut);

        public virtual void ResetAnimator()
        {
            animator.Rebind();
            animator.Update(0f);
        }

        public virtual void OnGround(bool isOnGround, bool isCrouching)
        {
            if (!isOnGround)
            {
                animator.SetTrigger(animTriggerFall);
            }
            else if (isCrouching)
            {
                animator.SetTrigger(animTriggerCover);
            }

            animator.SetBool(animBoolOnGround, isOnGround);
        }

        public virtual void OnCrouch(bool isCrouching)
        {
            animator.SetTrigger(isCrouching ?
                animTriggerCover : animTriggerUnCover);
        }

        public virtual void OnStagger(bool isDead)
        {
            rigidbody.velocity = Vector3.zero;
            animator.ResetTrigger(animTriggerStagger);
            animator.SetTrigger(isDead ? animTriggerDie : animTriggerStagger);
        }

        public virtual void OnUpdateStamina(int stamina, int criticalStamina)
        {
            bool isStaminaOut = (stamina <= 0);
            bool isPlayingStaminaOut = animator.GetBool(animBoolStaminaOut);

            if (isStaminaOut && !isPlayingStaminaOut)
            {
                animator.SetBool(animBoolStaminaOut, true);
                animator.SetTrigger(animTriggerStaminaOut);
            }
            else if ((stamina > criticalStamina)
                && isPlayingStaminaOut)
            {
                animator.SetBool(animBoolStaminaOut, false);
            }
        }

        public virtual bool IsStaminaOutPlaying()
        {
            return animator.GetBool(animBoolStaminaOut);
        }

    }

}