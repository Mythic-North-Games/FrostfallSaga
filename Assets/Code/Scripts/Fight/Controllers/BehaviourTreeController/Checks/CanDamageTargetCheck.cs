using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Checks
{
    /// <summary>
    /// Check if the possessed fighter can damage any target between the possible targets and types given in parameters.
    /// </summary>
    public class CanDamageTargetCheck : FBTNode
    {
        private readonly ETarget[] _possibleTargets;
        private readonly ETargetType _targetType;

        public CanDamageTargetCheck(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            ETarget[] possibleTargets,
            ETargetType targetType
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            _possibleTargets = possibleTargets;
            _targetType = targetType;
        }

        public override NodeState Evaluate()
        {
            List<Fighter> damagableTargets = GetDamagableFighters();
            if (damagableTargets.Count == 0)
            {
                return NodeState.FAILURE;
            }

            SetSharedData("damageTarget", GetPreferredTarget(damagableTargets));
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetDamagableFighters()
        {
            List<Fighter> damagableTargets = new();
            bool _possessedFighterTeam = _fighterTeams[_possessedFighter];

            foreach (Fighter fighter in _fighterTeams.Keys)
            {
                if (!CanDamageFighter(fighter))
                {
                    continue;
                }

                if (fighter == _possessedFighter && _possibleTargets.Contains(ETarget.SELF))
                {
                    damagableTargets.Add(fighter);
                    continue;
                }

                bool fighterIsAlly = _fighterTeams[fighter] == _possessedFighterTeam;
                if (fighterIsAlly && _possibleTargets.Contains(ETarget.ALLIES))
                {
                    damagableTargets.Add(fighter);
                    continue;
                }

                if (!fighterIsAlly && _possibleTargets.Contains(ETarget.OPONENTS))
                {
                    damagableTargets.Add(fighter);
                }
            }

            return damagableTargets;
        }

        private bool CanDamageFighter(Fighter fighter)
        {
            return (
                _possessedFighter.CanDirectAttack(_fightGrid, _fighterTeams, fighter) ||
                _possessedFighter.CanUseAtLeastOneActiveAbility(_fightGrid, _fighterTeams, fighter)
            );
        }

        private Fighter GetPreferredTarget(List<Fighter> damagableTargets)
        {
            switch (_targetType)
            {
                case ETargetType.RANDOM:
                    return Randomizer.GetRandomElementFromArray(damagableTargets.ToArray());
                case ETargetType.WEAKEST:
                    return damagableTargets.OrderBy(fighter => fighter.GetHealth()).First();
                case ETargetType.STRONGEST:
                    return damagableTargets.OrderByDescending(fighter => fighter.GetHealth()).First();
                case ETargetType.CLOSEST:
                    return damagableTargets.OrderBy(fighter => CellsPathFinding.GetShorterPath(
                            _fightGrid,
                            _possessedFighter.cell,
                            fighter.cell,
                            includeInaccessibleNeighbors: true,
                            includeHeightInaccessibleNeighbors: true
                        ).Length
                    ).First();
                default:
                    return null;
            }
        }
    }
}