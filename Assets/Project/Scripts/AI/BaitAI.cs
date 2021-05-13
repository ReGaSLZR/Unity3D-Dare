namespace ReGaSLZR.Dare.AI
{

    using Dare.Movement;

    using NaughtyAttributes;
    using System.Collections;
    using UnityEngine;

    public class BaitAI : BaseAI<BaseMovement>
    {

        #region Inspector Variables

        [SerializeField]
        [Required]
        private GameObject parent;

        [SerializeField]
        [Range(1f, 15f)]
        private float durationLifetime = 7f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float durationEntrance = 0.5f;

        [SerializeField]
        [Range(0.1f, 3f)]
        private float durationExit = 0.5f;


        [Header("Bait Objects")]

        [SerializeField]
        [Required]
        private GameObject baitModel;

        [SerializeField]
        [Required]
        private GameObject baitFXEntrance;

        [SerializeField]
        [Required]
        private GameObject baitFXExit;

        [Header("Animation Config")]

        [SerializeField]
        [Required]
        private Animation animEntrace;

        [SerializeField]
        [Required]
        private Animator animator;

        [SerializeField]
        [AnimatorParam("animator")]
        private string animParamIntIdleVar;

        [SerializeField]
        [MinMaxSlider(0, 5)]
        private Vector2 animIdleVarValues;

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            parent.SetActive(true);
            ConfigActiveElements(true);
            StopAllCoroutines();

            int idleVariation = Random.Range(
                (int)animIdleVarValues.x, (int)animIdleVarValues.y + 1);

            animator.SetInteger(animParamIntIdleVar, idleVariation);

            StartCoroutine(CorLifetimeCountdown());
        }

        #endregion

        private void ConfigActiveElements(bool isEntering)
        {
            baitModel.SetActive(isEntering);
            baitFXEntrance.SetActive(isEntering);

            baitFXExit.SetActive(!isEntering);

            if (isEntering)
            {
                animEntrace.Play();
            }
            else 
            {
                animEntrace.Stop();
            }
        }

        private IEnumerator CorLifetimeCountdown()
        {
            yield return new WaitForSeconds(durationLifetime + durationEntrance);
            ConfigActiveElements(false);
            yield return new WaitForSeconds(durationExit);
            parent.SetActive(false);
        }

    }

}