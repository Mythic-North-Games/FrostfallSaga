using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Controllers
{
    public class PlayerController : AFighterController
    {
        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;

        private Cell[] _currentMovePath;
        private bool _fighterIsMoving;
        private Material _cellHighlightMaterial;
        private Material _cellInaccessibleHighlightMaterial;

        public PlayerController(Material cellHighlightMaterial, Material cellInaccessibleHighlightMaterial)
        {
            _cellHighlightMaterial = cellHighlightMaterial;
            _cellInaccessibleHighlightMaterial = cellInaccessibleHighlightMaterial;
        }

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, string> fighterTeams, HexGrid fightGrid)
        {
            _currentFightGrid = fightGrid;
            _possessedFighter = fighterToPlay;
            _possessedFighter.OnFighterMoved += OnFighterMoved;
            BindCellMouseEvents(fightGrid);
        }

        private void OnCellClicked(Cell clickedCell)
        {
            Cell[] movePath = FightCellsPathFinding.GetShorterPath(_currentFightGrid, _possessedFighter.Cell, clickedCell);
            if (movePath.Length > _possessedFighter.GetMovePoints())
            {
                return;
            }

            ResetShorterPathCellsDefaultMaterial();
            _fighterIsMoving = true;
            _possessedFighter.Move(movePath);
        }

        private void OnFighterMoved(Fighter _possessedFighter)
        {
            _fighterIsMoving = false;
        }

        #region Cells highlighting for movement
        private void OnCellHovered(Cell hoveredCell)
        {
            if (_fighterIsMoving)
            {
                return;
            }

            _currentMovePath = FightCellsPathFinding.GetShorterPath(_currentFightGrid, _possessedFighter.cell, hoveredCell);
            HighlightShorterPathCells();
        }

        private void OnCellUnhovered(Cell unhoveredCell)
        {
            if (_fighterIsMoving)
            {
                return;
            }

            ResetShorterPathCellsDefaultMaterial();
        }

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (Cell cell in _currentMovePath)
            {
                if (i < _possessedFighter.GetMovePoints())
                {
                    cell.CellVisual.Highlight(_cellHighlightMaterial);
                }
                else
                {
                    cell.CellVisual.Highlight(_cellInaccessibleHighlightMaterial);
                }
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (Cell cell in _currentMovePath)
            {
                cell.CellVisual.ResetMaterial();
            }
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