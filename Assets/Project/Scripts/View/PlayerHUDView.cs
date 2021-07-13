namespace ReGaSLZR.Dare.View
{

    using Dare.Model;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Zenject;

    public class PlayerHUDView : MonoBehaviour
    {

        #region Private Variables

        [Inject]
        private readonly IPlayerStatusGetter playerStatus;
        [Inject]
        private readonly IRoundTimerGetter roundTimer;

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

        [Header("Texts")]

        [SerializeField]
        [Required]
        private TextMeshProUGUI textRoundCountdown;

        [SerializeField]
        [Required]
        private TextMeshProUGUI textRoundState;

        [SerializeField]
        [Required]
        private TextMeshProUGUI textRoundNumber;

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
            //Texts

            roundTimer.RoundState()
                .Subscribe(state => textRoundState.text = state.ToString())
                .AddTo(disposables); //TODO remove this

            roundTimer.Countdown()
                .Subscribe(countdown => textRoundCountdown.text = countdown.ToString())
                .AddTo(disposables);

            roundTimer.RoundNumber()
                .Subscribe(num => textRoundNumber.text = num.ToString())
                .AddTo(disposables);

            //Sliders

            playerStatus.Health()
                .Subscribe(health => sliderHealth.value = health)
                .AddTo(disposables);

            playerStatus.Stamina()
                .Subscribe(stamina => sliderStamina.value = stamina)
                .AddTo(disposables);

            playerStatus.Noise()
                .Subscribe(noise => sliderNoise.value = noise)
                .AddTo(disposables);

            //Overlays

            playerStatus.IsCrouching()
                .Subscribe(isCrouching =>
                    overlayOnCrouch.CrossFadeAlpha(
                        isCrouching ? 1f : 0f, durationCrouchOverlay, false))
                .AddTo(disposables);

            playerStatus.OnTakeDamage()
                .Where(hasTakenDamage => hasTakenDamage)
                .Subscribe(_ => StartCoroutine(CorOnHealthDecrease()))
                .AddTo(disposables);

            playerStatus.Health()
                .Where(health => health <= 0)
                .Subscribe(_ => StartCoroutine(CorOnHealthDecrease()))
                .AddTo(disposables);
        }

        private void OnDisable()
        {
            disposables.Clear();
        }

        private void Start()
        {
            ResetOverlaysAlpha();
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