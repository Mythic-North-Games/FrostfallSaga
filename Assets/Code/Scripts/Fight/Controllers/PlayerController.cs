using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using UnityEngine;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Core.InventorySystem;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    ///     Make the fighter do what the player indicates through the UI.
    /// </summary>
    public class PlayerController : AFighterController
    {
        private FighterActionPanelController _actionPanel;
        private Material _cellActionableHighlightMaterial;
        private Material _cellHighlightMaterial;
        private Material _cellInaccessibleHighlightMaterial;
        private ActiveAbilitySO _currentActiveAbility;

        private FightCell[] _currentMovePath = { };
        private bool _fighterIsActing;
        private bool _fighterIsTargetingForActiveAbility;
        private bool _fighterIsTargetingForDirectAttack;
        private FightManager _fightManager;
        private Fighter _possessedFighter;

        public void Setup(
            FightManager fightManager,
            FighterActionPanelController actionPanel,
            Material cellHighlightMaterial,
            Material cellActionableHighlightMaterial,
            Material cellInaccessibleHighlightMaterial
        )
        {
            _fightManager = fightManager;
            _actionPanel = actionPanel;
            _cellHighlightMaterial = cellHighlightMaterial;
            _cellActionableHighlightMaterial = cellActionableHighlightMaterial;
            _cellInaccessibleHighlightMaterial = cellInaccessibleHighlightMaterial;

            BindUIEvents();
            _fightManager.onFightEnded += OnFightEnded;
        }

        public override void PlayTurn(Fighter fighterToPlay)
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

            _possessedFighter = fighterToPlay;
            _fighterIsActing = false;
            _fighterIsTargetingForActiveAbility = false;
            _fighterIsTargetingForDirectAttack = false;

            BindPossessedFighterEventsForTurn(fighterToPlay);
            BindFightersMouseEvents(_fightManager.FighterTeams.Keys.ToList());
            BindCellMouseEventsForTurn(_fightManager.FightGrid);
        }

        private void OnCellClicked(Cell clickedCell)
        {
            FightCell clickedFightCell = (FightCell)clickedCell;

            if (_fighterIsActing) return;

            if (_fighterIsTargetingForDirectAttack && clickedFightCell != _possessedFighter.cell)
                TryTriggerDirectAttack(clickedFightCell);
            else if (_fighterIsTargetingForActiveAbility)
                TryTriggerActiveAbility(clickedFightCell);
            else if (
                _currentMovePath != null &&
                _currentMovePath.Length > 0 && clickedFightCell != _possessedFighter.cell
            )
                MakeFighterMove(clickedFightCell);
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            FightCell hoveredFightCell = (FightCell)hoveredCell;

            if (_fighterIsActing) return;

            if (
                _fighterIsTargetingForDirectAttack &&
                hoveredFightCell != _possessedFighter.cell &&
                _possessedFighter.Weapon.AttackTargeter.IsCellTargetable(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    hoveredFightCell,
                    _fightManager.FighterTeams
                )
            )
            {
                HighlightTargeterCells(_possessedFighter.Weapon.AttackTargeter, hoveredFightCell);
            }
            else if (
                _fighterIsTargetingForActiveAbility &&
                _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    _fightManager.FighterTeams,
                    _currentActiveAbility.CellAlterations
                ).Contains(hoveredFightCell)
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
                        _fightManager.FightGrid,
                        _possessedFighter.cell,
                        hoveredFightCell
                    ), cell => (FightCell)cell
                );
                HighlightShorterPathCells();
            }

            if (
                hoveredCell != _possessedFighter.cell && (
                    _fighterIsTargetingForActiveAbility || _fighterIsTargetingForDirectAttack
                )
            )
                _possessedFighter.MovementController.RotateTowardsCell(hoveredCell);
        }

        private void OnCellUnhovered(Cell unhoveredCell)
        {
            FightCell unhoveredFightCell = (FightCell)unhoveredCell;

            if (_fighterIsActing) return;

            if (_fighterIsTargetingForDirectAttack)
                ResetTargeterCellsMaterial(_possessedFighter.Weapon.AttackTargeter, unhoveredFightCell);
            else if (_fighterIsTargetingForActiveAbility)
                ResetTargeterCellsMaterial(_currentActiveAbility.Targeter, unhoveredFightCell);
            else if (
                !_fighterIsTargetingForActiveAbility &&
                !_fighterIsTargetingForDirectAttack &&
                _currentMovePath != null &&
                _currentMovePath.Length > 0
            )
                ResetShorterPathCellsDefaultMaterial();
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

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_fightManager.FightGrid);
            UnbindEntitiesGroupsMouseEvents(_fightManager.FighterTeams.Keys.ToList());
        }

        #region Suicide handling

        private void OnPossessedFighterDied(Fighter _fighterThatDied)
        {
            if (_fighterThatDied != _possessedFighter) return;
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_fightManager.FightGrid);
            UnbindEntitiesGroupsMouseEvents(_fightManager.FighterTeams.Keys.ToList());
        }

        #endregion

        #region Movement handling

        private void MakeFighterMove(FightCell destinationCell)
        {
            if (_currentMovePath.Length > _possessedFighter.GetMovePoints()) return;

            ResetShorterPathCellsDefaultMaterial();
            _fighterIsActing = true;
            _possessedFighter.Move(_currentMovePath);
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
                Debug.Log($"Fighter {_possessedFighter.name} is doing something else. Wait for it to finish.");
                return;
            }

            if (_fighterIsTargetingForDirectAttack)
            {
                StopTargetingForDirectAttack();
                return;
            }

            if (_possessedFighter.Weapon.UseActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log($"Fighter {_possessedFighter.name} does not have enough action points to execute its direct attack.");
                return;
            }

            if (_fighterIsTargetingForActiveAbility) StopTargetingActiveActiveAbility();
            if (_currentMovePath.Length > 0) ResetShorterPathCellsDefaultMaterial();

            FightCell[] cellsAvailableForTargeting =
                _possessedFighter.Weapon.AttackTargeter.GetAllCellsAvailableForTargeting(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    _fightManager.FighterTeams
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
                FightCell[] targetedCells = _possessedFighter.Weapon.AttackTargeter.Resolve(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    clickedCell,
                    _fightManager.FighterTeams
                );
                StopTargetingForDirectAttack();
                _fighterIsActing = true;
                _possessedFighter.UseDirectAttack(targetedCells);
                onFighterActionStarted?.Invoke(_possessedFighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log($"Fighter {_possessedFighter.name} can't use its direct attack from cell " +
                          _possessedFighter.cell.name);
            }
        }

        private void OnFighterDirectAttackEnded(Fighter _possessedFighter)
        {
            EndFighterAction();
        }

        private void StopTargetingForDirectAttack()
        {
            _fighterIsTargetingForDirectAttack = false;
            _possessedFighter.Weapon.AttackTargeter.GetAllCellsAvailableForTargeting(
                _fightManager.FightGrid,
                _possessedFighter.cell,
                _fightManager.FighterTeams
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
        }

        #endregion

        #region Active ability handling

        private void OnActiveAbilityClicked(ActiveAbilitySO clickedAbility)
        {
            if (_fighterIsActing)
            {
                Debug.Log($"Fighter {_possessedFighter.name} is doing something else. Wait for it to finish.");
                return;
            }

            if (_fighterIsTargetingForActiveAbility)
            {
                StopTargetingActiveActiveAbility();
                return;
            }

            if (clickedAbility.ActionPointsCost > _possessedFighter.GetActionPoints())
            {
                Debug.Log($"Fighter {_possessedFighter.name} does not have enough action points to execute the ability");
                return;
            }

            if (clickedAbility.GodFavorsPointsCost > _possessedFighter.GetGodFavorsPoints())
            {
                Debug.Log($"Fighter {_possessedFighter.name} does not have enough god favors points to execute the ability");
                return;
            }

            if (_fighterIsTargetingForDirectAttack) StopTargetingForDirectAttack();
            if (_currentMovePath.Length > 0) ResetShorterPathCellsDefaultMaterial();

            _currentActiveAbility = clickedAbility;
            FightCell[] cellsAvailableForTargeting =
                _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    _fightManager.FighterTeams,
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
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    clickedCell,
                    _fightManager.FighterTeams,
                    _currentActiveAbility.CellAlterations
                );
                StopTargetingActiveActiveAbility();
                ResetTargeterCellsMaterial(_currentActiveAbility.Targeter, clickedCell);
                _fighterIsActing = true;
                _possessedFighter.UseActiveAbility(_currentActiveAbility, targetedCells);
                onFighterActionStarted?.Invoke(_possessedFighter);
            }
            catch (TargeterUnresolvableException)
            {
                Debug.Log(
                    $"Fighter {_possessedFighter.name} can't use its active ability from cell {_possessedFighter.cell.name}"
                );
            }
        }

        private void OnFighterActiveAbilityEnded(Fighter _possessedFighter, ActiveAbilitySO usedAbility)
        {
            EndFighterAction();
        }

        private void StopTargetingActiveActiveAbility()
        {
            _fighterIsTargetingForActiveAbility = false;
            _possessedFighter.cell.HighlightController.ResetToInitialMaterial();
            _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _fightManager.FightGrid,
                _possessedFighter.cell,
                _fightManager.FighterTeams,
                _currentActiveAbility.CellAlterations
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialMaterial());
        }

        #endregion

        #region Consumable use handling

        private void OnConsumableClicked(InventorySlot consumableSlot)
        {
            if (_fighterIsActing)
            {
                Debug.Log($"Fighter {_possessedFighter.name} is doing something else. Wait for it to finish.");
                return;
            }

            if (_possessedFighter.GetActionPoints() < (consumableSlot.Item as ConsumableSO).ActionPointsCost)
            {
                Debug.Log($"Fighter {_possessedFighter.name} does not have enough action points to use the consumable.");
                return;
            }

            if (_fighterIsTargetingForActiveAbility) StopTargetingActiveActiveAbility();
            if (_fighterIsTargetingForDirectAttack) StopTargetingForDirectAttack();

            _fighterIsActing = true;
            _possessedFighter.UseConsumable(consumableSlot);
            onFighterActionStarted?.Invoke(_possessedFighter);
        }

        private void OnFighterConsumableUseEnded(Fighter _possessedFighter, InventorySlot _consumableSlotUsed)
        {
            EndFighterAction();
        }

        #endregion

        #region End turn handling

        private void OnEndTurnClicked()
        {
            if (_fighterIsActing)
            {
                Debug.Log($"Fighter {_possessedFighter.name} is doing something else. Wait for it to finish.");
                return;
            }

            if (_fighterIsTargetingForDirectAttack) StopTargetingForDirectAttack();
            if (_fighterIsTargetingForActiveAbility) StopTargetingActiveActiveAbility();
            if (_currentMovePath.Length > 0) ResetShorterPathCellsDefaultMaterial();

            EndPossessedFighterTurn();
        }

        private void EndPossessedFighterTurn()
        {
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents(_fightManager.FightGrid);
            UnbindEntitiesGroupsMouseEvents(_fightManager.FighterTeams.Keys.ToList());
            onFighterTurnEnded?.Invoke(_possessedFighter);
        }

        #endregion

        #region Cells highlighting for player feedback

        private void HighlightShorterPathCells()
        {
            int i = 0;
            foreach (FightCell cell in _currentMovePath)
            {
                if (i < _possessedFighter.GetMovePoints())
                    cell.HighlightController.Highlight(_cellHighlightMaterial);
                else
                    cell.HighlightController.Highlight(_cellInaccessibleHighlightMaterial);
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (FightCell cell in _currentMovePath) cell.HighlightController.ResetToDefaultMaterial();
        }

        private void HighlightTargeterCells(Targeter targeter, FightCell originCell)
        {
            try
            {
                FightCell[] targetedCells = targeter.Resolve(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    originCell,
                    _fightManager.FighterTeams
                );
                targetedCells.ToList()
                    .ForEach(cell => cell.HighlightController.Highlight(_cellActionableHighlightMaterial));
            }
            catch (TargeterUnresolvableException)
            {
                FightCell[] targetedCells = targeter.GetCellsFromSequence(
                    _fightManager.FightGrid,
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
                _fightManager.FightGrid,
                _possessedFighter.cell,
                originCell
            );
            targetedCells.ToList().ForEach(cell => cell.HighlightController.ResetToDefaultMaterial());
        }

        #endregion

        #region UI events binding

        private void BindUIEvents()
        {
            if (_actionPanel == null) Debug.LogError("Player controller has no action panel to work with.");

            _actionPanel.onDirectAttackClicked += OnDirectAttackClicked;
            _actionPanel.onActiveAbilityClicked += OnActiveAbilityClicked;
            _actionPanel.onConsumableClicked += OnConsumableClicked;
            _actionPanel.onEndTurnClicked += OnEndTurnClicked;
        }

        #endregion

        #region Possessed fighter events binding

        private void BindPossessedFighterEventsForTurn(Fighter _possessedFighter)
        {
            _possessedFighter.onFighterMoved += OnFighterMoved;
            _possessedFighter.onDirectAttackEnded += OnFighterDirectAttackEnded;
            _possessedFighter.onActiveAbilityEnded += OnFighterActiveAbilityEnded;
            _possessedFighter.onConsumableUseEnded += OnFighterConsumableUseEnded;
            _possessedFighter.onFighterDied += OnPossessedFighterDied;
        }

        private void UnbindFighterEventsForTurn()
        {
            _possessedFighter.onFighterMoved -= OnFighterMoved;
            _possessedFighter.onDirectAttackEnded -= OnFighterDirectAttackEnded;
            _possessedFighter.onActiveAbilityEnded -= OnFighterActiveAbilityEnded;
            _possessedFighter.onFighterDied -= OnPossessedFighterDied;
        }

        #endregion

        #region Cells mouse events binding

        private void BindCellMouseEventsForTurn(FightHexGrid fightGrid)
        {
            foreach (Cell cell in fightGrid.GetCells())
            {
                cell.CellMouseEventsController.OnElementHover += OnCellHovered;
                cell.CellMouseEventsController.OnElementUnhover += OnCellUnhovered;
                cell.CellMouseEventsController.OnLeftMouseUp += OnCellClicked;
            }
        }

        private void UnbindCellMouseEvents(FightHexGrid fightGrid)
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