namespace ReGaSLZR.Dare.Movement
{

    using Dare.Detector;
    using Dare.Model.Status;

    using NaughtyAttributes;
    using UnityEngine;
    using UniRx;
    using UniRx.Triggers;
    using Zenject;

    /// <summary>
    /// Controls the animation execution and position change
    /// for the following Player Movement:
    /// Idle, Walk, Run, Fall, Crouch, Crouched Walk
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {

        [Inject]
        private PlayerStatusGetter playerStatusGetter;

        [Inject]
        private PlayerStatusSetter playerStatusSetter;

        #region Inspector Variables

        //Components

        [Foldout("Components")]
        [SerializeField]
        [Required]
        private new Rigidbody rigidbody;

        [Foldout("Components")]
        [SerializeField]
        [Required]
        private Animator animator;

        [Foldout("Components")]
        [SerializeField]
        [Required]
        private CollisionDetector groundDetector;

        //Animation Params

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerCover;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerUnCover;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerFall;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animBoolOnGround;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerText;

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerUnText;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerPickUp;

        [Space]

        [Foldout("Animation Params")]
        [AnimatorParam("animator")]
        [SerializeField]
        private string animFloatSpeed;

        //Movement Calibration

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        private float speedRotation = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        private float speedCrouch = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        private float speedWalk = 3f;

        [Foldout("Movement Calibration")]
        [SerializeField]
        [Range(0f, 10f)]
        private float speedRunAdditional = 5f;

        #endregion

        #region Private Variables

        private Transform mainCam;
        private float turnSmoothVelocity;

        private CompositeDisposable disposable = new CompositeDisposable();

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            mainCam = Camera.main.transform;
        }

        private void OnEnable()
        {
            this.FixedUpdateAsObservable()
                .Where(_ => playerStatusGetter.IsOnGround().Value)
                .Select(_ => HasMovementInput())
                .Subscribe(isMoving => OnMove(isMoving, speedWalk))
                .AddTo(disposable);

            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown("Crouch") &&
                    !playerStatusGetter.IsOnGround().Value)
                .Subscribe(_ => OnCrouch())
                .AddTo(disposable);

            groundDetector.HasCollision()
                .Subscribe(hasCollision => OnGround(hasCollision))
                .AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }

        #endregion

        #region Class Implementation

        private bool HasMovementInput()
        {
            return (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0
                        || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0);
        }

        private void OnCrouch()
        {
            playerStatusSetter.ToggleIsCrouching();
            animator.SetTrigger(playerStatusGetter.IsCrouching().Value ?
                animTriggerCover : animTriggerUnCover);
        }

        private void OnGround(bool isOnGround)
        {
            playerStatusSetter.SetIsOnGround(isOnGround);

            if (!isOnGround)
            {
                animator.SetTrigger(animTriggerFall);

                //auto-cancel ongoing crouched state
                if (playerStatusGetter.IsCrouching().Value)
                {
                    playerStatusSetter.ToggleIsCrouching();
                }
            }

            animator.SetBool(animBoolOnGround, isOnGround);
        }

        /// <summary>
        /// Parts of this were taken from Brackey's tutorial:
        /// https://www.youtube.com/watch?v=4HpC--2iowE
        /// Prerequisite: The camera follows and looks at the Player
        /// but can also rotate/orbit around the Player.
        /// (Cinemachine Free-look VCam).
        /// </summary>
        private void OnMove(bool isMoving, float speed)
        {
            float hori = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            bool isCrouching = playerStatusGetter.IsCrouching().Value;
            bool isRunning = !isCrouching &&
                Input.GetButton("Run") && (vert > 0);

            if (isMoving)
            {
                float newSpeed = isRunning ? (speed + speedRunAdditional) 
                    : isCrouching ? speedCrouch
                    : speed;

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

            playerStatusSetter?.SetIsRunning(isRunning);
        }

        #endregion

    }

}