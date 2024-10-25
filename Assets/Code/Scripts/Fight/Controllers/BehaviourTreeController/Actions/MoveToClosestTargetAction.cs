using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.BehaviourTree;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.Controllers.BehaviourTreeController.Actions
{
    /// <summary>
    /// Move to the closest target between the ones given.
    /// </summary>
    public class MoveToClosestTargetAction : FBTNode
    {
        private ETarget[] _possibleTargets;

        public MoveToClosestTargetAction(
            Fighter possessedFighter,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams,
            ETarget[] possibleTargets
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
            _possibleTargets = possibleTargets;
        }

        public override NodeState Evaluate()
        {
            List<Fighter> fightersToMoveTowards = GetFightersToMoveTowards();
            if (fightersToMoveTowards.Count == 0)
            {
                return NodeState.FAILURE;
            }
            _possessedFighter.Move(GetClosestFighterPath(fightersToMoveTowards));
            return NodeState.SUCCESS;
        }

        private List<Fighter> GetFightersToMoveTowards()
        {
            List<Fighter> targetsToMoveTo = new();
            bool _possessedFighterTeam = _fighterTeams[_possessedFighter];

            foreach (Fighter fighter in _fighterTeams.Keys.Where(fighter => fighter != _possessedFighter))
            {
                bool fighterIsAlly = _fighterTeams[fighter] == _possessedFighterTeam;
                if (fighterIsAlly && _possibleTargets.Contains(ETarget.ALLIES))
                {
                    targetsToMoveTo.Add(fighter);
                    continue;
                }

                if (!fighterIsAlly && _possibleTargets.Contains(ETarget.OPONENTS))
                {
                    targetsToMoveTo.Add(fighter);
                }
            }

            return targetsToMoveTo;
        }

        private Cell[] GetClosestFighterPath(List<Fighter> fightersToMoveTowards)
        {
            Cell[] shortestPath = {};
            foreach (Fighter fighter in fightersToMoveTowards)
            {
                Cell[] path = CellsPathFinding.GetShorterPath(
                    _fightGrid,
                    _possessedFighter.cell,
                    fighter.cell,
                    includeInaccessibleNeighbors: true,
                    includeHeightInaccessibleNeighbors: true
                );

                if (shortestPath.Length == 0 || path.Length < shortestPath.Length)
                {
                    shortestPath = path;
                }
            }
            return shortestPath;
        }
    }
}