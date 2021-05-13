namespace ReGaSLZR.Dare.Skill
{

    using Dare.Detector;

    using Cinemachine;
    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UnityEngine;

    public class PlayerSkillSummonBait : BaseSkill
    {

        #region Inspector Variables

        [Header("Bait Camera")]

        [SerializeField]
        [Required]
        private CinemachineVirtualCameraBase aimCamera;

        [SerializeField]
        [Range(0f, 5f)]
        private float delayAimCameraDeactivation = 1f;

        [Header("Bait Config")]

        [SerializeField]
        [Required]
        private GameObject bait;

        [SerializeField]
        [Required]
        private CollisionDetector baitSpawnAimer;

        [SerializeField]
        [Required]
        private Transform baitSpawnMarker;

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
            bait.SetActive(false);
            aimCamera.gameObject.SetActive(false);
            baitSpawnMarker.gameObject.SetActive(false);
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
                baitSpawnMarker.gameObject.SetActive(
                    hit.collider.CompareTag(tagForSpawningAt));
                baitSpawnMarker.position = hit.point;
            }
            else
            {
                baitSpawnMarker.gameObject.SetActive(false);
            }
        }

        public override bool Execute(bool trigger = false)
        {
            disposables.Clear();

            if (baitSpawnMarker.gameObject.activeInHierarchy)
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
            baitSpawnMarker.gameObject.SetActive(false);
            animator.ResetTrigger(animTrigger);
            animator.SetTrigger(animTrigger);

            SetFXActive(false);
            bait.SetActive(false);
            SetFXActive(true);

            yield return new WaitForSeconds(delayOnActivate);

            bait.transform.position = baitSpawnMarker.position;
            bait.SetActive(true);

            yield return new WaitForSeconds(delayAimCameraDeactivation);
            aimCamera.gameObject.SetActive(false);
        }

        #endregion

    }


}