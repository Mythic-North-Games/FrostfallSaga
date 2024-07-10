using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Targeters;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    /// Make the fighter do random doable actions.
    /// </summary>
    public class RandomController : AFighterController
    {
        private static readonly System.Random _randomizer = new();
        public int maxActionsPerTurn = 3;

        private Fighter _possessedFighter;
        private HexGrid _fightGrid;
        private int _numberOfActionsToDoForTurn;
        private int _numberOfActionsDoneForTurn;

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, string> fighterTeams, HexGrid fightGrid)
        {
            _possessedFighter = fighterToPlay;
            _fightGrid = fightGrid;
            BindFighterEventsForTurn(fighterToPlay);

            _numberOfActionsToDoForTurn = _randomizer.Next(1, maxActionsPerTurn);
            _numberOfActionsDoneForTurn = 0;
            Debug.Log("Fighter " + fighterToPlay.name + " will do " + _numberOfActionsToDoForTurn + " actions this turn.");

            DoNextAction();
        }

        private FighterAction GetRandomDoableAction(Fighter fighterThatWillAct, HexGrid fightGrid)
        {
            List<FighterAction> doableActions = new();
            if (fighterThatWillAct.CanMove(fightGrid))
            {
                doableActions.Add(FighterAction.MOVE);
            }
            if (fighterThatWillAct.CanDirectAttack(fightGrid))
            {
                doableActions.Add(FighterAction.DIRECT_ATTACK);
            }
            if (fighterThatWillAct.CanUseAtLeastOneActiveAbility(fightGrid))
            {
                doableActions.Add(FighterAction.ACTIVE_ABILITY);
            }
            return doableActions[_randomizer.Next(0, doableActions.Count)];
        }

        /// <summary>
        /// If fighter can no longer act or he has done its maximum number of actions, end its turn, 
        /// otherwise do a random doable action.
        /// </summary>
        private void DoNextAction()
        {
            if (!_possessedFighter.CanAct(_fightGrid) || _numberOfActionsDoneForTurn == _numberOfActionsToDoForTurn)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " has finished its turn.");
                UnbindFighterEventsForTurn(_possessedFighter);
                onFighterTurnEnded?.Invoke(_possessedFighter);
                return;
            }

            switch (GetRandomDoableAction(_possessedFighter, _fightGrid))
            {
                case FighterAction.MOVE:
                    MakeFighterMove(_possessedFighter, _fightGrid);
                    break;
                case FighterAction.DIRECT_ATTACK:
                    MakeFighterDirectAttack(_possessedFighter, _fightGrid);
                    break;
                case FighterAction.ACTIVE_ABILITY:
                    MakeFighterUseActiveAbility(_possessedFighter, _fightGrid);
                    break;
                default:
                    break;
            }

            _numberOfActionsDoneForTurn += 1;
        }

        #region Movement handling
        private void MakeFighterMove(Fighter fighter, HexGrid fightGrid)
        {
            Debug.Log("Fighter " + fighter.name + " is randomly moving.");
            fighter.Move(GetRandomMovePath(fighter, fightGrid));
            onFighterActionStarted?.Invoke(fighter);
        }

        private void OnFighterMoved(Fighter fighterThatMoved)
        {
            Debug.Log("Fighter " + fighterThatMoved.name + " has finished its movement.");
            onFighterActionEnded?.Invoke(fighterThatMoved);
            DoNextAction();
        }

        private Cell[] GetRandomMovePath(Fighter fighterThatWillMove, HexGrid fightGrid)
        {
            List<Cell> randomMovePath = new();
            int numberOfCellsInPath = _randomizer.Next(1, fighterThatWillMove.GetMovePoints());

            Cell currentCellOfPath = fighterThatWillMove.cell;
            for (int i = 0; i < numberOfCellsInPath; i++)
            {
                List<Cell> currentCellOfPathNeighbors = new(FightCellNeighbors.GetNeighbors(fightGrid, currentCellOfPath));
                currentCellOfPathNeighbors.Remove(fighterThatWillMove.cell);
                currentCellOfPathNeighbors.RemoveAll(cell => randomMovePath.Contains(cell));

                if (currentCellOfPathNeighbors.Count == 0)  // Stop generating path if no cell is available to move.
                {
                    break;
                }

                Cell neighborCellToAdd = currentCellOfPathNeighbors[_randomizer.Next(0, currentCellOfPathNeighbors.Count)];
                randomMovePath.Add(neighborCellToAdd);
                currentCellOfPath = neighborCellToAdd;
            }

            return randomMovePath.ToArray();
        }
        #endregion

        #region Direct attack handling
        private void MakeFighterDirectAttack(Fighter fighter, HexGrid fightGrid)
        {
            try
            {
                Cell[] targetCells = fighter.FighterConfiguration.DirectAttackTargeter.GetRandomTargetCells(fightGrid, fighter.cell);
                Debug.Log("Fighter " + fighter.name + " is direct attacking.");
                fighter.UseDirectAttack(targetCells);
                onFighterActionStarted?.Invoke(fighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.LogError("Fighter " + fighter.name + " can't use its direct attack. It should not have been triggered.");
            }
        }

        private void OnFighterDirectAttackEnded(Fighter fighterThatDirectAttacked)
        {
            Debug.Log("Fighter " + fighterThatDirectAttacked.name + " has direct attacked.");
            onFighterActionEnded?.Invoke(fighterThatDirectAttacked);
            DoNextAction();
        }
        #endregion

        #region Active ability handling
        private void MakeFighterUseActiveAbility(Fighter fighter, HexGrid fightGrid)
        {
            ActiveAbilityToAnimation activeAbilityToUse = GetRandomUsableActiveAbility(fighter, fightGrid);
            try
            {
                Cell[] targetCells = activeAbilityToUse.activeAbility.Targeter.GetRandomTargetCells(fightGrid, fighter.cell);

                Debug.Log("Fighter " + fighter.name + " is using its active ability " + activeAbilityToUse.activeAbility.Name);

                fighter.UseActiveAbility(activeAbilityToUse, targetCells);
                onFighterActionStarted?.Invoke(fighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.LogError(
                    "Fighter " + fighter.name + " can't use the active ability " +
                    activeAbilityToUse.activeAbility.Name + ". It should not have been triggered."
                );
            }

        }

        private void OnFighterActiveAbilityEnded(Fighter fighter)
        {
            Debug.Log("Fighter " + fighter.name + " has finished using its active ability.");
            onFighterActionEnded?.Invoke(fighter);
            DoNextAction();
        }

        private ActiveAbilityToAnimation GetRandomUsableActiveAbility(Fighter fighter, HexGrid fightGrid)
        {
            List<ActiveAbilityToAnimation> usableActiveAbilities = new();
            fighter.FighterConfiguration.AvailableActiveAbilities.ToList().ForEach(
                (activeAbilityToAnimation) =>
                {
                    if (fighter.CanUseActiveAbility(fightGrid, activeAbilityToAnimation.activeAbility))
                    {
                        usableActiveAbilities.Add(activeAbilityToAnimation);
                    }
                }
            );
            return usableActiveAbilities[_randomizer.Next(0, usableActiveAbilities.Count)];
        }
        #endregion

        #region Possessed fighter events binding

        private void BindFighterEventsForTurn(Fighter fighterToPlay)
        {
            fighterToPlay.onFighterMoved += OnFighterMoved;
            fighterToPlay.onFighterDirectAttackEnded += OnFighterDirectAttackEnded;
            fighterToPlay.onFighterActiveAbilityEnded += OnFighterActiveAbilityEnded;
        }

        private void UnbindFighterEventsForTurn(Fighter fighterToPlay)
        {
            fighterToPlay.onFighterMoved -= OnFighterMoved;
            fighterToPlay.onFighterDirectAttackEnded -= OnFighterDirectAttackEnded;
            fighterToPlay.onFighterActiveAbilityEnded -= OnFighterActiveAbilityEnded;
        }

        #endregion

        private enum FighterAction
        {
            MOVE = 0,
            DIRECT_ATTACK = 1,
            ACTIVE_ABILITY = 2,
        }
    }
}