using FrostfallSaga.Audio;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    [CreateAssetMenu(
        fileName = "InternalAbilityAnimation",
        menuName = "ScriptableObjects/Fight/Abilities/Animations/InternalAbilityAnimation",
        order = 0
    )]
    public class InternalAbilityAnimationSO : AAbilityAnimationSO
    {
        [field: SerializeField] public string AnimationStateName { get; private set; }
        [field: SerializeField, Range(0f, 99f)] public float AnimationDuration { get; private set; }
        [SerializeReference] public AInternalAbilityAnimationExecutor Executor;
        [field: SerializeField] public AudioClip TriggerSoundFX { get; private set; }

        /// <summary>
        ///     Executes the internal ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            // Registering events
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            // Executing the animation
            FighterCollider fighterWeaponCollider = fighterThatWillExecute.GetWeaponCollider();
            Executor.Execute(
                fighterThatWillExecute,
                abilityTargetCells,
                AnimationStateName,
                AnimationDuration,
                fighterWeaponCollider
            );

            // Setting up the camera
            FindFollowCamera();
            _cameraFollow.FollowAbility(abilityTargetCells[^1].transform);

            // Play trigger the sound effect if any
            if (TriggerSoundFX == null) return;
            AudioManager.Instance.PlayFXSound(TriggerSoundFX, fighterThatWillExecute.transform);
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onCellTouched -= OnCellTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            _cameraFollow.StopFollowing();
            base.OnExecutorAnimationEnded(initiator);
        }
    }
}