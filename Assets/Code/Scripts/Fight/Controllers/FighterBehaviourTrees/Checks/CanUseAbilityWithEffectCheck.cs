using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Trees.BehaviourTree;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks
{
    /// <summary>
    /// Check if the possessed fighter can use an ability with the given effect.
    /// </summary>
    /// <typeparam name="T">The effect type to check usability.</typeparam>
    public class CanUseAbilityWithEffectCheck<T> : FBTNode where T : AEffect
    {
        public CanUseAbilityWithEffectCheck(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            return CanUseAbilityWithEffect() ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        private bool CanUseAbilityWithEffect()
        {
            ListOfTypes<AEffect> effects = new();
            effects.Add<T>();
            return (
                _possessedFighter.CanUseAtLeastOneActiveAbility(_fightGrid, _fighterTeams, null, effects)
            );
        }
    }
}
