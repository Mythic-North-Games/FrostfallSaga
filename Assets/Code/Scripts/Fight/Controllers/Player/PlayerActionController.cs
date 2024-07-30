using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Controllers
{
	public abstract class PlayerActionController
	{
		protected Fighter _possessedFighter;
		protected HexGrid _currentFightGrid;
		protected Material _highlightMaterial;
		protected Material _actionableHighlightMaterial;
		protected Material _inaccessibleHighlightMaterial;

		protected PlayerActionController(Fighter fighter, HexGrid fightGrid, Material highlightMaterial, Material actionableHighlightMaterial, Material inaccessibleHighlightMaterial)
		{
			_possessedFighter = fighter;
			_currentFightGrid = fightGrid;
			_highlightMaterial = highlightMaterial;
			_actionableHighlightMaterial = actionableHighlightMaterial;
			_inaccessibleHighlightMaterial = inaccessibleHighlightMaterial;
		}

		public abstract void OnCellClicked(Cell clickedCell);
		public abstract void OnCellHovered(Cell hoveredCell);
		public abstract void OnCellUnhovered(Cell unhoveredCell);
		public abstract void StopTargeting();

	}
}