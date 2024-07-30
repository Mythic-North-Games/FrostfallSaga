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

        public PlayerDirectAttackController(Fighter fighter, HexGrid fightGrid, Material highlightMaterial, Material actionableHighlightMaterial, Material inaccessibleHighlightMaterial): 
            base(fighter, fightGrid, highlightMaterial, actionableHighlightMaterial, inaccessibleHighlightMaterial) { }
        
        
        public void StartTargeting()
        {
            _isTargeting = true;
            HighlightCells(_possessedFighter.DirectAttackTargeter.GetAllCellsAvailableForTargeting(_currentFightGrid, _possessedFighter.cell));
        }

        public override void OnCellClicked(Cell clickedCell)
        {
            if (!_isTargeting) return;

            try
            {
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
            // Add specific hover logic if needed
        }

        public override void OnCellUnhovered(Cell unhoveredCell)
        {
            if (!_isTargeting) return;
            // Add specific unhover logic if needed
        }

        public override void StopTargeting()
        {
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
        }

        private void UnhighlightCells(Cell[] cells)
        {
            foreach (var cell in cells)
            {
                cell.HighlightController.ResetToInitialMaterial();
            }
        }

    }

}