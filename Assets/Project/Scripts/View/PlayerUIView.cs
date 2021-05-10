namespace ReGaSLZR.Dare.View
{

    using Dare.Model.Player;

    using NaughtyAttributes;
    using System.Collections;
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

        [Header("Sliders")]

        [SerializeField]
        [Required]
        private Slider sliderHealth;

        [SerializeField]
        [Required]
        private Slider sliderStamina;

        [SerializeField]
        [Required]
        private Slider sliderNoise;

        [Header("Overlays")]

        [SerializeField]
        [Required]
        private Graphic overlayOnStagger;

        [SerializeField]
        [Required]
        private Graphic overlayOnCrouch;

        [Header("Configuration")]

        [SerializeField]
        [Range(0.1f, 5f)]
        private float durationCrouchOverlay = 1f;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float durationStaggerOverlay = 1f;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float alphaStaggerMax = 0.5f;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            sliderHealth.maxValue = playerStatus.GetMaxHealth();
            sliderStamina.maxValue = playerStatus.GetMaxStamina();
            sliderNoise.maxValue = playerStatus.GetMaxNoise();
        }

        private void OnEnable()
        {
            playerStatus.IsCrouching()
                .Subscribe(isCrouching =>
                    overlayOnCrouch.CrossFadeAlpha(
                        isCrouching ? 1f : 0f, durationCrouchOverlay, false))
                .AddTo(disposables);

            playerStatus.Health()
                .Subscribe(health => sliderHealth.value = health)
                .AddTo(disposables);

            playerStatus.Stamina()
                .Subscribe(stamina => sliderStamina.value = stamina)
                .AddTo(disposables);

            playerStatus.Noise()
                .Subscribe(noise => sliderNoise.value = noise)
                .AddTo(disposables);

            playerStatus.OnTakeDamage()
                .Where(hasTakenDamage => hasTakenDamage)
                .Subscribe(_ => StartCoroutine(CorOnHealthDecrease()))
                .AddTo(disposables);

            playerStatus.Health()
                .Where(health => health <= 0)
                .Subscribe(_ => StartCoroutine(CorOnHealthDecrease()))
                .AddTo(disposables);

            ResetOverlaysAlpha();
        }

        private void OnDisable()
        {
            disposables.Clear();
        }

        #endregion

        #region Class Implementation

        private void ResetOverlaysAlpha()
        {
            overlayOnCrouch.CrossFadeAlpha(0f, 0f, false);
            overlayOnStagger.CrossFadeAlpha(0f, 0f, false);
        }

        private IEnumerator CorOnHealthDecrease()
        {
            overlayOnStagger.CrossFadeAlpha(0f, 0f, true);
            overlayOnStagger.CrossFadeAlpha(alphaStaggerMax,
                durationStaggerOverlay / 2, true);
            yield return new WaitForSeconds(
                durationStaggerOverlay / 2);
            overlayOnStagger.CrossFadeAlpha(0f,
                durationStaggerOverlay / 2, true);
        }

        #endregion

    }

}