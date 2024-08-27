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
    public class PlayerEndTurnController : AFighterController
    {
        [SerializeField] private FighterActionPanel _actionPanel;
        [SerializeField] private Material _cellHighlightMaterial;
        [SerializeField] private Material _cellActionableHighlightMaterial;
        [SerializeField] private Material _cellInaccessibleHighlightMaterial;

        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;

        private bool _fighterIsActing;
        private Cell[] _currentMovePath = { };
        private PlayerController _playerController;

        private PlayerDirectAttackController _directAttackController;
        private PlayerActiveAbilityController _activeAbilityController;

        public PlayerEndTurnController(PlayerController playerController, PlayerDirectAttackController attackController, PlayerActiveAbilityController abilityController)
        {
            Debug.Log("PLAYER END TURN CONTROLLER");
            _playerController = playerController;
            _directAttackController = attackController;
            _activeAbilityController = abilityController;
        }

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, bool> fighterTeams, HexGrid fightGrid)
        {
            _currentFightGrid = fightGrid;
            _possessedFighter = fighterToPlay;
            _fighterIsActing = false;

            _directAttackController = new PlayerDirectAttackController(_possessedFighter, _currentFightGrid, _cellHighlightMaterial, _cellActionableHighlightMaterial, _cellInaccessibleHighlightMaterial);
            _activeAbilityController = new PlayerActiveAbilityController(_possessedFighter, _currentFightGrid, _cellHighlightMaterial, _cellActionableHighlightMaterial, _cellInaccessibleHighlightMaterial);

            BindFighterEventsForTurn(_possessedFighter);
            BindUIEventsForTurn();
            BindCellMouseEventsForTurn(_currentFightGrid);
        }

        public void EndTurn()
        {
            if (_fighterIsActing)
            {
                Debug.Log($"Fighter {_possessedFighter.name} is doing something else. Wait for it to finish.");
                return;
            }

            _directAttackController.StopTargeting();
            _activeAbilityController.StopTargeting();

            if (_currentMovePath?.Length > 0)
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

        private void UnbindFighterEventsForTurn()
        {
            _possessedFighter.onFighterMoved -= OnFighterMoved;
            _possessedFighter.onFighterDirectAttackEnded -= OnFighterDirectAttackEnded;
            _possessedFighter.onFighterActiveAbilityEnded -= OnFighterActiveAbilityEnded;
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

        private void UnbindUIEventsForTurn()
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
            }

            _actionPanel.onDirectAttackClicked -= _directAttackController.StartTargeting;
            _actionPanel.onActiveAbilityClicked -= _activeAbilityController.StartTargeting;
            _actionPanel.onEndTurnClicked -= EndTurn;
        }

        private void OnFighterMoved(Fighter possessedFighter)
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(possessedFighter);
        }

        private void OnFighterDirectAttackEnded(Fighter possessedFighter)
        {
            EndFighterAction();
        }

        private void EndFighterAction()
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(_possessedFighter);
        }

        private void OnFighterActiveAbilityEnded(Fighter possessedFighter)
        {
            EndFighterAction();
        }

        private void BindFighterEventsForTurn(Fighter possessedFighter)
        {
            possessedFighter.onFighterMoved += OnFighterMoved;
            possessedFighter.onFighterDirectAttackEnded += OnFighterDirectAttackEnded;
            possessedFighter.onFighterActiveAbilityEnded += OnFighterActiveAbilityEnded;
        }

        private void BindUIEventsForTurn()
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
            }

            _actionPanel.onDirectAttackClicked += _directAttackController.StartTargeting;
            _actionPanel.onActiveAbilityClicked += _activeAbilityController.StartTargeting;
            _actionPanel.onEndTurnClicked += EndTurn;
        }

        private void BindCellMouseEventsForTurn(HexGrid fightGrid)
        {
            foreach (Cell cell in fightGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
            }
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            if (_fighterIsActing || hoveredCell == _possessedFighter.cell) return;

            _directAttackController.OnCellHovered(hoveredCell);
            _activeAbilityController.OnCellHovered(hoveredCell);
        }

        private void OnCellUnhovered(Cell unhoveredCell)
        {
            if (_fighterIsActing || unhoveredCell == _possessedFighter.cell) return;

            _directAttackController.OnCellUnhovered(unhoveredCell);
            _activeAbilityController.OnCellUnhovered(unhoveredCell);
        }

        private void OnCellClicked(Cell clickedCell)
        {
            if (_fighterIsActing || clickedCell == _possessedFighter.cell) return;

            _directAttackController.OnCellClicked(clickedCell);
            _activeAbilityController.OnCellClicked(clickedCell);
        }

        public void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (var cell in _currentMovePath)
            {
                cell.HighlightController.ResetToDefaultMaterial();
            }
        }
    }
}
