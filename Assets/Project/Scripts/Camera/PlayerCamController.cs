namespace ReGaSLZR.Dare.Camera
{

    using Dare.Model.Player;

    using Cinemachine;
    using NaughtyAttributes;
    using UnityEngine;
    using UniRx;
    using UniRx.Triggers;
    using Zenject;

    public class PlayerCamController : MonoBehaviour
    {

        [Inject]
        private IPlayerStatusGetter playerStatus;

        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private float[] origOrbitRadius;

        #region Inspector Variables

        [SerializeField]
        [Required]
        private CinemachineFreeLook playerCam;

        [Space]

        [SerializeField]
        [Range(0.1f, 5f)]
        private float orbitRadiusOnRun = 1f;

        [SerializeField]
        [Range(0.1f, 10f)]
        private float radiusChangeSpeed = 2f;

        #endregion

        private void Awake()
        {
            CacheOriginalOrbit();
        }

        private void OnEnable()
        {
            this.UpdateAsObservable()
                .Select(_ => playerStatus.IsRunning().Value)
                .Subscribe(isRunning => OnPlayerRun(isRunning))
                .AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }

        #region Class Implementation

        private void CacheOriginalOrbit()
        {
            origOrbitRadius = new float[playerCam.m_Orbits.Length];

            for (int x=0; x<origOrbitRadius.Length; x++)
            {
                origOrbitRadius[x] = playerCam.m_Orbits[x].m_Radius;
            }
        }

        private void OnPlayerRun(bool isRunning)
        {
            for (int x=0; x<playerCam.m_Orbits.Length; x++)
            {
                var targetRadius = isRunning
                    ? orbitRadiusOnRun : origOrbitRadius[x];
                var currentRadius = playerCam.m_Orbits[x].m_Radius;

                playerCam.m_Orbits[x].m_Radius = Mathf.Lerp(
                    currentRadius, targetRadius, 
                    Time.deltaTime * radiusChangeSpeed);
            }
        }

        #endregion

    }

}