using UnityEngine;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Utils;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using System.Collections.Generic;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Checks
{
    /// <summary>
    /// Check if the possessed fighter can use an ability with the given effect.
    /// </summary>
    /// <typeparam name="T">The effect type to check usability.</typeparam>
    public class IsDefinedTargetCloseForEffectCheck<T> : FBTNode where T : AEffect
    {
        public IsDefinedTargetCloseForEffectCheck(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            Fighter target = GetSharedData(FBTNode.TARGET_SHARED_DATA_KEY) as Fighter;
            if (target == null)
            {
                Debug.LogError("A target has to be defined before using the IsDefinedTargetCloseForEffectCheck.");
                return NodeState.FAILURE;
            }
            return CanReachTargetWithEffect(target) ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        private bool CanReachTargetWithEffect(Fighter target)
        {
            ListOfTypes<AEffect> effects = new();
            effects.Add<T>();
            return (
                _possessedFighter.CanUseAtLeastOneActiveAbility(_fightGrid, _fighterTeams, target, effects)
            );
        }
    }
}
