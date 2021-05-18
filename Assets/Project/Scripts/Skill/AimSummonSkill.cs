namespace ReGaSLZR.Dare.Skill
{

    using Dare.Detector;

    using Cinemachine;
    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UnityEngine;

    public class AimSummonSkill : BaseSkill
    {

        #region Inspector Variables

        [Header("Aim Camera")]

        [SerializeField]
        [Required]
        private CinemachineVirtualCameraBase aimCamera;

        [SerializeField]
        [Range(0f, 5f)]
        private float delayAimCameraDeactivation = 1f;

        [Header("Summon Config")]

        [SerializeField]
        [Required]
        private GameObject summon;

        [SerializeField]
        [Required]
        private CollisionDetector aimSpotDetector;

        [SerializeField]
        [Required]
        private Transform spawnMarker;

        [SerializeField]
        [Tag]
        private string tagForSpawningAt;

        #endregion

        private Camera mainCam;
        private readonly Vector2 center = new Vector3(Screen.width / 2, Screen.height / 2);
        private RaycastHit hit;
        private CompositeDisposable disposables = new CompositeDisposable();

        #region Overriden Methods

        private void Awake()
        {
            mainCam = Camera.main;
        }

        protected override void Start()
        {
            base.Start();
            summon.SetActive(false);
            aimCamera.gameObject.SetActive(false);
            spawnMarker.gameObject.SetActive(false);
        }

        #endregion

        #region Class Implementation

        public override void Aim()
        {
            aimCamera.gameObject.SetActive(true);

            //baitSpawnAimer.HasCollision()
            //    .Subscribe(hasCollision => {
            //        if (hasCollision)
            //        {
            //            baitSpawnMarker.gameObject.SetActive(true);
            //            baitSpawnMarker.position = baitSpawnAimer.ContactPoint;
            //        }
            //        else
            //        {
            //            baitSpawnMarker.gameObject.SetActive(false);
            //        }
            //    })
            //    .AddTo(disposables);

            var ray = mainCam.ScreenPointToRay(center);
            if (Physics.Raycast(ray, out hit, 50f))
            {
                spawnMarker.gameObject.SetActive(
                    hit.collider.CompareTag(tagForSpawningAt));
                spawnMarker.position = hit.point;
            }
            else
            {
                spawnMarker.gameObject.SetActive(false);
            }
        }

        public override bool Execute(bool trigger = false)
        {
            disposables.Clear();

            if (spawnMarker.gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(CorSummonBait());
                return true;
            }
            else
            {
                aimCamera.gameObject.SetActive(false);
                return false;
            }
        }

        private IEnumerator CorSummonBait()
        {
            isInEffect.Value = false;
            spawnMarker.gameObject.SetActive(false);
            PlayAnimation();

            SetFXActive(false);
            summon.SetActive(false);
            SetFXActive(true);

            yield return new WaitForSeconds(delayOnActivate);
            isInEffect.Value = true;
            summon.transform.position = spawnMarker.position;
            summon.SetActive(true);

            yield return new WaitForSeconds(delayAimCameraDeactivation);
            aimCamera.gameObject.SetActive(false);
            isInEffect.Value = false;
        }

        #endregion

    }


}