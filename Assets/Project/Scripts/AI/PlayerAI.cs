namespace ReGaSLZR.Dare.AI
{
    using Dare.Model;
    using Dare.Model.Player;
    using Dare.Movement;
    using Dare.Skill;

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
        [Inject]
        private readonly INoiseActionGetter noiseActions;
        [Inject]
        private readonly IStaminaCostsGetter staminaCosts;

        #region Inspector Variables

        [Header("Player AI Config")]

        [SerializeField]
        [Required]
        [Tooltip("A skill with Aim and Release mechanic.")]
        private BaseSkill skillSub;

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterTerminal();
        }

        private void RegisterTerminal()
        {
            playerStatusGetter.Health()
                .Where(health => (health <= 0))
                .Subscribe(health => {
                    DisableNonTerminals();
                    movement.OnStagger(true);
                })
                .AddTo(disposableTerminal);

            playerStatusGetter.Health()
                .Where(health => health >=
                    playerStatusGetter.GetMaxHealth())
                .Subscribe(_ => {
                    DisableNonTerminals();
                    RegisterNonTerminals();
                    movement.ResetAnimator();
                })
                .AddTo(disposableTerminal);
        }

        private void DisableNonTerminals()
        {
            disposableMovement.Clear();
            disposableSkill.Clear();
        }

        private void RegisterNonTerminals()
        {
            RegisterMovement();
            RegisterMovementNoise();
            RegisterSkillMain();
            RegisterSkillSub();
        }

        private void RegisterMovement()
        {
            //Basic Movement: Walk, Run, Crouch
            this.FixedUpdateAsObservable()
                .Where(_ => playerStatusGetter.IsOnGround().Value)
                .Where(_ => !movement.IsStaminaOutPlaying())
                .Select(_ => movement.HasMovementInput())
                .Subscribe(isMoving => {
                    var isRunning = movement.OnMove(isMoving, 
                        playerStatusGetter.IsCrouching().Value,
                        (playerStatusGetter.Stamina().Value > 0));

                    playerStatusSetter.SetIsMoving(isMoving);
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

        private void RegisterMovementNoise()
        {
            //Crouch idle and Standing idle
            playerStatusGetter.IsCrouching()
                .Where(_ => !movement.HasMovementInput())
                .Select(_ => playerStatusGetter.IsCrouching().Value)
                .Subscribe(isCrouching => {
                    playerStatusSetter.SetNoise(
                            !isCrouching,
                            isCrouching ? 
                                noiseActions.CrouchIdle() :
                                noiseActions.StandingIdle(),
                            isCrouching);
                })
                .AddTo(disposableMovement);

            //Crouch walk and Standing walk
            playerStatusGetter.IsMoving()
                .Where(_ => !playerStatusGetter.IsRunning().Value)
                .Select(_ => playerStatusGetter.IsMoving().Value)
                .Subscribe(isMoving =>
                    playerStatusSetter.SetNoise(isMoving,
                        playerStatusGetter.IsCrouching().Value ?
                        noiseActions.CrouchWalk() : noiseActions.Walk()))
                .AddTo(disposableMovement);

            //Running
            playerStatusGetter.IsRunning()
                .Subscribe(isRunning => 
                    playerStatusSetter.SetNoise(isRunning, 
                        noiseActions.Run()))
                .AddTo(disposableMovement);
        }

        private void RegisterSkillSub()
        {
            //Upon holding down the skill key, aim...
            this.UpdateAsObservable()
                .Where(_ => Input.GetButton(skillSub.GetSkillButton()))
                .Where(_ => playerStatusGetter.Stamina().Value >=
                    staminaCosts.SkillSummonBait())
                .Subscribe(_ => skillSub.Aim())
                .AddTo(disposableSkill);

            //Upon releasing the skill key, execute
            this.UpdateAsObservable()
               .Where(_ => Input.GetButtonUp(skillSub.GetSkillButton()))
               .Where(_ => playerStatusGetter.Stamina().Value >=
                    staminaCosts.SkillSummonBait())
               .Subscribe(_ =>
               {
                   var isComplete = skillSub.Execute();
                   if (isComplete)
                   {
                       playerStatusSetter.CostStamina(
                            staminaCosts.SkillSummonBait());
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