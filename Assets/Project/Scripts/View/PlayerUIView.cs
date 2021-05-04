namespace ReGaSLZR.Dare.View
{

    using Dare.Model.Status;

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class PlayerUIView : MonoBehaviour
    {

        #region Private Variables

        [Inject]
        private IPlayerStatusGetter playerStatus;

        private CompositeDisposable disposables = new CompositeDisposable();

        #endregion

        #region Inspector Variables

        [SerializeField]
        [Required]
        private Slider sliderHealth;

        [Header("Overlays")]

        [SerializeField]
        [Required]
        private Animation animOverlayOnStagger;

        [SerializeField]
        [Required]
        private Image overlayOnCrouch;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            playerStatus.IsCrouching()
                .Subscribe(isCrouching => 
                    overlayOnCrouch.CrossFadeAlpha(
                        isCrouching ? 1 : 0f, 1f, false))
                .AddTo(disposables);

            playerStatus.Health()
                .Subscribe(health => sliderHealth.value = health)
                .AddTo(disposables);

            playerStatus.OnTakeDamage()
                .Where(hasTakenDamage => hasTakenDamage)
                .Subscribe(_ => OnHealthDecrease())
                .AddTo(disposables);

            playerStatus.Health()
                .Where(health => health <= 0)
                .Subscribe(_ => OnHealthDecrease())
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
        }

        #endregion

        #region Class Implementation

        private void OnHealthDecrease()
        {
            animOverlayOnStagger.gameObject.SetActive(true);
            animOverlayOnStagger.Play();
        }

        #endregion

    }

}