namespace ReGaSLZR.Dare.Action
{

    using Dare.Model.Player;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using UnityEngine.Playables;
    using Zenject;

    public class PlayerSkillShield : BaseSkill
    {

        [Inject]
        private IPlayerSkillGetter playerSkillGetter;

        [Inject]
        private IPlayerSkillSetter playerSkillSetter;

        #region Inspector Variables

        [Header("Shield Config")]

        [SerializeField]
        [Required]
        private Renderer shieldRenderer;

        [SerializeField]
        [Required]
        private PlayableDirector shieldOnEntrance;

        [SerializeField]
        [Required]
        private PlayableDirector shieldOnExit;

        #endregion

        #region Overriden Methods

        protected override void OnReady()
        {
            playerSkillGetter.IsShielding()
                .Subscribe(isShielding => {
                    if (!isShielding && 
                        !shieldRenderer.gameObject.activeInHierarchy)
                    {
                        return;
                    }
                    else 
                    {
                        StopAllCoroutines();
                        StartCoroutine(CorUpdateShield(isShielding));
                    }
                })
                .AddTo(disposables);

            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown(skillButton))
                .Subscribe(_ => playerSkillSetter.ToggleShielding())
                .AddTo(disposables);
        }

        #endregion

        #region Class Implementation

        private IEnumerator CorUpdateShield(bool isShielding)
        {
            SetFXActive(false);
            shieldRenderer.gameObject.SetActive(true);

            if (isShielding)
            {        
                animator.SetTrigger(animTrigger);
                yield return new WaitForSeconds(delayOnActivate);
                SetFXActive(true);
                shieldOnEntrance.Play();
            }
            else 
            {
                shieldOnExit.Play();
                yield return new WaitForSeconds((float)shieldOnExit.duration);
                shieldRenderer.gameObject.SetActive(false);
            }
        }

        #endregion

    }

}