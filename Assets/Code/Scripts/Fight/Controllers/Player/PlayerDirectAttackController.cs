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
    public class PlayerDirectAttackController : PlayerActionController
    {
        private bool _isTargeting;
        private bool _fighterIsTargetingForDirectAttack;
        private Cell[] _currentMovePath = { };
        private Fighter _fighter;  // Declare the field here

        public PlayerDirectAttackController(Fighter fighter, HexGrid fightGrid, Material highlightMaterial, Material actionableHighlightMaterial, Material inaccessibleHighlightMaterial): 
            base(fighter, fightGrid, highlightMaterial, actionableHighlightMaterial, inaccessibleHighlightMaterial) 
        {
            _fighter = fighter;
        }
        
        
        public void StartTargeting()
        {
            Debug.Log("TARGETING");
            if (_possessedFighter == null)
            {
                Debug.LogError("_possessedFighter is null.");
                return;
            }

            if (_possessedFighter.DirectAttackTargeter == null)
            {
                Debug.LogError("_possessedFighter.DirectAttackTargeter is null.");
                return;
            }

            if (_currentFightGrid == null)
            {
                Debug.LogError("_currentFightGrid is null.");
                return;
            }

            if (_possessedFighter.cell == null)
            {
                Debug.LogError("_possessedFighter.cell is null.");
                return;
            }

            _isTargeting = true;
            HighlightCells(_possessedFighter.DirectAttackTargeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell));
        }

        public override void OnCellClicked(Cell clickedCell)
        {
            Debug.Log("OnCellClicked before try");
            if (!_isTargeting) return;

            try
            {
                Debug.Log("OnCellClicked");
                MakeFighterMove(clickedCell);
                TryTriggerDirectAttack(clickedCell);
                Cell[] targetedCells = _possessedFighter.DirectAttackTargeter.Resolve(_currentFightGrid, clickedCell, _possessedFighter.cell);
                StopTargeting();
                _possessedFighter.UseDirectAttack(targetedCells);
                
            }
            catch (TargeterUnresolvableException ex)
            {
                Debug.Log($"Direct attack not possible: {ex.Message}");
            }
        }

        public override void OnCellHovered(Cell hoveredCell)
        {
            if (!_isTargeting) return;
            if (_isTargeting || hoveredCell == _possessedFighter.cell) return;

            if (_fighterIsTargetingForDirectAttack)
            {
                if(_possessedFighter.DirectAttackTargeter.IsCellInRange(_currentFightGrid, _possessedFighter.cell, hoveredCell))
                {
                    HighlightTargeterCells(_possessedFighter.DirectAttackTargeter, hoveredCell);
                }
            }

            Debug.Log("test pcq ça s'affiche pas");
        }

        public override void OnCellUnhovered(Cell unhoveredCell)
        {
            if (!_isTargeting) return;
            if (_fighterIsTargetingForDirectAttack)
            {
                ResetTargeterCellsMaterial(_possessedFighter.DirectAttackTargeter, unhoveredCell);
            }

            else if (_currentMovePath.Length > 0)
            {
                ResetShorterPathCellsDefaultMaterial();
            }
            Debug.Log("test pcq ça s'affiche pas");
        }

        public override void StopTargeting()
        {Debug.Log("TARGETING");
            if (_possessedFighter == null)
            {
                Debug.LogError("_possessedFighter is null.");
                return;
            }

            if (_possessedFighter.DirectAttackTargeter == null)
            {
                Debug.LogError("_possessedFighter.DirectAttackTargeter is null.");
                return;
            }

            if (_currentFightGrid == null)
            {
                Debug.LogError("_currentFightGrid is null.");
                return;
            }

            if (_possessedFighter.cell == null)
            {
                Debug.LogError("_possessedFighter.cell is null.");
                return;
            }
            _isTargeting = false;
            UnhighlightCells(_possessedFighter.DirectAttackTargeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell));
        }

        private void HighlightCells(Cell[] cells)
        {
            foreach (var cell in cells)
            {
                cell.HighlightController.UpdateCurrentDefaultMaterial(_highlightMaterial);
                cell.HighlightController.Highlight(_highlightMaterial);
            }
            _fighterIsTargetingForDirectAttack = true;
        }
        private void HighlightTargeterCells(TargeterSO targeter, Cell originCell)
        {
            try
            {
                Cell[] targetedCells = targeter.Resolve(_currentFightGrid, originCell, _possessedFighter.cell);
                targetedCells.ToList().ForEach(cell => cell.HighlightController.Highlight(_actionableHighlightMaterial));
            }
            catch (TargeterUnresolvableException)
            {
                Cell[] targetedCells = targeter.GetCellsFromSequence(_currentFightGrid, originCell);
                targetedCells.ToList().ForEach(cell => cell.HighlightController.Highlight(_inaccessibleHighlightMaterial));
            }
        }

        private void UnhighlightCells(Cell[] cells)
        {
            foreach (var cell in cells)
            {
                cell.HighlightController.ResetToInitialMaterial();
            }
        }
        private void ResetTargeterCellsMaterial(TargeterSO targeter, Cell originCell)
        {
            Cell[] targetedCells = targeter.GetCellsFromSequence(_currentFightGrid, originCell);
            targetedCells.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }

        public override void ResetShorterPathCellsDefaultMaterial()
        {
            _currentMovePath.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }
        private void TryTriggerDirectAttack(Cell clickedCell)
        {
            try
            {
                Debug.Log("Trigger");

                Cell[] targetedCells = _possessedFighter.DirectAttackTargeter.Resolve(
                    _currentFightGrid,
                    clickedCell,
                    _possessedFighter.cell
                );
                StopTargeting();
                _isTargeting = true;
                _possessedFighter.UseDirectAttack(targetedCells);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log($"Fighter {_possessedFighter.name} can't use its direct attack from cell {_possessedFighter.cell.name}");
            }
        }

        #region Movement handling

        private void MakeFighterMove(Cell clickedCell)
        {
            Debug.Log("FighterMove");

            Cell[] movePath = FightCellsPathFinding.GetShorterPath(_currentFightGrid, _possessedFighter.cell, clickedCell);
            if (movePath.Length > _possessedFighter.GetMovePoints())
            {
                return;
            }

            ResetShorterPathCellsDefaultMaterial();
            _isTargeting= true;
            _possessedFighter.Move(movePath);
        }

        private void OnFighterMoved(Fighter possessedFighter)
        {
            _isTargeting = false;
        }

        #endregion
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

    }

}