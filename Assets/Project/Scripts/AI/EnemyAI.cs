namespace ReGaSLZR.Dare.AI
{

    using Dare.Detector;
    using Dare.Movement;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    /// <summary>
    /// A Noise-seeker type of Enemy. Will use a skill when gets in range.
    /// </summary>
    public class EnemyAI : BaseAI<TargetedMovement>
    {

        #region Inspector Variables

        [SerializeField]
        [Required]
        private CollisionDetector skillUseOnRangeDetector;

        [SerializeField]
        [Required]
        private CollisionDetector chaseTargetDetector;

        [Space]

        [SerializeField]
        [Required]
        private CollisionDetector noiseDetector;

        [Space]

        [SerializeField]
        private Transform target;

        [Space]

        [SerializeField]
        [Range(0f, 75f)]
        private float wanderRange = 50f;

        [SerializeField]
        [Range(3, 10)]
        private int wanderPauseMaxDuration = 5;

        #endregion

        private bool isWandering;

        #region Unity Callbacks

        private void OnEnable()
        {
            //Noise detected! -> target the noise origin
            //NO Noise detected! -> start wandering
            noiseDetector.HasCollision()
                .Subscribe(_ => SetDestination())
                .AddTo(disposableTerminal);

            //Is Wandering to a set position, move to the position
            this.UpdateAsObservable()
                .Where(_ => isWandering)
                .Subscribe(_ => movement.OnMove(false))
                .AddTo(disposableMovement);

            //Upon reaching Wander target destination -> pause movement then Wander again
            Observable.Interval(System.TimeSpan.FromSeconds(1))
                .Where(_ => isWandering && movement.HasReachedTarget())
                .Subscribe(_ => {
                    StopAllCoroutines();
                    StartCoroutine(CorPauseFromWandering());
                })
                .AddTo(disposableMovement);

            //Has detected noise; no current Attack target
            //Move to noise origin; speed depends on whether has Chase target
            this.UpdateAsObservable()
                .Where(_ => noiseDetector.HasCollision().Value
                    && !skillUseOnRangeDetector.HasCollision().Value)
                .Select(_ => chaseTargetDetector.HasCollision().Value)
                .Subscribe(hasChaseTarget => movement.OnMove(hasChaseTarget))
                .AddTo(disposableMovement);

            //Attack target met!  -> Stop movement, start continuously attacking!
            //Attack target lost! -> Stop attacking
            skillUseOnRangeDetector.HasCollision()
                .Subscribe(hasAttackTarget => {
                    if (hasAttackTarget)
                    {
                        movement.OnStop();
                        Debug.Log("Attack Target met! Now attacking...");
                        //TODO continuous attack!
                    }
                    else 
                    { 
                        //TODO stop attack!
                    }
                })
                .AddTo(disposableSkill);
        }

        #endregion

        #region Class Implementation

        private void SetDestination()
        {
            var isNoiseDetected = noiseDetector.HasCollision().Value;
            isWandering = !isNoiseDetected;

            target.position = isNoiseDetected ?
                noiseDetector.CollidedObjectPosition :
                GetWanderTarget();

            movement.SetTargetPosition(target.position);
        }

        private IEnumerator CorPauseFromWandering()
        {
            isWandering = false;
            movement.OnStop();
            yield return new WaitForSeconds(Random.Range(1f, wanderPauseMaxDuration));

            if (!skillUseOnRangeDetector.HasCollision().Value
                && !chaseTargetDetector.HasCollision().Value)
            { 
                SetDestination();
            }
        }

        private Vector3 GetWanderTarget()
        {
            var currentPosition = movement.GetCurrentPosition();

            return new Vector3(
                    Random.Range(-wanderRange, wanderRange) + currentPosition.x,
                    currentPosition.y,
                    Random.Range(-wanderRange, wanderRange) + currentPosition.z);
        }

        #endregion

    }

}