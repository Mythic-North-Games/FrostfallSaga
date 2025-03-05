using System;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils.Trees.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions
{
    /// <summary>
    /// Move to the closest target between the ones given.
    /// </summary>
    public class MoveToClosestTargetAction : FBTNode
    {
        private readonly List<ETarget> _possibleTargets;

        public MoveToClosestTargetAction(
            Fighter possessedFighter,
            AHexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            List<ETarget> possibleTargets
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            _possibleTargets = possibleTargets;
        }

        public override NodeState Evaluate()
        {
            List<Fighter> fightersToMoveTowards = GetFightersToMoveTowards();

            // If there are no fighters to move towards, don't move.
            if (fightersToMoveTowards.Count == 0)
            {
                return NodeState.FAILURE;
            }

            FightCell[] pathToClosestFighter = GetShorterPathToClosestFighter(fightersToMoveTowards);

            // If we are already in melee with the closest fighter, don't move.
            if (pathToClosestFighter.Length == 0)
            {
                return NodeState.FAILURE;
            }

            _possessedFighter.onFighterMoved += OnPossessedFighterMoved;
            _possessedFighter.Move(pathToClosestFighter, goUntilAllMovePointsUsed: true);
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, true);
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetFightersToMoveTowards()
        {
            List<Fighter> targetsToMoveTo = new();
            bool _possessedFighterTeam = _fighterTeams[_possessedFighter];

            foreach (Fighter fighter in _fighterTeams.Keys.Where(fighter => fighter != _possessedFighter && fighter.GetHealth() > 0))
            {
                bool fighterIsAlly = _fighterTeams[fighter] == _possessedFighterTeam;
                if (fighterIsAlly && _possibleTargets.Contains(ETarget.ALLIES))
                {
                    targetsToMoveTo.Add(fighter);
                    continue;
                }

                if (!fighterIsAlly && _possibleTargets.Contains(ETarget.OPONNENTS))
                {
                    targetsToMoveTo.Add(fighter);
                }
            }

            return targetsToMoveTo;
        }

        private FightCell[] GetShorterPathToClosestFighter(List<Fighter> fightersToMoveTowards)
        {
            FightCell[] shortestPath = { };
            foreach (Fighter fighter in fightersToMoveTowards)
            {
                FightCell[] path = Array.ConvertAll(
                    CellsPathFinding.GetShorterPath(
                        _fightGrid,
                        _possessedFighter.cell,
                        fighter.cell,
                        includeInaccessibleNeighbors: false,
                        includeHeightInaccessibleNeighbors: false,
                        includeOccupiedNeighbors: false,
                        checkLastCell: false
                    ), cell => (FightCell)cell
                );

                if (shortestPath.Length == 0 || path.Length < shortestPath.Length)
                {
                    shortestPath = path.Take(path.Length - 1).ToArray();
                }
            }
            return shortestPath;
        }

        private void OnPossessedFighterMoved(Fighter fighter)
        {
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, false);
            _possessedFighter.onFighterMoved -= OnPossessedFighterMoved;
        }
    }
}