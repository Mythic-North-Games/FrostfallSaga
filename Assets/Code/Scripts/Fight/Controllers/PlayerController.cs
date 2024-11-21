using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Abilities;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    /// Make the fighter do what the player indicates through the UI.
    /// </summary>
    public class PlayerController : AFighterController
    {
        private FighterActionPanelController _actionPanel;
        private Material _cellHighlightMaterial;
        private Material _cellActionableHighlightMaterial;
        private Material _cellInaccessibleHighlightMaterial;

        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;
        private Dictionary<Fighter, bool> _fighterTeams;

        private FightCell[] _currentMovePath = { };
        private bool _fighterIsActing;
        private bool _fighterIsTargetingForDirectAttack;
        private ActiveAbilitySO _currentActiveAbility;
        private bool _fighterIsTargetingForActiveAbility;

        public void Setup(
            FighterActionPanelController actionPanel,
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

        public override void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, bool> fighterTeams, HexGrid fightGrid)
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
                return;
            }
            if (_cellHighlightMaterial == null)
            {
                Debug.LogError("Player controller has no cell highlight material to work with.");
                return;
            }
            if (_cellActionableHighlightMaterial == null)
            {
                Debug.LogError("Player controller has no cell highlight material to work with.");
                return;
            }
            if (_cellInaccessibleHighlightMaterial == null)
            {
                Debug.LogError("Player controller has no cell highlight material to work with.");
                return;
            }

            _currentFightGrid = fightGrid;
            _possessedFighter = fighterToPlay;
            _fighterTeams = fighterTeams;
            _fighterIsActing = false;
            _fighterIsTargetingForActiveAbility = false;
            _fighterIsTargetingForDirectAttack = false;

            BindPossessedFighterEventsForTurn(fighterToPlay);
            BindFightersMouseEvents(fighterTeams.Keys.ToList());
            BindUIEventsForTurn();
            BindCellMouseEventsForTurn(fightGrid);
        }

        private void OnCellClicked(Cell clickedCell)
        {
            FightCell clickedFightCell = (FightCell)clickedCell;

            if (_fighterIsActing)
            {
                return;
            }

            if (_fighterIsTargetingForDirectAttack && clickedFightCell != _possessedFighter.cell)
            {
                TryTriggerDirectAttack(clickedFightCell);
            }
            else if (_fighterIsTargetingForActiveAbility)
            {
                TryTriggerActiveAbility(clickedFightCell);
            }
            else if (
                _currentMovePath != null &&
                _currentMovePath.Length > 0 && clickedFightCell != _possessedFighter.cell
            )
            {
                MakeFighterMove(clickedFightCell);
            }
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            FightCell hoveredFightCell = (FightCell)hoveredCell;

            if (_fighterIsActing)
            {
                return;
            }

            if (
                _fighterIsTargetingForDirectAttack &&
                hoveredFightCell != _possessedFighter.cell &&
                _possessedFighter.DirectAttackTargeter.IsCellTargetable(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    hoveredFightCell,
                    _fighterTeams
                )
            )
            {
                HighlightTargeterCells(_possessedFighter.DirectAttackTargeter, hoveredFightCell);
            }
            else if (
                _fighterIsTargetingForActiveAbility &&
                (
                    _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                        _currentFightGrid,
                        _possessedFighter.cell,
                        _fighterTeams,
                        _currentActiveAbility.CellAlterations
                    ).Contains(hoveredFightCell)
                )
            )
            {
                HighlightTargeterCells(_currentActiveAbility.Targeter, hoveredFightCell);
            }
            else if (
                !_fighterIsTargetingForActiveAbility &&
                !_fighterIsTargetingForDirectAttack && 
                hoveredFightCell != _possessedFighter.cell
            )
            {
                _currentMovePath = Array.ConvertAll(
                    CellsPathFinding.GetShorterPath(
                        _currentFightGrid,
                        _possessedFighter.cell,
                        hoveredFightCell,
                        includeOccupiedNeighbors: false
                    ), cell => (FightCell)cell
                );
                HighlightShorterPathCells();
            }

            if (_fighterIsTargetingForActiveAbility || _fighterIsTargetingForDirectAttack)
            {
                _possessedFighter.MovementController.RotateTowardsCell(hoveredCell);
            }
        }

        private void OnCellUnhovered(Cell unhoveredCell)
        {
            FightCell unhoveredFightCell = (FightCell)unhoveredCell;

            if (_fighterIsActing)
            {
                return;
            }

            if (_fighterIsTargetingForDirectAttack)
            {
                ResetTargeterCellsMaterial(_possessedFighter.DirectAttackTargeter, unhoveredFightCell);
            }
            else if (_fighterIsTargetingForActiveAbility)
            {
                ResetTargeterCellsMaterial(_currentActiveAbility.Targeter, unhoveredFightCell);
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

        private void OnFighterHovered(Fighter hoveredFighter)
        {
            OnCellHovered(hoveredFighter.cell);
        }

        private void OnFighterUnhovered(Fighter unhoveredFighter)
        {
            OnCellUnhovered(unhoveredFighter.cell);
        }

        private void OnFighterClicked(Fighter clickedFighter)
        {
            OnCellClicked(clickedFighter.cell);
        }

        private void EndFighterAction()
        {
            _fighterIsActing = false;
            onFighterActionEnded?.Invoke(_possessedFighter);
        }

        #region Movement handling

        private void MakeFighterMove(FightCell destinationCell)
        {
            FightCell[] movePath = Array.ConvertAll(
                CellsPathFinding.GetShorterPath(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    destinationCell
                ), cell => (FightCell)cell
            );
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
            if (_possessedFighter.DirectAttackActionPointsCost > _possessedFighter.GetActionPoints())
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

            FightCell[] cellsAvailableForTargeting = _possessedFighter.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell,
                _fighterTeams
            );
            cellsAvailableForTargeting.ToList().ForEach(
                cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial)
            );
            cellsAvailableForTargeting.ToList().ForEach(
                cell => cell.HighlightController.Highlight(_cellHighlightMaterial)
            );

            _fighterIsTargetingForDirectAttack = true;
        }

        private void TryTriggerDirectAttack(FightCell clickedCell)
        {
            try
            {
                FightCell[] targetedCells = _possessedFighter.DirectAttackTargeter.Resolve(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    clickedCell,
                    _fighterTeams
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
            _possessedFighter.DirectAttackTargeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell,
                _fighterTeams
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
        }

        #endregion

        #region Active ability handling

        private void OnActiveAbilityClicked(ActiveAbilitySO clickedAbility)
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
            if (clickedAbility.ActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log("Fighter " + _possessedFighter.name + " does not have enough action points to execute the ability");
                return;
            }
            if (clickedAbility.GodFavorsPointsCost > _possessedFighter.GetGodFavorsPoints())
            {
                Debug.Log("Fighter " + _possessedFighter.name + " does not have enough god favors points to execute the ability");
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
            FightCell[] cellsAvailableForTargeting =
                _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    _fighterTeams,
                    _currentActiveAbility.CellAlterations
                );
            cellsAvailableForTargeting.ToList().ForEach(
                cell => cell.HighlightController.UpdateCurrentDefaultMaterial(_cellHighlightMaterial)
            );
            cellsAvailableForTargeting.ToList().ForEach(
                cell => cell.HighlightController.Highlight(_cellHighlightMaterial)
            );
            _fighterIsTargetingForActiveAbility = true;


        }

        private void TryTriggerActiveAbility(FightCell clickedCell)
        {
            try
            {
                FightCell[] targetedCells = _currentActiveAbility.Targeter.Resolve(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    clickedCell,
                    _fighterTeams
                );
                StopTargetingActiveActiveAbility();
                ResetTargeterCellsMaterial(_currentActiveAbility.Targeter, clickedCell);
                _fighterIsActing = true;
                _possessedFighter.UseActiveAbility(_currentActiveAbility, targetedCells);
                onFighterActionStarted?.Invoke(_possessedFighter);


            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log($"Fighter {_possessedFighter.name} can't use its active ability from cell {_possessedFighter.cell.name}");
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
            _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _currentFightGrid,
                _possessedFighter.cell,
                _fighterTeams,
                _currentActiveAbility.CellAlterations
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
            UnbindEntitiesGroupsMouseEvents(_fighterTeams.Keys.ToList());
            onFighterTurnEnded?.Invoke(_possessedFighter);
        }

        #endregion

        #region Suicide handling

        private void OnPossessedFighterDied(Fighter _fighterThatDied)
        {
            if (_fighterThatDied != _possessedFighter)
            {
                return;
            }
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_currentFightGrid);
            UnbindUIEventsForTurn();
            UnbindEntitiesGroupsMouseEvents(_fighterTeams.Keys.ToList());
        }

        #endregion

        #region Cells highlighting for player feedback

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (FightCell cell in _currentMovePath)
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
            foreach (FightCell cell in _currentMovePath)
            {
                cell.HighlightController.ResetToDefaultMaterial();
            }
        }

        private void HighlightTargeterCells(Targeter targeter, FightCell originCell)
        {
            try
            {
                FightCell[] targetedCells = targeter.Resolve(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    originCell,
                    _fighterTeams
                );
                targetedCells.ToList().ForEach(cell => cell.HighlightController.Highlight(_cellActionableHighlightMaterial));
            }
            catch (TargeterUnresolvableException)
            {
                FightCell[] targetedCells = targeter.GetCellsFromSequence(
                    _currentFightGrid,
                    _possessedFighter.cell,
                    originCell
                );
                targetedCells.ToList().ForEach(
                    cell => cell.HighlightController.Highlight(_cellInaccessibleHighlightMaterial)
                );
            }
        }

        private void ResetTargeterCellsMaterial(Targeter targeter, FightCell originCell)
        {
            FightCell[] targetedCells = targeter.GetCellsFromSequence(
                _currentFightGrid,
                _possessedFighter.cell,
                originCell
            );
            targetedCells.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }

        #endregion

        #region UI events binding

        private void BindUIEventsForTurn()
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
            }

            _actionPanel.onDirectAttackClicked += OnDirectAttackClicked;
            _actionPanel.onActiveAbilityClicked += OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked += OnEndTurnClicked;
        }

        private void UnbindUIEventsForTurn()
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
            }

            _actionPanel.onDirectAttackClicked -= OnDirectAttackClicked;
            _actionPanel.onActiveAbilityClicked -= OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked -= OnEndTurnClicked;
        }

        #endregion

        #region Possessed fighter events binding

        private void BindPossessedFighterEventsForTurn(Fighter _possessedFighter)
        {
            _possessedFighter.onFighterMoved += OnFighterMoved;
            _possessedFighter.onFighterDirectAttackEnded += OnFighterDirectAttackEnded;
            _possessedFighter.onFighterActiveAbilityEnded += OnFighterActiveAbilityEnded;
            _possessedFighter.onFighterDied += OnPossessedFighterDied;
        }

        private void UnbindFighterEventsForTurn()
        {
            _possessedFighter.onFighterMoved -= OnFighterMoved;
            _possessedFighter.onFighterDirectAttackEnded -= OnFighterDirectAttackEnded;
            _possessedFighter.onFighterActiveAbilityEnded -= OnFighterActiveAbilityEnded;
            _possessedFighter.onFighterDied -= OnPossessedFighterDied;
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

        #region Fighters mouse events binding

        private void BindFightersMouseEvents(List<Fighter> fighters)
        {
            fighters.ForEach(fighter =>
            {
                fighter.FighterMouseEventsController.OnElementHover += OnFighterHovered;
                fighter.FighterMouseEventsController.OnElementUnhover += OnFighterUnhovered;
                fighter.FighterMouseEventsController.OnLeftMouseUp += OnFighterClicked;
            });
        }

        private void UnbindEntitiesGroupsMouseEvents(List<Fighter> fighters)
        {
            fighters.ForEach(fighter =>
            {
                fighter.FighterMouseEventsController.OnElementHover += OnFighterHovered;
                fighter.FighterMouseEventsController.OnElementUnhover += OnFighterUnhovered;
                fighter.FighterMouseEventsController.OnLeftMouseUp += OnFighterClicked;
            });
        }

        #endregion
    }
}