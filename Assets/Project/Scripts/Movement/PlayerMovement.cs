namespace ReGaSLZR.Dare.Movement
{

    using NaughtyAttributes;
    using UnityEngine;
 
    public class PlayerMovement : BaseMovement
    {

        #region Inspector Variables

        [Foldout("Input Params")]
        [SerializeField]
        [InputAxis]
        private string inputMoveVertical;

        [Foldout("Input Params")]
        [SerializeField]
        [InputAxis]
        private string inputMoveHorizontal;

        [Foldout("Input Params")]
        [SerializeField]
        [InputAxis]
        private string inputRun;

        [Foldout("Input Params")]
        [SerializeField]
        [InputAxis]
        private string inputCrouch;

        #endregion

        #region Private Variables

        private Transform mainCam;
        private float turnSmoothVelocity;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            mainCam = Camera.main.transform;
        }

        #endregion

        #region Class Implementation

        public bool HasMovementInput()
        {
            return (Mathf.Abs(Input.GetAxisRaw(inputMoveVertical)) > 0
                        || Mathf.Abs(Input.GetAxisRaw(inputMoveHorizontal)) > 0);
        }

        public bool HasCrouchInput()
        {
            return Input.GetButtonDown(inputCrouch);
        }

        /// <summary>
        /// Parts of this were taken from Brackey's tutorial:
        /// https://www.youtube.com/watch?v=4HpC--2iowE
        /// Prerequisite: The camera follows and looks at the Player
        /// but can also rotate/orbit around the Player.
        /// (Cinemachine Free-look VCam).
        /// </summary>
        public override bool OnMove(bool isMoving, bool isCrouching, bool isStaminaOut)
        {
            float hori = Input.GetAxisRaw(inputMoveHorizontal);
            float vert = Input.GetAxisRaw(inputMoveVertical);
            bool isRunning = Input.GetButton(inputRun) && isStaminaOut && (vert > 0);

            if (isCrouching && isRunning)
            {
                OnCrouch(false);
                isCrouching = false;
            }

            if (isMoving)
            {
                float newSpeed = isRunning ? (speedWalk + speedRunAdditional) 
                    : isCrouching ? speedCrouch
                    : speedWalk;

                Vector3 direction = new Vector3(hori, 0, vert).normalized;
                float targetAngle = Mathf.Atan2(direction.x, direction.z) 
                    * Mathf.Rad2Deg + mainCam.eulerAngles.y;

                float angle = Mathf.SmoothDampAngle(rigidbody.transform.eulerAngles.y, 
                    targetAngle, ref turnSmoothVelocity, 
                    isRunning ? (speedRotation/8f) : speedRotation);
                rigidbody.transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) 
                    * Vector3.forward;
                rigidbody.position += (movementDirection.normalized 
                    * newSpeed * Time.fixedDeltaTime);
                animator.SetFloat(animFloatSpeed, newSpeed);
            }
            else 
            {
                animator.SetFloat(animFloatSpeed, 0);
                rigidbody.angularVelocity = Vector3.zero;
            }

            return isRunning;
        }

        #endregion

    }

}