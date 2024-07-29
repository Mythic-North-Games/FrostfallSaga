using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid.Cells;

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
        [field: SerializeField] public AExternalAbilityAnimationExecutorSO Executor { get; private set; }

        /// <summary>
        /// Executes the external ability animation as configured.
        /// </summary>
        public override void Execute(Fighter fighterThatWillExecute, Cell[] abilityTargetCells)
        {
            Executor.onFighterTouched += OnFighterTouched;
            Executor.onAnimationEnded += OnExecutorAnimationEnded;
            
            Executor.Execute(fighterThatWillExecute, abilityTargetCells, ProjectilePrefab);
        }

        protected override void OnExecutorAnimationEnded(Fighter initiator)
        {
            Executor.onFighterTouched -= OnFighterTouched;
            Executor.onAnimationEnded -= OnExecutorAnimationEnded;
            base.OnExecutorAnimationEnded(initiator);
        }
    }
}