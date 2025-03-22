using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Fight.Fighters;
using System;
using FrostfallSaga.Fight.Abilities;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        #region UXML Element names and classes
        private static readonly string ACTION_PANEL_ROOT_UI_NAME = "ActionPanelRoot";
        private static readonly string TEAM_MATES_PANEL_ROOT_UI_NAME = "TeamInfosPanel";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesContainer";
        private static readonly string ABILITIES_CONTAINER_UI_NAME = "AbilitiesContainer";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";

        private static readonly string ACTION_PANEL_HIDDEN_CLASSNAME = "actionPanelRootHidden";
        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "bottomPanelStatusIconContainer";
        #endregion

        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action onEndTurnClicked;

        [SerializeField] private FightManager _fightManager;
        [SerializeField] private VisualTreeAsset _statusIconContainerTemplate;
        [SerializeField] private Color _movementPointsProgressColor;
        [SerializeField] private Color _actionPointsProgressColor;
        private VisualElement _actionPanelRoot;
        private VisualElement _statusesContainer;
        private TeamMatesPanelController _teamMatesPanelController;
        private AbilitiesBarController _abilitiesBarController;

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
            _teamMatesPanelController = new TeamMatesPanelController(
                _uiDoc.rootVisualElement.Q<VisualElement>(TEAM_MATES_PANEL_ROOT_UI_NAME),
                _movementPointsProgressColor,
                _actionPointsProgressColor
            );
            _abilitiesBarController = new AbilitiesBarController(
                _uiDoc.rootVisualElement.Q<VisualElement>(ABILITIES_CONTAINER_UI_NAME)
            );

            // Subscribe to events
            _abilitiesBarController.onDirectAttackClicked += () => onDirectAttackClicked?.Invoke();
            _abilitiesBarController.onAbilityButtonClicked += (ability) => onActiveAbilityClicked?.Invoke(ability);
            _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME).clicked += () => onEndTurnClicked?.Invoke();

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
            Hide();
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            _actionPanelRoot.RemoveFromHierarchy();
        }

        private void SetupActionPanelForFighter(Fighter playingFighter)
        {
            Fighter[] teamMates = _fightManager.GetMatesOfFighter(playingFighter);
            _teamMatesPanelController.Setup(
                playingFighter,
                teamMates.ElementAtOrDefault(0),
                teamMates.ElementAtOrDefault(1)
            );
            _abilitiesBarController.UpdateAbilities(playingFighter);
            UpdateStatuses(playingFighter);
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

        private void RegisterPlayingFighterEvents(Fighter playingFighter)
        {
            // Statuses events
            playingFighter.onStatusApplied += (_fighter, _status) => {
                UpdateStatuses(playingFighter);
            };
            playingFighter.onStatusRemoved += (_fighter, _status) => {
                UpdateStatuses(playingFighter);
            };

            // Abilities events
            playingFighter.onActiveAbilityEnded += (_fighter, _ability) => {
                if (playingFighter.IsDead()) return;    // Suicide ability ends after new turn so check the health 
                _abilitiesBarController.UpdateAbilities(playingFighter);
            };
            playingFighter.onDirectAttackEnded += (_fighter) => _abilitiesBarController.UpdateAbilities(playingFighter);
            playingFighter.onNonMagicalStatMutated += (_fighter, _mutatedStat, _newValue) => _abilitiesBarController.UpdateAbilities(playingFighter);
            playingFighter.onPassiveAbilityApplied += (_fighter, _ability) =>
            {
                _abilitiesBarController.UpdateAbilities(playingFighter);
                UpdateStatuses(playingFighter);
            };
            playingFighter.onPassiveAbilityRemoved += (_fighter, _ability) =>
            {
                _abilitiesBarController.UpdateAbilities(playingFighter);
                UpdateStatuses(playingFighter);
            };
        }
    }
}