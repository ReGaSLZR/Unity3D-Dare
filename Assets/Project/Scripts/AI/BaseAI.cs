namespace ReGaSLZR.Dare.AI
{

    using Dare.Detector;
    using Dare.Movement;
    using Dare.Skill;

    using NaughtyAttributes;
    using UniRx;
    using UnityEngine;

    public abstract class BaseAI<T> : MonoBehaviour where T : BaseMovement
    {

        protected CompositeDisposable disposableTerminal = new CompositeDisposable();
        protected CompositeDisposable disposableMovement = new CompositeDisposable();
        protected CompositeDisposable disposableSkill = new CompositeDisposable();

        #region Inspector Variables

        [Header("Base AI Config")]

        [SerializeField]
        [Required]
        protected T movement;

        [SerializeField]
        [Required]
        protected CollisionDetector groundDetector;

        [SerializeField]
        [Required]
        protected BaseSkill skillMain;

        #endregion

        protected virtual void OnDisable()
        {
            disposableMovement.Clear();
            disposableTerminal.Clear();
            disposableSkill.Clear();
        }

    }

}