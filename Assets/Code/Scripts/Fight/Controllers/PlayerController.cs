using System;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.UI;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Inputs;
using UnityEngine;

namespace FrostfallSaga.Fight.Controllers
{
    /// <summary>
    ///     Make the fighter do what the player indicates through the UI.
    /// </summary>
    public class PlayerController : AFighterController
    {
        private FighterActionPanelController _actionPanel;
        private ActiveAbilitySO _currentActiveAbility;

        private FightCell[] _currentMovePath = { };
        private bool _fighterIsActing;
        private bool _fighterIsTargetingForActiveAbility;
        private bool _fighterIsTargetingForDirectAttack;
        private HighlightColor _currentDefaultHighlightColor;
        private FightManager _fightManager;
        private bool _isMouseLeftButtonHold;
        private Fighter _possessedFighter;

        private TypedWorldMouseInteractor<FightCell> _cellsMouseInteractor;
        private TypedWorldMouseInteractor<Fighter> _fightersMouseInteractor;

        public void Setup(
            FightManager fightManager,
            FighterActionPanelController actionPanel
        )
        {
            _fightManager = fightManager;
            _actionPanel = actionPanel;
            BindUIEvents();
            _fightManager.onFightEnded += OnFightEnded;
        }

        public override void PlayTurn(Fighter fighterToPlay)
        {
            if (!_actionPanel)
            {
                Debug.LogError("Player controller has no action panel to work with.");
                return;
            }

            _possessedFighter = fighterToPlay;
            _fighterIsActing = false;
            _fighterIsTargetingForActiveAbility = false;
            _fighterIsTargetingForDirectAttack = false;
            _currentDefaultHighlightColor = HighlightColor.NONE;

            BindPossessedFighterEventsForTurn(fighterToPlay);
            BindFightersMouseEvents();
            BindCellMouseEvents();
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
                _currentMovePath is { Length: > 0 } && clickedFightCell != _possessedFighter.cell
            )
                MakeFighterMove();
        }

        private void OnCellHovered(Cell hoveredCell)
        {
            FightCell hoveredFightCell = (FightCell)hoveredCell;

            if (_fighterIsActing || _isMouseLeftButtonHold) return;

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
                    _currentActiveAbility.cellAlterations
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
                _currentMovePath is { Length: > 0 }
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

        private void OnLongClickHold(Cell _cell)
        {
            _isMouseLeftButtonHold = true;
        }

        private void OnLongClick(Cell _cell)
        {
            _isMouseLeftButtonHold = false;
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents();
            UnbindFightersMouseEvents();
        }

        #region Suicide handling

        private void OnPossessedFighterDied(Fighter _fighterThatDied)
        {
            if (_fighterThatDied != _possessedFighter) return;
            UnbindFighterEventsForTurn();
            UnbindCellMouseEvents();
            UnbindFightersMouseEvents();
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

        #region Movement handling

        private void MakeFighterMove()
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
                Debug.Log(
                    $"Fighter {_possessedFighter.name} does not have enough action points to execute its direct attack.");
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
                cell => cell.HighlightController.Highlight(HighlightColor.ACCESSIBLE)
            );

            _currentDefaultHighlightColor = HighlightColor.ACCESSIBLE;
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
            _currentDefaultHighlightColor = HighlightColor.NONE;
            _fighterIsTargetingForDirectAttack = false;
            _possessedFighter.Weapon.AttackTargeter.GetAllCellsAvailableForTargeting(
                _fightManager.FightGrid,
                _possessedFighter.cell,
                _fightManager.FighterTeams
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialColor());
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
                Debug.Log(
                    $"Fighter {_possessedFighter.name} does not have enough action points to execute the ability");
                return;
            }

            if (clickedAbility.GodFavorsPointsCost > _possessedFighter.GetGodFavorsPoints())
            {
                Debug.Log(
                    $"Fighter {_possessedFighter.name} does not have enough god favors points to execute the ability");
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
                    _currentActiveAbility.cellAlterations
                );
            cellsAvailableForTargeting.ToList().ForEach(
                cell => cell.HighlightController.Highlight(HighlightColor.ACCESSIBLE)
            );

            _currentDefaultHighlightColor = HighlightColor.ACCESSIBLE;
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
                    _currentActiveAbility.cellAlterations
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

        private void OnFighterActiveAbilityEnded(Fighter possessedFighter, ActiveAbilitySO usedAbility)
        {
            EndFighterAction();
        }

        private void StopTargetingActiveActiveAbility()
        {
            _currentDefaultHighlightColor = HighlightColor.NONE;
            _fighterIsTargetingForActiveAbility = false;
            _possessedFighter.cell.HighlightController.ResetToInitialColor();
            _currentActiveAbility.Targeter.GetAllCellsAvailableForTargeting(
                _fightManager.FightGrid,
                _possessedFighter.cell,
                _fightManager.FighterTeams,
                _currentActiveAbility.cellAlterations
            ).ToList().ForEach(cell => cell.HighlightController.ResetToInitialColor());
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
                Debug.Log(
                    $"Fighter {_possessedFighter.name} does not have enough action points to use the consumable.");
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
            UnbindCellMouseEvents();
            UnbindFightersMouseEvents();
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
                    cell.HighlightController.Highlight(HighlightColor.ACCESSIBLE);
                else
                    cell.HighlightController.Highlight(HighlightColor.INACCESSIBLE);
                i++;
            }
        }

        private void ResetShorterPathCellsDefaultMaterial()
        {
            foreach (FightCell cell in _currentMovePath) cell.HighlightController.ResetToInitialColor();
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
                    .ForEach(cell => cell.HighlightController.Highlight(HighlightColor.ACTIONABLE));
            }
            catch (TargeterUnresolvableException)
            {
                FightCell[] targetedCells = targeter.GetCellsFromSequence(
                    _fightManager.FightGrid,
                    _possessedFighter.cell,
                    originCell
                );
                targetedCells.ToList().ForEach(
                    cell => cell.HighlightController.Highlight(HighlightColor.INACCESSIBLE)
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
            targetedCells.ToList().ForEach(
                cell =>
                {
                    if (_currentDefaultHighlightColor != HighlightColor.NONE)
                        cell.HighlightController.Highlight(_currentDefaultHighlightColor);
                    else
                        cell.HighlightController.ResetToInitialColor();
                }
            );
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

        private void BindCellMouseEvents()
        {
            _cellsMouseInteractor = new();
            _cellsMouseInteractor.onHovered += OnCellHovered;
            _cellsMouseInteractor.onUnhovered += OnCellUnhovered;
            _cellsMouseInteractor.onLeftUp += OnCellClicked;
            _cellsMouseInteractor.onLeftClickHold += OnLongClickHold;
            _cellsMouseInteractor.onLeftClickHoldReleased += OnLongClick;
        }

        private void UnbindCellMouseEvents()
        {
            if (_cellsMouseInteractor == null) return;
            
            _cellsMouseInteractor.onHovered -= OnCellHovered;
            _cellsMouseInteractor.onUnhovered -= OnCellUnhovered;
            _cellsMouseInteractor.onLeftUp -= OnCellClicked;
            _cellsMouseInteractor.onLeftClickHold -= OnLongClickHold;
            _cellsMouseInteractor.onLeftClickHoldReleased -= OnLongClick;
        }

        #endregion

        #region Fighters mouse events binding

        private void BindFightersMouseEvents()
        {
            _fightersMouseInteractor = new();
            _fightersMouseInteractor.onHovered += OnFighterHovered;
            _fightersMouseInteractor.onUnhovered += OnFighterUnhovered;
            _fightersMouseInteractor.onLeftUp += OnFighterClicked;
        }

        private void UnbindFightersMouseEvents()
        {
            if (_fightersMouseInteractor == null) return;

            _fightersMouseInteractor.onHovered -= OnFighterHovered;
            _fightersMouseInteractor.onUnhovered -= OnFighterUnhovered;
            _fightersMouseInteractor.onLeftUp -= OnFighterClicked;
        }

        #endregion

        private void OnDisable()
        {
            UnbindCellMouseEvents();
            UnbindFighterEventsForTurn();
            UnbindFightersMouseEvents();
        }
    }
}