namespace ReGaSLZR.Dare.AI
{

    using Dare.Detector;
    using Dare.Movement;

    using NaughtyAttributes;
    using System.Collections;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public class SeekerEnemyAI : BaseAI<TargetedMovement>
    {

        #region Inspector Variables

        [SerializeField]
        [Required]
        protected CollisionDetector skillUseOnRangeDetector;

        [SerializeField]
        [Required]
        protected CollisionDetector chaseTargetDetector;

        [Space]

        [SerializeField]
        [Required]
        protected CollisionDetector noiseDetector;

        [Space]

        [SerializeField]
        protected Transform target;

        [Space]

        [SerializeField]
        [Range(0f, 75f)]
        protected float wanderRange = 50f;

        [SerializeField]
        [Range(3, 10)]
        protected int wanderPauseMaxDuration = 5;

        #endregion

        protected bool isWandering;

        #region Unity Callbacks

        protected override void OnEnable()
        {
            base.OnEnable();

            //Noise detected! -> target the noise origin
            //NO Noise detected! -> start wandering
            noiseDetector.HasCollision()
                .Where(_ => !chaseTargetDetector.HasCollision().Value
                    && !skillUseOnRangeDetector.HasCollision().Value)
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

            //Skill target met!  -> Stop movement, start continuously using skill!
            //Skill target lost! -> Stop skill use
            skillUseOnRangeDetector.HasCollision()
                .Subscribe(hasAttackTarget => {
                    StopAllCoroutines();

                    if (hasAttackTarget)
                    {
                        movement.OnStop();
                        skillMain.Execute(true);
                    }
                    else 
                    {
                        skillMain.Execute(false);
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
            yield return new WaitForSeconds(Random.Range(1, wanderPauseMaxDuration+1));

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