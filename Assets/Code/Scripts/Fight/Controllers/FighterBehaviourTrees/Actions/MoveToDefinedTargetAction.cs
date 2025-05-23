using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils.DataStructures.BehaviourTree;
using UnityEngine;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees.Actions
{
    /// <summary>
    /// Move to the closest target between the ones given.
    /// </summary>
    public class MoveToDefinedTargetAction : FBTNode
    {
        public MoveToDefinedTargetAction(
            Fighter possessedFighter,
            FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fighterTeams
        ) : base(possessedFighter, fightGrid, fighterTeams)
        {
        }

        public override NodeState Evaluate()
        {
            Fighter fightersToMoveToward = GetSharedData(FBTNode.TARGET_SHARED_DATA_KEY) as Fighter;

            if (fightersToMoveToward == null)
            {
                Debug.LogError("A target has to be defined before using the MoveToDefinedTargetAction.");
                return NodeState.FAILURE;
            }

            FightCell[] pathToClosestFighter = GetShorterPathToFighter(fightersToMoveToward);

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

        private FightCell[] GetShorterPathToFighter(Fighter fighterToMoveTowards)
        {
            return Array.ConvertAll(
                CellsPathFinding.GetShorterPath(
                    _fightGrid,
                    _possessedFighter.cell,
                    fighterToMoveTowards.cell,
                    includeInaccessibleNeighbors: false,
                    includeHeightInaccessibleNeighbors: false,
                    includeOccupiedNeighbors: false,
                    checkLastCell: false
                ), cell => (FightCell)cell
            );
        }

        private void OnPossessedFighterMoved(Fighter fighter)
        {
            SetSharedData(FBTNode.ACTION_RUNNING_SHARED_DATA_KEY, false);
            _possessedFighter.onFighterMoved -= OnPossessedFighterMoved;
        }
    }
}