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

        [field: SerializeField]
        [field: Range(0f, 99f)]
        public float AnimationDuration { get; private set; }

        [SerializeReference] public AInternalAbilityAnimationExecutor Executor;

        [field: Header("Trigger sound")]
        [field: SerializeField]
        public AudioClip TriggerSoundFX { get; private set; }

        [field: SerializeField, Range(0f, 1f)] public float TriggerSoundVolume { get; private set; } = 1f;
        [field: SerializeField, Range(0f, 3f)] public float TriggerSoundFadeOutDuration { get; private set; } = 0.7f;

        /// <summary>
        ///     Executes the internal ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            FighterCollider fighterWeaponCollider = fighterThatWillExecute.GetWeaponCollider();
            Executor.Execute(
                fighterThatWillExecute,
                abilityTargetCells,
                AnimationStateName,
                AnimationDuration,
                fighterWeaponCollider
            );
            AudioManager.Instance.PlayFXSound(
                TriggerSoundFX,
                fighterThatWillExecute.transform,
                TriggerSoundVolume,
                TriggerSoundFadeOutDuration
            );
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onCellTouched -= OnCellTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
        }
    }
}