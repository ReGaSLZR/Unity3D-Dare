namespace ReGaSLZR.Dare.AI
{

    using Dare.Movement;

    using NaughtyAttributes;
    using System.Collections;
    using UnityEngine;

    public class NoiseBaitAI : BaseAI<BaseMovement>
    {

        #region Inspector Variables

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
        private GameObject baitFXEntrance;

        [SerializeField]
        [Required]
        private GameObject baitFXExit;

        [Header("Animation Config")]

        [SerializeField]
        [Required]
        private Animation animEntrance;

        #endregion

        #region Unity Callbacks

        protected override void OnEnable()
        {
            base.OnEnable();

            ConfigActiveElements(true);
            StopAllCoroutines();
            StartCoroutine(CorLifetimeCountdown());

            skillMain.Execute();
        }

        #endregion

        private void ConfigActiveElements(bool isEntering)
        {
            baitFXEntrance.SetActive(isEntering);
            baitFXExit.SetActive(!isEntering);

            if (isEntering)
            {
                animEntrance.Play();
            }
            else 
            {
                animEntrance.Stop();
            }
        }

        private IEnumerator CorLifetimeCountdown()
        {
            yield return new WaitForSeconds(durationLifetime + durationEntrance);
            ConfigActiveElements(false);
            yield return new WaitForSeconds(durationExit);
            stats.DisableParentGameObject();
        }

    }

}