namespace ReGaSLZR.Dare.AI
{

    using Dare.Detector;
    using Dare.Movement;

    using NaughtyAttributes;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;

    public class EnemyAI : BaseAI<TargetedMovement>
    {

        [SerializeField]
        [Required]
        private CollisionDetector attackTargetDetector;

        [SerializeField]
        [Required]
        private CollisionDetector chaseTargetDetector;

        [Space]

        [SerializeField]
        [Required]
        private Transform target;

        #region Unity Callbacks

        private void OnEnable()
        {
            this.UpdateAsObservable()
                .Where(_ => !chaseTargetDetector.HasCollision().Value &&
                    !attackTargetDetector.HasCollision().Value)
                .Subscribe(_ => {
                    //TODO code search for target AI before OnMove() call
                    movement.SetTargetPosition(target.position);
                    movement.OnMove(false);
                })
                .AddTo(disposableMovement);

            this.UpdateAsObservable()
                .Where(_ => chaseTargetDetector.HasCollision().Value &&
                    !attackTargetDetector.HasCollision().Value)
                .Subscribe(_ => {
                    movement.SetTargetPosition(target.position);
                    movement.OnMove(true);
                })
                .AddTo(disposableMovement);

            attackTargetDetector.HasCollision()
                .Subscribe(hasTarget =>
                {
                    if (hasTarget)
                    {
                        movement.OnStop(); //TODO replace with attack execution
                    }
                    else
                    {
                        //TODO code search for target AI for the position to move to
                        movement.SetTargetPosition(target.position);
                    }
                })
                .AddTo(disposableMovement);

        }

        #endregion

        #region Class Implementation



        #endregion

    }

}