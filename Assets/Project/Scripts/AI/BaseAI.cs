namespace ReGaSLZR.Dare.AI
{

    using Dare.Detector;
    using Dare.Movement;
    using Dare.Skill;
    using Dare.Stats;

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

        [SerializeField]
        [Required]
        protected BaseStats stats;

        #endregion

        protected virtual void OnEnable()
        {
            stats.Health()
                .Where(health => health <= 0)
                .Subscribe(health => OnHealthDepletion())
                .AddTo(disposableTerminal);
        }

        protected virtual void OnDisable()
        {
            disposableMovement.Clear();
            disposableTerminal.Clear();
            disposableSkill.Clear();
        }

        protected virtual void OnHealthDepletion()
        {
            movement.OnStagger(true);
            enabled = false;
        }

    }

}