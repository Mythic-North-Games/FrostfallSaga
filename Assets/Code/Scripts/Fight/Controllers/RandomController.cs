using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Abilities;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    /// Make the fighter do random doable actions.
    /// </summary>
    public class RandomController : AFighterController
    {
        public int maxActionsPerTurn = 4;
        public int timeBetweenActionsInSec = 2;

        private Fighter _possessedFighter;
        private Dictionary<Fighter, bool> _fighterTeams;
        private HexGrid _fightGrid;
        private int _numberOfActionsToDoForTurn;
        private int _numberOfActionsDoneForTurn;

        public void Setup(int maxActionsPerTurn = 4, int timeBetweenActionsInSec = 2)
        {
            this.maxActionsPerTurn = maxActionsPerTurn;
            this.timeBetweenActionsInSec = timeBetweenActionsInSec;
            _possessedFighter = null;
            _fighterTeams = null;
            _fightGrid = null;
            _numberOfActionsToDoForTurn = 0;
            _numberOfActionsDoneForTurn = 0;
        }

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, bool> fighterTeams, HexGrid fightGrid)
        {
            _possessedFighter = fighterToPlay;
            _fighterTeams = fighterTeams;
            _fightGrid = fightGrid;
            BindFighterEventsForTurn(fighterToPlay);

            _numberOfActionsToDoForTurn = Randomizer.GetRandomIntBetween(1, maxActionsPerTurn);
            _numberOfActionsDoneForTurn = 0;
            Debug.Log($"Fighter {fighterToPlay.name} will do {_numberOfActionsToDoForTurn} actions this turn.");

            _possessedFighter.StartCoroutine(DoNextAction());
        }

        private FighterAction GetRandomDoableAction(Fighter fighterThatWillAct, HexGrid fightGrid)
        {
            List<FighterAction> doableActions = new();

            if (fighterThatWillAct.CanMove(fightGrid))
            {
                doableActions.Add(FighterAction.MOVE);
            }
            if (fighterThatWillAct.CanDirectAttack(fightGrid, _fighterTeams))
            {
                doableActions.Add(FighterAction.DIRECT_ATTACK);
            }
            if (fighterThatWillAct.CanUseAtLeastOneActiveAbility(fightGrid, _fighterTeams))
            {
                doableActions.Add(FighterAction.ACTIVE_ABILITY);
            }
            return doableActions[Randomizer.GetRandomIntBetween(0, doableActions.Count)];
        }

        /// <summary>
        /// If fighter can no longer act or he has done its maximum number of actions, end its turn, 
        /// otherwise do a random doable action.
        /// </summary>
        private IEnumerator DoNextAction()
        {
            if (!_possessedFighter.CanAct(_fightGrid, _fighterTeams) || _numberOfActionsDoneForTurn == _numberOfActionsToDoForTurn)
            {
                UnbindFighterEventsForTurn(_possessedFighter);
                onFighterTurnEnded?.Invoke(_possessedFighter);
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenActionsInSec);

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
            Debug.Log($"Fighter {fighter.name} is randomly moving.");
            fighter.Move(GetRandomMovePath(fighter, fightGrid));
            onFighterActionStarted?.Invoke(fighter);
        }

        private void OnFighterMoved(Fighter fighterThatMoved)
        {
            Debug.Log($"Fighter {fighterThatMoved.name} has finished its movement.");
            onFighterActionEnded?.Invoke(fighterThatMoved);
            _possessedFighter.StartCoroutine(DoNextAction());
        }

        private FightCell[] GetRandomMovePath(Fighter fighterThatWillMove, HexGrid fightGrid)
        {
            List<FightCell> randomMovePath = new();
            int numberOfCellsInPath = Randomizer.GetRandomIntBetween(1, fighterThatWillMove.GetMovePoints());

            FightCell currentCellOfPath = fighterThatWillMove.cell;
            for (int i = 0; i < numberOfCellsInPath; i++)
            {
                List<FightCell> currentCellOfPathNeighbors = new(
                    Array.ConvertAll(
                        CellsNeighbors.GetNeighbors(fightGrid, currentCellOfPath), cell => (FightCell)cell
                    )
                );
                currentCellOfPathNeighbors.Remove(fighterThatWillMove.cell);
                currentCellOfPathNeighbors.RemoveAll(cell => randomMovePath.Contains(cell));

                if (currentCellOfPathNeighbors.Count == 0)  // Stop generating path if no cell is available to move.
                {
                    break;
                }

                FightCell neighborCellToAdd = Randomizer.GetRandomElementFromArray(currentCellOfPathNeighbors.ToArray());
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
                FightCell[] targetedCells = fighter.Weapon.AttackTargeter.GetRandomTargetCells(
                    fightGrid, fighter.cell, _fighterTeams
                );
                fighter.MovementController.RotateTowardsCell(targetedCells[0]);
                Debug.Log($"Fighter {fighter.name} is direct attacking.");
                fighter.UseDirectAttack(targetedCells);
                onFighterActionStarted?.Invoke(fighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.LogError($"Fighter {fighter.name} can't use its direct attack. It should not have been triggered.");
            }
        }

        private void OnFighterDirectAttackEnded(Fighter fighterThatDirectAttacked)
        {
            Debug.Log($"Fighter {fighterThatDirectAttacked.name} has direct attacked.");
            onFighterActionEnded?.Invoke(fighterThatDirectAttacked);
            _possessedFighter.StartCoroutine(DoNextAction());
        }
        #endregion

        #region Active ability handling
        private void MakeFighterUseActiveAbility(Fighter fighter, HexGrid fightGrid)
        {
            ActiveAbilitySO activeAbilityToUse = GetRandomUsableActiveAbility(fighter, fightGrid);
            try
            {
                FightCell[] targetedCells = activeAbilityToUse.Targeter.GetRandomTargetCells(
                    fightGrid, fighter.cell, _fighterTeams
                );
                fighter.MovementController.RotateTowardsCell(targetedCells[0]);
                Debug.Log($"Fighter {fighter.name} is using its active ability {activeAbilityToUse.Name}");

                fighter.UseActiveAbility(activeAbilityToUse, targetedCells);
                onFighterActionStarted?.Invoke(fighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.LogError(
                    $"Fighter {fighter.name} can't use the active ability {activeAbilityToUse.Name}. " +
                    "It should not have been triggered."
                );
            }

        }

        private void OnFighterActiveAbilityEnded(Fighter fighter)
        {
            Debug.Log($"Fighter {fighter.name} has finished using its active ability.");
            onFighterActionEnded?.Invoke(fighter);
            _possessedFighter.StartCoroutine(DoNextAction());
        }

        private ActiveAbilitySO GetRandomUsableActiveAbility(Fighter fighter, HexGrid fightGrid)
        {
            List<ActiveAbilitySO> usableActiveAbilities = new();
            fighter.ActiveAbilities.ToList()
                .FindAll(activeAbility => fighter.CanUseActiveAbility(fightGrid, activeAbility, _fighterTeams))
                .ForEach(activeAbility => usableActiveAbilities.Add(activeAbility));
            return Randomizer.GetRandomElementFromArray(usableActiveAbilities.ToArray());
        }
        #endregion

        #region Suicide handling

        private void OnPossessedFighterDied(Fighter fighterThatDied)
        {
            if (fighterThatDied != _possessedFighter)
            {
                return;
            }
            UnbindFighterEventsForTurn(fighterThatDied);
        }

        #endregion

        #region Possessed fighter events binding

        private void BindFighterEventsForTurn(Fighter fighterToPlay)
        {
            fighterToPlay.onFighterMoved += OnFighterMoved;
            fighterToPlay.onDirectAttackEnded += OnFighterDirectAttackEnded;
            fighterToPlay.onActiveAbilityEnded += OnFighterActiveAbilityEnded;
            fighterToPlay.onFighterDied += OnPossessedFighterDied;
        }

        private void UnbindFighterEventsForTurn(Fighter fighterToPlay)
        {
            fighterToPlay.onFighterMoved -= OnFighterMoved;
            fighterToPlay.onDirectAttackEnded -= OnFighterDirectAttackEnded;
            fighterToPlay.onActiveAbilityEnded -= OnFighterActiveAbilityEnded;
            fighterToPlay.onFighterDied -= OnPossessedFighterDied;
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