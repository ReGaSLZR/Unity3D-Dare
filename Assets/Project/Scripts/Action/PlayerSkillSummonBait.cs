namespace ReGaSLZR.Dare.Action
{

    using Dare.Model.Player;

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

        [Header("Bait Config")]

        [SerializeField]
        [Required]
        private GameObject bait;

        [SerializeField]
        [Required]
        private Transform spawnPoint;

        #endregion

        private Transform mainCam;

        #region Overriden Methods

        private void Awake()
        {
            mainCam = Camera.main.transform;
        }

        protected override void OnReady()
        {
            this.UpdateAsObservable()
               .Where(_ => Input.GetButtonDown(skillButton))
               .Where(_ => playerStatusGetter.Stamina().Value >= 
                    playerSkillGetter.GetStaminaCostSummonBait())
               .Subscribe(_ => {
                   StopAllCoroutines();
                   StartCoroutine(CorSummonBait());
               })
               .AddTo(disposables);

            bait.SetActive(false);
        }

        #endregion

        #region Class Implementation

        private IEnumerator CorSummonBait()
        {
            animator.ResetTrigger(animTrigger);
            animator.SetTrigger(animTrigger);

            SetFXActive(false);
            bait.SetActive(false);
            SetFXActive(true);

            yield return new WaitForSeconds(delayOnActivate);

            playerStatusSetter.CostStamina(
                playerSkillGetter.GetStaminaCostSummonBait());

            bait.transform.position = spawnPoint.position;
            bait.SetActive(true);
        }

        #endregion

    }


}