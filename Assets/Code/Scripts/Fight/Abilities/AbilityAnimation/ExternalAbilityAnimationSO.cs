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
        [field: SerializeField] public AudioClip ProjectileTriggerSoundFX { get; private set; }

        /// <summary>
        ///     Executes the external ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, FightCell[] abilityTargetCells)
        {
            // Registering events
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onCellTouched += OnCellTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;

            // Executing the animation
            Executor.Execute(fighterThatWillExecute, abilityTargetCells, ProjectilePrefab);

            // Setting up the camera
            FindFollowCamera();
            _cameraFollow.FollowAbility(abilityTargetCells[abilityTargetCells.Length / 2].transform);

            // Playing the sound effect if any
            if (ProjectileTriggerSoundFX == null) return;
            AudioManager.Instance.PlayFXSound(ProjectileTriggerSoundFX, fighterThatWillExecute.transform);
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onCellTouched -= OnCellTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
            _cameraFollow.StopFollowing();
        }
    }
}