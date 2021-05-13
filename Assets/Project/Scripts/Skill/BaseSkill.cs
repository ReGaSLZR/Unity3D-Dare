namespace ReGaSLZR.Dare.Skill
{

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    public abstract class BaseSkill : MonoBehaviour
    {

        #region Private Variables

        protected readonly CompositeDisposable disposables = new CompositeDisposable();

        #endregion

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

        #region Unity Callbacks

        protected virtual void OnEnable()
        {
            OnReady();
        }

        protected virtual void OnDisable()
        {
            disposables.Clear();
        }

        private void Start()
        {
            SetFXActive(false);
        }

        #endregion

        #region Base Methods

        protected abstract void OnReady();

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