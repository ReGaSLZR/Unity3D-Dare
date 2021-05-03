namespace ReGaSLZR.Dare.Camera
{

    using Dare.Model.Status;

    using Cinemachine;
    using NaughtyAttributes;
    using UnityEngine;
    using UniRx;
    using Zenject;

    public class CameraChanger : MonoBehaviour
    {

        [Inject]
        private PlayerStatusGetter playerStatus;

        private CompositeDisposable disposable = new CompositeDisposable();

        #region Inspector Variables

        [SerializeField]
        [Required]
        private CinemachineVirtualCameraBase playerCamNormal;

        [SerializeField]
        [Required]
        private CinemachineVirtualCameraBase playerCamRunning;

        #endregion

        private void OnEnable()
        {
            playerStatus?.IsRunning()
                .Subscribe(isRunning => OnPlayerRun(isRunning))
                .AddTo(disposable);
        }

        private void OnDisable()
        {
            disposable.Clear();
        }

        #region Class Implementation

        private void OnPlayerRun(bool isRunning)
        {
            playerCamNormal.gameObject.SetActive(!isRunning);
            playerCamRunning.gameObject.SetActive(isRunning);
        }

        #endregion

    }

}