namespace ReGaSLZR.Dare.Action
{

    using Dare.Model.Player;

    using Cinemachine;
    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using Zenject;

    public class PlayerSkillSummonBait : BaseSkill
    {

        [Inject]
        private IPlayerStatusGetter playerStatusGetter;

        [Inject]
        private IPlayerStatusSetter playerStatusSetter;

        [Inject]
        private IPlayerSkillGetter playerSkillGetter;

        #region Inspector Variables

        [SerializeField]
        [Required]
        private Transform playerParent;

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
        private Transform baitSpawnPoint;

        [SerializeField]
        [Tag]
        private string tagSpawnPoint;

        #endregion

        private Camera mainCam;
        private readonly Vector2 center = new Vector3(Screen.width / 2, Screen.height / 2);
        private RaycastHit hit;
        #region Overriden Methods

        private void Awake()
        {
            mainCam = Camera.main;
        }

        protected override void OnReady()
        {
            //Upon holding down the skill key, aim...
            this.UpdateAsObservable()
                .Where(_ => Input.GetButton(skillButton))
                .Where(_ => playerStatusGetter.Stamina().Value >=
                    playerSkillGetter.GetStaminaCostSummonBait())
                .Subscribe(_ => {
                    aimCamera.gameObject.SetActive(true);

                    var ray = mainCam.ScreenPointToRay(center);
                    if (Physics.Raycast(ray, out hit))
                    {
                        baitSpawnPoint.gameObject.SetActive(
                            hit.collider.CompareTag(tagSpawnPoint));
                        baitSpawnPoint.position = hit.point;
                    }
                    else 
                    {
                        baitSpawnPoint.gameObject.SetActive(false);
                    }
                })
                .AddTo(disposables);

            //Upon releasing the skill key, execute (if there's a spawn point)
            this.UpdateAsObservable()
               .Where(_ => Input.GetButtonUp(skillButton))
               .Where(_ => playerStatusGetter.Stamina().Value >= 
                    playerSkillGetter.GetStaminaCostSummonBait())
               .Subscribe(_ => {
                   if (baitSpawnPoint.gameObject.activeInHierarchy)
                   {
                       StopAllCoroutines();
                       StartCoroutine(CorSummonBait());
                   }
                   else 
                   {
                       aimCamera.gameObject.SetActive(false);
                   }
               })
               .AddTo(disposables);

            bait.SetActive(false);
        }

        private void Start()
        {
            aimCamera.gameObject.SetActive(false);
            baitSpawnPoint.gameObject.SetActive(false);
        }

        #endregion

        #region Class Implementation

        private IEnumerator CorSummonBait()
        {
            baitSpawnPoint.gameObject.SetActive(false);
            animator.ResetTrigger(animTrigger);
            animator.SetTrigger(animTrigger);

            SetFXActive(false);
            bait.SetActive(false);
            SetFXActive(true);

            yield return new WaitForSeconds(delayOnActivate);

            playerStatusSetter.CostStamina(
                playerSkillGetter.GetStaminaCostSummonBait());

            bait.transform.position = baitSpawnPoint.position;
            bait.SetActive(true);

            yield return new WaitForSeconds(delayAimCameraDeactivation);
            aimCamera.gameObject.SetActive(false);
        }

        #endregion

    }


}