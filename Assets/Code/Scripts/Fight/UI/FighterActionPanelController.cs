using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.InventorySystem.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        [SerializeField] private FightManager _fightManager;
        [SerializeField] private VisualTreeAsset _statusIconContainerTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private VisualTreeAsset _consumableSlotTemplate;
        [SerializeField] private Color _movementPointsProgressColor;
        [SerializeField] private Color _actionPointsProgressColor;
        [SerializeField] private Color _statValueColor = new(0.8f, 0.8f, 0.8f);
        [SerializeField] private float _statusDetailsPanelXOffset = 4f;
        private AbilitiesBarController _abilitiesBarController;

        private VisualElement _actionPanelRoot;
        private ConsumablesUIController _consumablesUIController;
        private VisualElement _detailsPanelsContainer;
        private VisualElement _objectDetailsPanelContainer;
        private ObjectDetailsUIController _objectDetailsPanelController;

        private Fighter _playingFighter;
        private StatusDetailsPanelUIController _statusDetailsPanelController;
        private VisualElement _statusesContainer;
        private TeamMatesPanelController _teamMatesPanelController;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action<InventorySlot> onConsumableClicked;

        public Action onDirectAttackClicked;
        public Action onEndTurnClicked;

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
            _detailsPanelsContainer = _uiDoc.rootVisualElement.Q<VisualElement>(DETAILS_PANELS_CONTAINER_UI_NAME);

            // Setup sub controllers
            _statusDetailsPanelController = new StatusDetailsPanelUIController(
                _actionPanelRoot.Q<VisualElement>(STATUS_DETAILS_PANEL_UI_NAME)
            );
            _statusDetailsPanelController.Hide();

            _objectDetailsPanelContainer =
                _uiDoc.rootVisualElement.Q<VisualElement>(OBJECT_DETAILS_PANEL_CONTAINER_UI_NAME);
            _objectDetailsPanelController = new ObjectDetailsUIController(
                _objectDetailsPanelContainer,
                _statContainerTemplate,
                OBJECT_DETAILS_EFFECT_LINE_CLASSNAME,
                _statValueColor
            );
            _objectDetailsPanelContainer.AddToClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);

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
            _abilitiesBarController.onDirectAttackLongHovered += OnDirectAttackLongHovered;
            _abilitiesBarController.onDirectAttackLongUnhovered += () => HideObjectDetailsPanel();

            _abilitiesBarController.onAbilityClicked += (ability) => onActiveAbilityClicked?.Invoke(ability);
            _abilitiesBarController.onAbilityLongHovered += OnAbilityButtonLongHovered;
            _abilitiesBarController.onAbilityLongUnhovered += (_) => HideObjectDetailsPanel();

            _consumablesUIController.onConsumableUsed +=
                (consumableSlot) => onConsumableClicked?.Invoke(consumableSlot);
            _consumablesUIController.onConsumableLongHovered += OnConsumableLongHovered;
            _consumablesUIController.onConsumableLongUnhovered += (_) => HideObjectDetailsPanel();

            _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME).clicked += () => onEndTurnClicked?.Invoke();

            // Subscribe to fight events
            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
            _fightManager.onFighterTurnEnded += OnFighterTurnEnded;
            _fightManager.onFightEnded += OnFightEnded;
        }

        private void OnFighterTurnBegan(Fighter playingFighter, bool isAlly)
        {
            if (!isAlly) return;

            _playingFighter = playingFighter;
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

        private void HideObjectDetailsPanel()
        {
            _objectDetailsPanelContainer.AddToClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
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

        private void OnDirectAttackLongHovered()
        {
            List<string> weaponSpecialEffects = _playingFighter.Weapon.GetSpecialEffectsUIData();

            _objectDetailsPanelContainer.RemoveFromClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _objectDetailsPanelController.Setup(
                icon: _playingFighter.Weapon.IconSprite,
                name: _playingFighter.Weapon.Name,
                description: _playingFighter.Weapon.Description,
                stats: _playingFighter.Weapon.GetStatsUIData(),
                primaryEffectsTitle: weaponSpecialEffects.Count > 0 ? "Special effects" : null,
                primaryEffects: weaponSpecialEffects
            );
        }

        private void OnAbilityButtonLongHovered(ActiveAbilitySO longHoveredAbility)
        {
            _objectDetailsPanelContainer.RemoveFromClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _objectDetailsPanelController.Setup(
                icon: longHoveredAbility.IconSprite,
                name: longHoveredAbility.Name,
                description: longHoveredAbility.Description,
                stats: longHoveredAbility.GetStatsUIData(),
                primaryEffectsTitle: "Effects",
                primaryEffects: longHoveredAbility.GetEffectsUIData(),
                secondaryEffectsTitle: "Masterstroke Effects",
                secondaryEffects: longHoveredAbility.GetMasterstrokeEffectsUIData()
            );
        }

        private void OnConsumableLongHovered(InventorySlot consumableSlot)
        {
            if (consumableSlot.Item is not AConsumable consumable) return;

            _objectDetailsPanelContainer.RemoveFromClassList(OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME);
            _objectDetailsPanelController.Setup(
                icon: consumable.IconSprite,
                name: consumable.Name,
                description: consumable.Description,
                stats: null,
                primaryEffectsTitle: "Effects",
                primaryEffects: consumable.GetEffectsUIData()
            );
        }

        private void OnPlayingFighterStatusesChanged(Fighter playingFighter, AStatus status)
        {
            if (playingFighter.GetStatuses().Count == 0)
            {
                _detailsPanelsContainer.style.bottom = new Length(70, LengthUnit.Percent);
            }
            else
            {
                _detailsPanelsContainer.style.bottom = new Length(100, LengthUnit.Percent);
            }

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

        #region UXML Element names and classes

        private static readonly string ACTION_PANEL_ROOT_UI_NAME = "ActionPanelRoot";
        private static readonly string TEAM_MATES_PANEL_ROOT_UI_NAME = "TeamInfosPanel";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesContainer";
        private static readonly string DETAILS_PANELS_CONTAINER_UI_NAME = "DetailsPanelsContainer";
        private static readonly string STATUS_DETAILS_PANEL_UI_NAME = "StatusDetailsPanel";
        private static readonly string OBJECT_DETAILS_PANEL_CONTAINER_UI_NAME = "ObjectDetailsPanelContainer";
        private static readonly string ABILITIES_CONTAINER_UI_NAME = "AbilitiesContainer";
        private static readonly string CONSUMABLES_BAR_UI_NAME = "ConsumablesBar";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";

        private static readonly string ACTION_PANEL_HIDDEN_CLASSNAME = "actionPanelRootHidden";

        private static readonly string OBJECT_DETAILS_PANEL_CONTAINER_HIDDEN_CLASSNAME =
            "objectDetailsPanelContainerHidden";

        private static readonly string OBJECT_DETAILS_EFFECT_LINE_CLASSNAME = "objectDetailsEffectLine";
        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "bottomPanelStatusIconContainer";

        #endregion

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