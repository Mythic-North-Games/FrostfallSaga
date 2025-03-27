using System;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        #region UXML Element names and classes
        private static readonly string ACTION_PANEL_ROOT_UI_NAME = "ActionPanelRoot";
        private static readonly string TEAM_MATES_PANEL_ROOT_UI_NAME = "TeamInfosPanel";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesContainer";
        private static readonly string STATUS_DETAILS_PANEL_UI_NAME = "StatusDetailsPanel";
        private static readonly string ABILITIES_CONTAINER_UI_NAME = "AbilitiesContainer";
        private static readonly string CONSUMABLES_BAR_UI_NAME = "ConsumablesBar";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";

        private static readonly string ACTION_PANEL_HIDDEN_CLASSNAME = "actionPanelRootHidden";
        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "bottomPanelStatusIconContainer";
        #endregion

        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action<InventorySlot> onConsumableClicked;
        public Action onEndTurnClicked;

        [SerializeField] private FightManager _fightManager;
        [SerializeField] private VisualTreeAsset _statusIconContainerTemplate;
        [SerializeField] private VisualTreeAsset _consumableSlotTemplate;
        [SerializeField] private Color _movementPointsProgressColor;
        [SerializeField] private Color _actionPointsProgressColor;
        [SerializeField] private float _statusDetailsPanelXOffset = 4f;

        private VisualElement _actionPanelRoot;
        private VisualElement _statusesContainer;
        private StatusDetailsPanelUIController _statusDetailsPanelController;
        private TeamMatesPanelController _teamMatesPanelController;
        private AbilitiesBarController _abilitiesBarController;
        private ConsumablesUIController _consumablesUIController;

        private void Awake()
        {
            // Setup sub controllers and components
            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
            if (_fightManager == null)
            {
                Debug.LogError("FightManager not found in scene.");
                return;
            }

            _actionPanelRoot = _uiDoc.rootVisualElement.Q<VisualElement>(ACTION_PANEL_ROOT_UI_NAME);
            _statusesContainer = _uiDoc.rootVisualElement.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME);

            // Setup sub controllers
            _statusDetailsPanelController = new StatusDetailsPanelUIController(
                _actionPanelRoot.Q<VisualElement>(STATUS_DETAILS_PANEL_UI_NAME)
            );
            _statusDetailsPanelController.Hide();

            _teamMatesPanelController = new TeamMatesPanelController(
                _uiDoc.rootVisualElement.Q<VisualElement>(TEAM_MATES_PANEL_ROOT_UI_NAME),
                _movementPointsProgressColor,
                _actionPointsProgressColor
            );
            _abilitiesBarController = new AbilitiesBarController(
                _uiDoc.rootVisualElement.Q<VisualElement>(ABILITIES_CONTAINER_UI_NAME)
            );
            _consumablesUIController = new ConsumablesUIController(
                _uiDoc.rootVisualElement.Q<VisualElement>(CONSUMABLES_BAR_UI_NAME),
                _consumableSlotTemplate
            );

            // Setup internal events
            _abilitiesBarController.onDirectAttackClicked += () => onDirectAttackClicked?.Invoke();
            _abilitiesBarController.onAbilityButtonClicked += (ability) => onActiveAbilityClicked?.Invoke(ability);
            _consumablesUIController.onConsumableUsed += (consumableSlot) => onConsumableClicked?.Invoke(consumableSlot);
            _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME).clicked += () => onEndTurnClicked?.Invoke();

            // Subscribe to fight events
            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
            _fightManager.onFighterTurnEnded += OnFighterTurnEnded;
            _fightManager.onFightEnded += OnFightEnded;
        }

        private void OnFighterTurnBegan(Fighter playingFighter, bool isAlly)
        {
            if (!isAlly) return;

            SetupActionPanelForFighter(playingFighter);
            RegisterPlayingFighterEvents(playingFighter);
            if (IsHidden()) Display();
        }

        private void OnFighterTurnEnded(Fighter playingFighter, bool isAlly)
        {
            if (!isAlly) return;
            UnRegisterPlayingFighterEvents(playingFighter);
            Hide();
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            _actionPanelRoot.RemoveFromHierarchy();
        }

        private void SetupActionPanelForFighter(Fighter playingFighter)
        {
            Fighter[] teamMates = _fightManager.GetMatesOfFighter(playingFighter);
            _teamMatesPanelController.Update(
                playingFighter,
                teamMates.ElementAtOrDefault(0),
                teamMates.ElementAtOrDefault(1)
            );
            _abilitiesBarController.UpdateAbilities(playingFighter);
            UpdateStatuses(playingFighter);
            _consumablesUIController.UpdateConsumables(playingFighter.Inventory);
        }

        private void UpdateStatuses(Fighter playingFighter)
        {
            Dictionary<AStatus, (bool isActive, int duration)> currentStatuses = playingFighter.GetStatuses();
            _statusesContainer.Clear();

            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in currentStatuses)
            {
                VisualElement statusIconContainerRoot = _statusIconContainerTemplate.Instantiate();
                statusIconContainerRoot.AddToClassList(STATUS_ICON_CONTAINER_ROOT_CLASSNAME);
                StatusContainerUIController.SetupStatusContainer(statusIconContainerRoot, status.Key);
                statusIconContainerRoot.RegisterCallback<MouseEnterEvent>((_) =>
                {
                    DisplayStatusDetailsPanel(
                        _statusesContainer.IndexOf(statusIconContainerRoot),
                        status.Key,
                        status.Value.duration
                    );
                });
                statusIconContainerRoot.RegisterCallback<MouseLeaveEvent>(HideStatusDetailsPanel);
                _statusesContainer.Add(statusIconContainerRoot);
            }
        }

        private void Display()
        {
            _actionPanelRoot.RemoveFromClassList(ACTION_PANEL_HIDDEN_CLASSNAME);
            _actionPanelRoot.SetEnabled(true);
        }

        private void Hide()
        {
            _actionPanelRoot.AddToClassList(ACTION_PANEL_HIDDEN_CLASSNAME);
            _actionPanelRoot.SetEnabled(false);
        }

        private bool IsHidden()
        {
            return _actionPanelRoot.ClassListContains(ACTION_PANEL_HIDDEN_CLASSNAME);
        }

        private void DisplayStatusDetailsPanel(int statusIconIndex, AStatus statusToDisplay, int lastingDuration)
        {
            _statusDetailsPanelController.Root.style.left = new Length(
                (statusIconIndex + 1) * _statusDetailsPanelXOffset,
                LengthUnit.Percent
            );
            _statusDetailsPanelController.Display(statusToDisplay, lastingDuration);
        }

        private void HideStatusDetailsPanel(MouseLeaveEvent evt)
        {
            _statusDetailsPanelController.Hide();
        }

        private void OnPlayingFighterStatusesChanged(Fighter playingFighter, AStatus _status)
        {
            UpdateStatuses(playingFighter);
        }

        private void OnPlayingFighterActiveAbilitiesChanged(Fighter playingFighter, ActiveAbilitySO _ability)
        {
            _abilitiesBarController.UpdateAbilities(playingFighter);
        }

        private void OnPlayingFighterConsumablesChanged(Fighter playingFighter, InventorySlot _consumableSlot)
        {
            _consumablesUIController.UpdateConsumables(playingFighter.Inventory);
        }

        private void OnPlayingFighterPassiveAbilitiesChanged(Fighter playingFighter, PassiveAbilitySO _ability)
        {
            UpdateStatuses(playingFighter);
            _abilitiesBarController.UpdateAbilities(playingFighter);
        }

        private void OnPlayingFighterNonMagicalStatMutated(
            Fighter playingFighter,
            EFighterMutableStat _mutatedStat,
            float _newValue
        )
        {
            _abilitiesBarController.UpdateAbilities(playingFighter);
        }

        private void OnPlayingFighterDirectAttackEnded(Fighter playingFighter)
        {
            _abilitiesBarController.UpdateAbilities(playingFighter);
        }

        #region Playing fighter events registration
        private void RegisterPlayingFighterEvents(Fighter playingFighter)
        {
            playingFighter.onStatusApplied += OnPlayingFighterStatusesChanged;
            playingFighter.onStatusRemoved += OnPlayingFighterStatusesChanged;

            playingFighter.onActiveAbilityEnded += OnPlayingFighterActiveAbilitiesChanged;
            playingFighter.onDirectAttackEnded += OnPlayingFighterDirectAttackEnded;

            playingFighter.onConsumableUseStarted += OnPlayingFighterConsumablesChanged;

            playingFighter.onNonMagicalStatMutated += OnPlayingFighterNonMagicalStatMutated;

            playingFighter.onPassiveAbilityApplied += OnPlayingFighterPassiveAbilitiesChanged;
            playingFighter.onPassiveAbilityRemoved += OnPlayingFighterPassiveAbilitiesChanged;
        }

        private void UnRegisterPlayingFighterEvents(Fighter playingFighter)
        {
            playingFighter.onStatusApplied -= OnPlayingFighterStatusesChanged;
            playingFighter.onStatusRemoved -= OnPlayingFighterStatusesChanged;

            playingFighter.onActiveAbilityEnded -= OnPlayingFighterActiveAbilitiesChanged;
            playingFighter.onDirectAttackEnded -= OnPlayingFighterDirectAttackEnded;

            playingFighter.onConsumableUseStarted -= OnPlayingFighterConsumablesChanged;

            playingFighter.onNonMagicalStatMutated -= OnPlayingFighterNonMagicalStatMutated;

            playingFighter.onPassiveAbilityApplied -= OnPlayingFighterPassiveAbilitiesChanged;
            playingFighter.onPassiveAbilityRemoved -= OnPlayingFighterPassiveAbilitiesChanged;
        }
        #endregion
    }
}