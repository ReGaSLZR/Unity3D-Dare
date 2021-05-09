namespace ReGaSLZR.Dare.Action
{

    using Dare.Model.Player;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
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
        private string shieldShaderVarName = "_Rim";

        [SerializeField]
        [Range(0f, 1f)]
        private float shieldShaderVarMaxVal = 1f;

        [SerializeField]
        private Vector3 shieldSizeMin;

        [SerializeField]
        private Vector3 shieldSizeMax;

        #endregion

        #region Overriden Methods

        protected override void OnReady()
        {
            playerSkillGetter.IsShielding()
                .Subscribe(isShielding => {
                    StopAllCoroutines();
                    StartCoroutine(CorUpdateShield(isShielding));
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
            var targetVal = isShielding ? shieldShaderVarMaxVal : 0f;
            var currentVal = shieldRenderer.material.GetFloat(shieldShaderVarName);
            var targetSize = isShielding ? shieldSizeMax : shieldSizeMin;

            if (isShielding)
            {
                SetFXActive(false);
                shieldRenderer.gameObject.SetActive(true);
                animator.SetTrigger(animTrigger);
                yield return new WaitForSeconds(delayOnActivate);
                SetFXActive(true);
            }

            shieldRenderer.gameObject.transform.localScale =
                isShielding ? shieldSizeMin : shieldSizeMax;

            while (currentVal != targetVal)
            {
                var damping = activationDamping * Time.deltaTime;
                currentVal = Mathf.Lerp(currentVal, targetVal, damping);
                yield return new WaitForSeconds(damping);
                shieldRenderer.material.SetFloat(shieldShaderVarName, currentVal);
                shieldRenderer.gameObject.transform.localScale =
                    Vector3.Lerp(shieldRenderer.gameObject.transform.localScale,
                    targetSize, damping);
            }

            if (!isShielding)
            {
                shieldRenderer.gameObject.SetActive(false);
                SetFXActive(false);
            }
        }

        #endregion

    }

}