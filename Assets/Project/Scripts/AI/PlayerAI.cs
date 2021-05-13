namespace ReGaSLZR.Dare.AI
{
    using Dare.Movement;
    using Dare.Skill;
    using Dare.Model.Player;

    using NaughtyAttributes;
    using UniRx;
    using UniRx.Triggers;
    using UnityEngine;
    using Zenject;

    public class PlayerAI : BaseAI<PlayerMovement>
    {

        [Inject]
        private IPlayerStatusGetter playerStatusGetter;

        [Inject]
        private IPlayerStatusSetter playerStatusSetter;

        [Inject]
        private IPlayerSkillGetter playerSkillGetter;

        [Inject]
        private IPlayerSkillSetter playerSkillSetter;

        #region Inspector Variables

        [Header("Player AI Config")]

        [SerializeField]
        [Required]
        [Tooltip("A skill with Aim and Release mechanic.")]
        private BaseSkill skillSub;

        #endregion

        private void OnEnable()
        {
            RegisterTerminal();
        }

        private void RegisterTerminal()
        {
            playerStatusGetter.Health()
                .Where(health => (health <= 0))
                .Subscribe(health => {
                    disposableMovement.Clear();
                    disposableSkill.Clear();

                    movement.OnStagger(true);
                })
                .AddTo(disposableTerminal);

            playerStatusGetter.Health()
                .Where(health => health >=
                    playerStatusGetter.GetMaxHealth())
                .Subscribe(_ => {
                    disposableMovement.Clear();
                    disposableSkill.Clear();

                    RegisterMovement();
                    RegisterSkillMain();
                    RegisterSkillSub();

                    movement.ResetAnimator();
                })
                .AddTo(disposableTerminal);
        }

        private void RegisterMovement()
        {
            //Basic Movement: Walk, Run, Crouch
            this.FixedUpdateAsObservable()
                .Where(_ => playerStatusGetter.IsOnGround().Value)
                .Where(_ => !movement.IsStaminaOutPlaying())
                .Select(_ => movement.HasMovementInput())
                .Subscribe(isMoving => {
                    var isRunning = movement.OnMove(isMoving, playerStatusGetter.IsCrouching().Value,
                        (playerStatusGetter.Stamina().Value > 0));
                    playerStatusSetter.SetIsRunning(isRunning);

                    if (isRunning && playerStatusGetter.IsCrouching().Value)
                    {
                        playerStatusSetter.ToggleIsCrouching();
                    }
                })
                .AddTo(disposableMovement);

            this.UpdateAsObservable()
                .Where(_ => movement.HasCrouchInput()
                    && playerStatusGetter.IsOnGround().Value)
                .Subscribe(_ => {
                    playerStatusSetter.ToggleIsCrouching();
                    movement.OnCrouch(playerStatusGetter.IsCrouching().Value);
                })
                .AddTo(disposableMovement);

            //Update on Grounded state
            groundDetector.HasCollision()
                .Subscribe(hasCollision =>
                {
                    playerStatusSetter.SetIsOnGround(hasCollision);
                    movement.OnGround(hasCollision, playerStatusGetter.IsCrouching().Value);
                })
                .AddTo(disposableMovement);

            //Update on Stamina (depletion and refill)
            playerStatusGetter.Stamina()
                .Subscribe(stamina =>
                    movement.OnUpdateStamina(stamina, playerStatusGetter.GetCriticalStamina()))
                .AddTo(disposableMovement);

            //On Stagger
            playerStatusGetter.OnTakeDamage()
                .Where(hasTakenDamage => hasTakenDamage)
                .Subscribe(_ => movement.OnStagger(false))
                .AddTo(disposableMovement);
        }

        private void RegisterSkillSub()
        {
            //Upon holding down the skill key, aim...
            this.UpdateAsObservable()
                .Where(_ => Input.GetButton(skillSub.GetSkillButton()))
                .Where(_ => playerStatusGetter.Stamina().Value >=
                    playerSkillGetter.GetStaminaCostSummonBait())
                .Subscribe(_ => skillSub.Aim())
                .AddTo(disposableSkill);

            //Upon releasing the skill key, execute
            this.UpdateAsObservable()
               .Where(_ => Input.GetButtonUp(skillSub.GetSkillButton()))
               .Where(_ => playerStatusGetter.Stamina().Value >=
                    playerSkillGetter.GetStaminaCostSummonBait())
               .Subscribe(_ =>
               {
                   var isComplete = skillSub.Execute();
                   if (isComplete)
                   {
                       playerStatusSetter.CostStamina(
                            playerSkillGetter.GetStaminaCostSummonBait());
                   }
               })
               .AddTo(disposableSkill);
        }

        private void RegisterSkillMain()
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetButtonDown(skillMain.GetSkillButton()))
                .Subscribe(_ => playerSkillSetter.ToggleShielding())
                .AddTo(disposableSkill);

            playerSkillGetter.IsShielding()
                .Subscribe(isShielding => skillMain.Execute(isShielding))
                .AddTo(disposableSkill);
        }

    }

}