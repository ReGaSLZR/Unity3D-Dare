namespace ReGaSLZR.Dare.Movement
{

    using NaughtyAttributes;
    using UnityEngine;
    using UnityEngine.AI;


    public class TargetedMovement : BaseMovement
    {

        #region Inspector Variables

        [Foldout("Components")]
        [SerializeField]
        [Required]
        private NavMeshAgent navMeshAgent;

        #endregion

        #region Class Implementation

        public void OnStop()
        {
            navMeshAgent.isStopped = true;
            animator.SetFloat(animFloatMovementSpeed, 0f);
        }

        public void OnMove(bool isRunning = false)
        {
            navMeshAgent.isStopped = false;
            
            var speed = isRunning ?
                 (speedWalk + speedRunAdditional) : speedWalk;
            animator.SetFloat(animFloatMovementSpeed, speed);
            navMeshAgent.speed = speed;
        }

        public void SetTargetPosition(Vector3 position)
        {
            navMeshAgent.SetDestination(position);
        }

        public bool HasReachedTarget()
        {
            return navMeshAgent.remainingDistance <=
                navMeshAgent.stoppingDistance;
        }

        public Vector3 GetCurrentPosition()
        {
            return navMeshAgent.transform.position;
        }

        #endregion

    }

}