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
    public class PlayerActiveAbilityController : PlayerActionController
    {
        private bool _isTargeting;
        private bool _fighterIsTargetingForActiveAbility;
        private ActiveAbilityToAnimation _currentActiveAbility;
        private Cell[] _currentMovePath = { };

        public PlayerActiveAbilityController(Fighter fighter, HexGrid fightGrid, Material highlightMaterial, Material actionableHighlightMaterial, Material inaccessibleHighlightMaterial)
            : base(fighter, fightGrid, highlightMaterial, actionableHighlightMaterial, inaccessibleHighlightMaterial) { }

        public void StartTargeting(ActiveAbilityToAnimation activeAbility)
        {
            _isTargeting = true;
            _currentActiveAbility = activeAbility;
            HighlightCells(_currentActiveAbility.activeAbility.Targeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell));
        }

        public override void OnCellClicked(Cell clickedCell)
        {
            if (!_isTargeting) return;

            try
            {
                Cell[] targetedCells = _currentActiveAbility.activeAbility.Targeter.Resolve(_currentFightGrid, clickedCell, _possessedFighter.cell);
                StopTargeting();
                _possessedFighter.UseActiveAbility(_currentActiveAbility, targetedCells);
            }
            catch (TargeterUnresolvableException ex)
            {
                Debug.Log($"Active ability not possible: {ex.Message}");
            }
        }

        public override void OnCellHovered(Cell hoveredCell)
        {
            if (!_isTargeting) return;
            if (_isTargeting || hoveredCell == _possessedFighter.cell) return;
            if (_fighterIsTargetingForActiveAbility)
            {
                TargeterSO targeter = _currentActiveAbility.activeAbility.Targeter;
                if (targeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell).Contains(hoveredCell) ||
                    hoveredCell == _possessedFighter.cell)
                {
                    HighlightTargeterCells(_currentActiveAbility.activeAbility.Targeter, hoveredCell);
                }
            }

            else if (_currentMovePath.Length > 0)
            {
                ResetShorterPathCellsDefaultMaterial();
            }

            Debug.Log("test pcq ça s'affiche pas");
        }

        public override void OnCellUnhovered(Cell unhoveredCell)
        {
            if (!_isTargeting) return;
            if (_fighterIsTargetingForActiveAbility)
            {
                ResetTargeterCellsMaterial(_currentActiveAbility.activeAbility.Targeter, unhoveredCell);
            }
            Debug.Log("test pcq ça s'affiche pas");
            
        }

        public override void StopTargeting()
        {
            _isTargeting = false;
            UnhighlightCells(_currentActiveAbility.activeAbility.Targeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell));
        }

        private void HighlightCells(Cell[] cells)
        {
            foreach (var cell in cells)
            {
                cell.HighlightController.UpdateCurrentDefaultMaterial(_highlightMaterial);
                cell.HighlightController.Highlight(_highlightMaterial);
            }
        }

        private void UnhighlightCells(Cell[] cells)
        {
            foreach (var cell in cells)
            {
                cell.HighlightController.ResetToInitialMaterial();
            }
        }
        public override void ResetShorterPathCellsDefaultMaterial()
        {
            _currentMovePath.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
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
        private void ResetTargeterCellsMaterial(TargeterSO targeter, Cell originCell)
        {
            Cell[] targetedCells = targeter.GetCellsFromSequence(_currentFightGrid, originCell);
            targetedCells.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }

    }
}