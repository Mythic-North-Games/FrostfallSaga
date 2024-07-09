using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.UI;

namespace FrostfallSaga.Fight.Controllers
{
    public class PlayerController : AFighterController
    {
        public Action<Fighter> onFighterActionStarted;
        public Action<Fighter> onFighterActionEnded;
        public Action<Fighter> onFighterTurnEnded;

        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;

        private Cell[] _currentMovePath;
        private bool _fighterIsActing;
        private bool _fighterIsTargetingForDirectAttack;
        private ActiveAbilitySO _currentActiveAbility;
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
            actionPanel.onDirectAttackClicked += OnDirectAttackClicked;
            actionPanel.onActiveAbilityClicked += OnActiveAbilityClicked;
            actionPanel.onEndTurnClicked += OnEndTurnClicked;

            _cellHighlightMaterial = cellHighlightMaterial;
            _cellActionableHighlightMaterial = cellActionableHighlightMaterial;
            _cellInaccessibleHighlightMaterial = cellInaccessibleHighlightMaterial;
        }

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, string> fighterTeams, HexGrid fightGrid)
        {
            _currentFightGrid = fightGrid;
            _possessedFighter = fighterToPlay;
            BindFighterEventsForTurn(fighterToPlay);
            BindCellMouseEvents(fightGrid);
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
            else if (_currentMovePath.Length > 0)
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
            else if (_fighterIsTargetingForActiveAbility)
            {
                if (
                    !_currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                        _currentFightGrid,
                        _possessedFighter.cell
                    ).Contains(hoveredCell)
                )
                {
                    return;
                }
                HighlightTargeterCells(_currentActiveAbility.Targeter, hoveredCell);
            }
            else if (hoveredCell != _possessedFighter.cell)
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
                ResetTargeterCellsMaterial(_currentActiveAbility.Targeter, unhoveredCell);
            }
            else
            {
                ResetShorterPathCellsDefaultMaterial();
            }
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

            Cell[] cellsAvailableForTargeting = _possessedFighter.FighterConfiguration.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial));
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellHighlightMaterial));

            _fighterIsTargetingForDirectAttack = true;
            onFighterActionStarted?.Invoke(_possessedFighter);
        }

        private void TryTriggerDirectAttack(Cell clickedCell)
        {
            Cell[] cellsAvailableForTargeting = _possessedFighter.FighterConfiguration.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());

            try
            {
                _fighterIsActing = true;
                Cell[] targetedCells = _possessedFighter.FighterConfiguration.DirectAttackTargeter.Resolve(
                    _currentFightGrid,
                    clickedCell,
                    _possessedFighter.cell
                );
                _possessedFighter.UseDirectAttack(targetedCells);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " can't use its direct attack from cell " + _possessedFighter.cell.name);
                _fighterIsTargetingForDirectAttack = false;
                EndFighterAction();
            }
        }

        private void OnFighterDirectAttackEnded(Fighter _possessedFighter)
        {
            _fighterIsTargetingForDirectAttack = false;
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

        private void OnActiveAbilityClicked(ActiveAbilitySO clickedAbility)
        {
            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
                return;
            }
            if (clickedAbility.ActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log("Fighter " + _possessedFighter.name + " does not have enough action points to execute the ability");
                return;
            }

            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
            }

            _currentActiveAbility = clickedAbility;
            Cell[] cellsAvailableForTargeting = _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial));
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellHighlightMaterial));

            _fighterIsTargetingForActiveAbility = true;
            onFighterActionStarted?.Invoke(_possessedFighter);
        }

        private void TryTriggerActiveAbility(Cell clickedCell)
        {
            Cell[] cellsAvailableForTargeting = _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            );
            cellsAvailableForTargeting.ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());

            try
            {
                _fighterIsActing = true;
                Cell[] targetedCells = _currentActiveAbility.Targeter.Resolve(_currentFightGrid, clickedCell, _possessedFighter.cell);
                // TODO : Launch active ability when Alexis is done
                OnFighterActiveAbilityEnded(_possessedFighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log("Fighter " + _possessedFighter.name + " can't use its active ability from cell " + _possessedFighter.cell.name);
                _fighterIsTargetingForActiveAbility = false;
                EndFighterAction();
            }
        }

        private void OnFighterActiveAbilityEnded(Fighter _possessedFighter)
        {
            _fighterIsTargetingForActiveAbility = false;
            EndFighterAction();
        }

        private void StopTargetingActiveActiveAbility()
        {
            _fighterIsTargetingForActiveAbility = false;
            _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
            _currentActiveAbility = null;
            return;
        }

        #endregion

        #region End turn handling

        private void OnEndTurnClicked()
        {
            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
            }
            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
            }

            EndPossessedFighterTurn();
        }

        private void EndPossessedFighterTurn()
        {
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_currentFightGrid);
            onFighterTurnEnded?.Invoke(_possessedFighter);
        }

        #endregion
        private void EndFighterAction()
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(_possessedFighter);
        }

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

        #region Possessed fighter events binding

        private void BindFighterEventsForTurn(Fighter _possessedFighter)
        {
            _possessedFighter.OnFighterMoved += OnFighterMoved;
            _possessedFighter.OnFighterDirectAttackEnded += OnFighterDirectAttackEnded;
            // TODO : Active ability binding
        }

        private void UnbindFighterEventsForTurn()
        {
            _possessedFighter.OnFighterMoved -= OnFighterMoved;
            _possessedFighter.OnFighterDirectAttackEnded -= OnFighterDirectAttackEnded;
            // TODO : Active ability unbinding
        }

        #endregion

        #region Cells mouse events binding

        private void BindCellMouseEvents(HexGrid fightGrid)
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