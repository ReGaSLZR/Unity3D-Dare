namespace ReGaSLZR.Dare.Action
{
    using Dare.Model.Player;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using Zenject;

    public class PlayerSkillShield : MonoBehaviour
    {

        #region Private Variables

        [Inject]
        private IPlayerSkillGetter playerSkillGetter;

        [Inject]
        private IPlayerSkillSetter playerSkillSetter;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        #endregion

        #region Inspector Variables

        [SerializeField]
        private string skillButton = string.Empty;

        [SerializeField]
        [Range(0.001f, 5f)]
        private float skillActivationDamping = 0.1f;

        [SerializeField]
        [Required]
        private Animator animator;

        [AnimatorParam("animator")]
        [SerializeField]
        private string animTriggerSkill;

        [SerializeField]
        [Range(0.1f, 5f)]
        private float skillDelayOnActivate = 1f;

        [Space]

        [SerializeField]
        [Required]
        private Renderer shield;

        [SerializeField]
        private string shieldShaderVarName = "_Rim";

        [SerializeField]
        [Range(0f, 1f)]
        private float shieldShaderVarMaxVal = 1f;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            playerSkillGetter.IsShielding()
                .Subscribe(isShielding => {
                    StopAllCoroutines();
                    StartCoroutine(CorUpdateShield(isShielding));
                })
                .AddTo(disposables);

            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown(skillButton))
                .Subscribe(_ => playerSkillSetter.ToggleShielding());
        }

        private void OnDisable()
        {
            disposables.Clear();   
        }

        #endregion

        #region Class Implementation

        private IEnumerator CorUpdateShield(bool isShielding)
        {
            var targetVal = isShielding ? shieldShaderVarMaxVal : 0f;
            var currentVal = shield.material.GetFloat(shieldShaderVarName);

            if (isShielding)
            {
                animator.SetTrigger(animTriggerSkill);
                yield return new WaitForSeconds(skillDelayOnActivate);
            }

            while (currentVal != targetVal)
            {
                var damping = skillActivationDamping * Time.deltaTime;
                currentVal = Mathf.Lerp(currentVal, targetVal, damping);
                yield return new WaitForSeconds(damping);
                shield.material.SetFloat(shieldShaderVarName, currentVal);
            }
        }

        #endregion

    }

}