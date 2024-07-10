using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.UI;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    /// Make the fighter do what the player indicates through the UI.
    /// </summary>
    public class PlayerController : AFighterController
    {
        private readonly FighterActionPanel _actionPanel;
        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;

        private Cell[] _currentMovePath = {};
        private bool _fighterIsActing;
        private bool _fighterIsTargetingForDirectAttack;
        private ActiveAbilityToAnimation _currentActiveAbility;
        private bool _fighterIsTargetingForActiveAbility;

        private readonly Material _cellHighlightMaterial;
        private readonly Material _cellActionableHighlightMaterial;
        private readonly Material _cellInaccessibleHighlightMaterial;

        public PlayerController(
            FighterActionPanel actionPanel,
            Material cellHighlightMaterial,
            Material cellActionableHighlightMaterial,
            Material cellInaccessibleHighlightMaterial
        )
        {
            _actionPanel = actionPanel;
            _cellHighlightMaterial = cellHighlightMaterial;
            _cellActionableHighlightMaterial = cellActionableHighlightMaterial;
            _cellInaccessibleHighlightMaterial = cellInaccessibleHighlightMaterial;
        }

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, string> fighterTeams, HexGrid fightGrid)
        {
            _currentFightGrid = fightGrid;
            _possessedFighter = fighterToPlay;
            BindFighterEventsForTurn(fighterToPlay);
            BindUIEventsForTurn();
            BindCellMouseEventsForTurn(fightGrid);
        }

        private void OnCellClicked(Cell clickedCell)
        {
            if (_fighterIsActing)
            {
                return;
            }

            if (_fighterIsTargetingForDirectAttack && clickedCell != _possessedFighter.cell)
            {
                TryTriggerDirectAttack(clickedCell);
            }
            else if (_fighterIsTargetingForActiveAbility)
            {
                TryTriggerActiveAbility(clickedCell);
            }
            else if (_currentMovePath != null && _currentMovePath.Length > 0 && clickedCell != _possessedFighter.cell)
            {
                MakeFighterMove(clickedCell);
            }
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            if (_fighterIsActing)
            {
                return;
            }

            if (
                _fighterIsTargetingForDirectAttack &&
                hoveredCell != _possessedFighter.cell &&
                _possessedFighter.FighterConfiguration.DirectAttackTargeter.IsCellInRange(_currentFightGrid, _possessedFighter.cell, hoveredCell)
            )
            {
                HighlightTargeterCells(_possessedFighter.FighterConfiguration.DirectAttackTargeter, hoveredCell);
            }
            else if (
                _fighterIsTargetingForActiveAbility &&
                (
                    _currentActiveAbility.activeAbility.Targeter.GetAllCellsAvailableForTargeting(
                        _currentFightGrid,
                        _possessedFighter.cell
                    ).Contains(hoveredCell) ||
                    hoveredCell == _possessedFighter.cell
                )
            )
            {
                HighlightTargeterCells(_currentActiveAbility.activeAbility.Targeter, hoveredCell);
            }
            else if (!_fighterIsTargetingForActiveAbility && !_fighterIsTargetingForDirectAttack && hoveredCell != _possessedFighter.cell)
            {
                _currentMovePath = FightCellsPathFinding.GetShorterPath(_currentFightGrid, _possessedFighter.cell, hoveredCell);
                HighlightShorterPathCells();
            }
        }

        private void OnCellUnhovered(Cell unhoveredCell)
        {
            if (_fighterIsActing)
            {
                return;
            }

            if (_fighterIsTargetingForDirectAttack)
            {
                ResetTargeterCellsMaterial(_possessedFighter.FighterConfiguration.DirectAttackTargeter, unhoveredCell);
            }
            else if (_fighterIsTargetingForActiveAbility)
            {
                ResetTargeterCellsMaterial(_currentActiveAbility.activeAbility.Targeter, unhoveredCell);
            }
            else if (
                !_fighterIsTargetingForActiveAbility &&
                !_fighterIsTargetingForDirectAttack &&
                _currentMovePath != null &&
                _currentMovePath.Length > 0
            )
            {
                ResetShorterPathCellsDefaultMaterial();
            }
        }

        private void EndFighterAction()
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(_possessedFighter);
        }

        #region Movement handling

        private void MakeFighterMove(Cell clickedCell)
        {
            Cell[] movePath = FightCellsPathFinding.GetShorterPath(_currentFightGrid, _possessedFighter.cell, clickedCell);
            if (movePath.Length > _possessedFighter.GetMovePoints())
            {
                return;
            }

            ResetShorterPathCellsDefaultMaterial();
            _fighterIsActing = true;
            _possessedFighter.Move(movePath);
            onFighterActionStarted?.Invoke(_possessedFighter);
        }

        private void OnFighterMoved(Fighter possessedFighter)
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(possessedFighter);
        }

        #endregion

        #region Direct attack handling

        private void OnDirectAttackClicked()
        {
            if (_fighterIsActing)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " is doing something else. Wait for it to finish.");
                return;
            }
            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
                return;
            }
            if (_possessedFighter.FighterConfiguration.DirectAttackActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log("Fighter " + _possessedFighter.name + " does not have enough action points to execute its direct attack.");
                return;
            }

            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
            }
            if (_currentMovePath.Length > 0)
            {
                ResetShorterPathCellsDefaultMaterial();
            }

            Cell[] cellsAvailableForTargeting = _possessedFighter.FighterConfiguration.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial));
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellHighlightMaterial));

            _fighterIsTargetingForDirectAttack = true;
        }

        private void TryTriggerDirectAttack(Cell clickedCell)
        {
            try
            {
                Cell[] targetedCells = _possessedFighter.FighterConfiguration.DirectAttackTargeter.Resolve(
                    _currentFightGrid,
                    clickedCell,
                    _possessedFighter.cell
                );
                StopTargetingForDirectAttack();
                _fighterIsActing = true;
                _possessedFighter.UseDirectAttack(targetedCells);
                onFighterActionStarted?.Invoke(_possessedFighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " can't use its direct attack from cell " + _possessedFighter.cell.name);
            }
        }

        private void OnFighterDirectAttackEnded(Fighter _possessedFighter)
        {
            EndFighterAction();
        }

        private void StopTargetingForDirectAttack()
        {
            _fighterIsTargetingForDirectAttack = false;
            _possessedFighter.FighterConfiguration.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
        }

        #endregion

        #region Active ability handling

        private void OnActiveAbilityClicked(ActiveAbilityToAnimation clickedAbility)
        {
            if (_fighterIsActing)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " is doing something else. Wait for it to finish.");
                return;
            }
            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
                return;
            }
            if (clickedAbility.activeAbility.ActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log("Fighter " + _possessedFighter.name + " does not have enough action points to execute the ability");
                return;
            }

            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
            }
            if (_currentMovePath.Length > 0)
            {
                ResetShorterPathCellsDefaultMaterial();
            }

            _currentActiveAbility = clickedAbility;
            Cell[] cellsAvailableForTargeting = _currentActiveAbility.activeAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial));
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellHighlightMaterial));
            _possessedFighter.cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial);
            _possessedFighter.cell.HighlightController.Highlight(_cellHighlightMaterial);

            _fighterIsTargetingForActiveAbility = true;
        }

        private void TryTriggerActiveAbility(Cell clickedCell)
        {
            try
            {
                Cell[] targetedCells = _currentActiveAbility.activeAbility.Targeter.Resolve(_currentFightGrid, clickedCell, _possessedFighter.cell);
                StopTargetingActiveActiveAbility();
                ResetTargeterCellsMaterial(_currentActiveAbility.activeAbility.Targeter, clickedCell);
                _fighterIsActing = true;
                _possessedFighter.UseActiveAbility(_currentActiveAbility, targetedCells);
                onFighterActionStarted?.Invoke(_possessedFighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " can't use its active ability from cell " + _possessedFighter.cell.name);
            }
        }

        private void OnFighterActiveAbilityEnded(Fighter _possessedFighter)
        {
            EndFighterAction();
        }

        private void StopTargetingActiveActiveAbility()
        {
            _fighterIsTargetingForActiveAbility = false;
            _possessedFighter.cell.HighlightController.ResetToInitialMaterial();
            _currentActiveAbility.activeAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
        }

        #endregion

        #region End turn handling

        private void OnEndTurnClicked()
        {
            if (_fighterIsActing)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " is doing something else. Wait for it to finish.");
                return;
            }
            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
            }
            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
            }
            if (_currentMovePath.Length > 0)
            {
                ResetShorterPathCellsDefaultMaterial();
            }

            EndPossessedFighterTurn();
        }

        private void EndPossessedFighterTurn()
        {
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_currentFightGrid);
            UnbindUIEventsForTurn();
            onFighterTurnEnded?.Invoke(_possessedFighter);
        }

        #endregion

        #region Cells highlighting for player feedback

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (Cell cell in _currentMovePath)
            {
                if (i < _possessedFighter.GetMovePoints())
                {
                    cell.HighlightController.Highlight(_cellHighlightMaterial);
                }
                else
                {
                    cell.HighlightController.Highlight(_cellInaccessibleHighlightMaterial);
                }
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (Cell cell in _currentMovePath)
            {
                cell.HighlightController.ResetToDefaultMaterial();
            }
        }

        private void HighlightTargeterCells(TargeterSO targeter, Cell originCell)
        {
            try
            {
                Cell[] targetedCells = targeter.Resolve(_currentFightGrid, originCell, _possessedFighter.cell);
                targetedCells.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellActionableHighlightMaterial));
            }
            catch (TargeterUnresolvableException)
            {
                Cell[] targetedCells = targeter.GetCellsFromSequence(_currentFightGrid, originCell);
                targetedCells.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellInaccessibleHighlightMaterial));
            }
        }

        private void ResetTargeterCellsMaterial(TargeterSO targeter, Cell originCell)
        {
            Cell[] targetedCells = targeter.GetCellsFromSequence(_currentFightGrid, originCell);
            targetedCells.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }

        #endregion

        #region UI events binding

        private void BindUIEventsForTurn()
        {
            _actionPanel.onDirectAttackClicked += OnDirectAttackClicked;
            _actionPanel.onActiveAbilityClicked += OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked += OnEndTurnClicked;
        }

        private void UnbindUIEventsForTurn()
        {
            _actionPanel.onDirectAttackClicked -= OnDirectAttackClicked;
            _actionPanel.onActiveAbilityClicked -= OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked -= OnEndTurnClicked;
        }

        #endregion

        #region Possessed fighter events binding

        private void BindFighterEventsForTurn(Fighter _possessedFighter)
        {
            _possessedFighter.onFighterMoved += OnFighterMoved;
            _possessedFighter.onFighterDirectAttackEnded += OnFighterDirectAttackEnded;
            _possessedFighter.onFighterActiveAbilityEnded += OnFighterActiveAbilityEnded;
        }

        private void UnbindFighterEventsForTurn()
        {
            _possessedFighter.onFighterMoved -= OnFighterMoved;
            _possessedFighter.onFighterDirectAttackEnded -= OnFighterDirectAttackEnded;
            _possessedFighter.onFighterActiveAbilityEnded -= OnFighterActiveAbilityEnded;
        }

        #endregion

        #region Cells mouse events binding

        private void BindCellMouseEventsForTurn(HexGrid fightGrid)
        {
            foreach (Cell cell in fightGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
            }
        }

        private void UnbindCellMouseEvents(HexGrid fightGrid)
        {
            foreach (Cell cell in fightGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover -= OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover -= OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp -= OnCellClicked;
            }
        }

        #endregion
    }
}