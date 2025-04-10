using FrostfallSaga.Audio;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    [CreateAssetMenu(
        fileName = "ExternalAbilityAnimation",
        menuName = "ScriptableObjects/Fight/Abilities/Animations/ExternalAbilityAnimation",
        order = 0
    )]
    public class ExternalAbilityAnimationSO : AAbilityAnimationSO
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
        [SerializeReference] public AExternalAbilityAnimationExecutor Executor;

        [field: Header("Projectile trigger sound")]
        [field: SerializeField]
        public AudioClip ProjectileTriggerSoundFX { get; private set; }

        [field: SerializeField, Range(0f, 1f)] public float ProjectileTriggerSoundVolume { get; private set; } = 1f;

        [field: SerializeField, Range(0f, 3f)]
        public float ProjectileTriggerSoundFadeOutDuration { get; private set; } = 0.7f;

        /// <summary>
        ///     Executes the external ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;
            AbilityCameraFollow cameraFollow = FindObjectOfType<AbilityCameraFollow>();
            Executor.Execute(fighterThatWillExecute, abilityTargetCells, ProjectilePrefab);
            cameraFollow.FollowAbility(abilityTargetCells[^1].transform);
            AudioManager.Instance.PlayFXSound(
                ProjectileTriggerSoundFX,
                fighterThatWillExecute.transform,
                ProjectileTriggerSoundVolume,
                ProjectileTriggerSoundFadeOutDuration
            );
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onCellTouched -= OnCellTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
            FindObjectOfType<AbilityCameraFollow>().StopFollowing();
        }
    }
}