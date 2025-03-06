using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    /// <summary>
    /// Used to configure the type of target to select in fighter behaviour tree nodes.
    /// </summary>
    public enum ETargetType
    {
        WEAKEST,
        STRONGEST,
        CLOSEST,
        RANDOM,
    }

    public static class ETargetTypeMethods {

        /// <summary>
        /// Get the prefered target in the given list depending on the target type.
        /// </summary>
        /// <param name="targetType">The target type to choose.</param>
        /// <param name="possessedFighter">The fighter possessed by the behaviour tree.</param>
        /// <param name="fightGrid">The grid on which the fight is happenning.</param>
        /// <param name="possibleTargets">The possible targets that can be chosen.</param>
        /// <returns></returns>
        public static Fighter GetPreferedTargetInList(
            this ETargetType targetType, 
            Fighter possessedFighter,
            FightHexGrid fightGrid,
            List<Fighter> possibleTargets

        ) {
            return targetType switch
            {
                ETargetType.RANDOM => Randomizer.GetRandomElementFromArray(possibleTargets.ToArray()),
                ETargetType.WEAKEST => possibleTargets.OrderBy(fighter => fighter.GetHealth()).First(),
                ETargetType.STRONGEST => possibleTargets.OrderByDescending(fighter => fighter.GetHealth()).First(),
                ETargetType.CLOSEST => possibleTargets.OrderBy(
                    fighter => CellsPathFinding.GetShorterPath(
                        fightGrid,
                        possessedFighter.cell,
                        fighter.cell,
                        includeInaccessibleNeighbors: true,
                        includeHeightInaccessibleNeighbors: true,
                        includeOccupiedNeighbors: true
                    ).Length
                ).First(),
                _ => null
            };
        }
    }
}