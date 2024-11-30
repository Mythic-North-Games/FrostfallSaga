using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        private static readonly string ACTION_PANEL_ROOT_UI_NAME = "RootActionPanel";
        private static readonly string ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string DIRECT_ATTACK_BUTTON_UI_NAME = "AbilityButton0";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";
        private static readonly string NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string HP_LABEL_UI_NAME = "ProgressLifeBar_Label";
        private static readonly string HP_PROGRESS_UI_NAME = "ProgressLifeBar_Progress";
        private static readonly string AP_LABEL_UI_NAME = "ProgressActionBar_Label";
        private static readonly string AP_PROGRESS_UI_NAME = "ProgressActionBar_Progress";
        private static readonly string MP_LABEL_UI_NAME = "ProgressMoveBar_Label";
        private static readonly string MP_PROGRESS_UI_NAME = "ProgressMoveBar_Progress";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "Statuses_Panel";
        private static readonly string STATUS_CONTAINER_UI_NAME = "StatusContainer";
        private static readonly string PLAYING_FIGHTER_ICON_CONTAINER_UI_NAME = "TeamMatePanel1Frame";
        private static readonly string MATE1_ICON_CONTAINER_UI_NAME = "TeamMatePanel2Frame";
        private static readonly string MATE2_ICON_CONTAINER_UI_NAME = "TeamMatePanel3Frame";

        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action onEndTurnClicked;

        [SerializeField] private FightManager _fightManager;
        [SerializeField] private FighterDetailsPanelController _fighterDetailsPanelController;
        [SerializeField] private int _fighterDetailsPanelYOffset = 20;

        private readonly Dictionary<Button, ActiveAbilitySO> _buttonToActiveAbility = new();

        /// <summary>
        /// Displays or hides the entire action panel.
        /// </summary>
        /// <param name="isVisible">True to display, False to hide.</param>
        public void SetIsVisible(bool isVisible)
        {
            _uiDoc.rootVisualElement.Q(ACTION_PANEL_ROOT_UI_NAME).style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnFighterTurnBegan(Fighter currentFighter, bool isAlly)
        {

            SetIsVisible(isAlly);
            if (isAlly)
            {
                currentFighter.onDamageReceived += (fighter, damage, isMasterstroke) => UpdateLifeBar(fighter);
                currentFighter.onHealReceived += (fighter, damage, isMasterstroke) => UpdateLifeBar(fighter);
                currentFighter.onDirectAttackStarted += (fighter) => UpdateActionBar(fighter);
                currentFighter.onDirectAttackEnded += (fighter) => UpdateActionBar(fighter);
                currentFighter.onActiveAbilityStarted += (fighter, usedAbility) => UpdateActionBar(fighter);
                currentFighter.onActiveAbilityEnded += (fighter, usedAbility) => UpdateActionBar(fighter);
                currentFighter.onFighterMoved += (fighter) => UpdateMoveBar(fighter);
                currentFighter.onStatusApplied += (fighter, status) => UpdateStatuses(fighter);
                currentFighter.onStatusRemoved += (fighter, status) => UpdateStatuses(fighter);
                currentFighter.onStatMutationReceived += (fighter, stat, value) => UpdateFighterDetails(fighter);
                UpdateActionPanelForFighter(currentFighter);
                UpdateFighterIcons(currentFighter);
            }
        }

        private void UpdateActionPanelForFighter(Fighter fighter)
        {
            UpdateAbilityButtons(fighter);
            UpdateFighterDetails(fighter);
        }

        private void UpdateFighterDetails(Fighter fighter)
        {
            Label fighterNameLabel = _uiDoc.rootVisualElement.Q<Label>(NAME_LABEL_UI_NAME);
            fighterNameLabel.text = fighter.name;

            UpdateLifeBar(fighter);
            UpdateActionBar(fighter);
            UpdateMoveBar(fighter);
            UpdateStatuses(fighter);
        }

        private void UpdateLifeBar(Fighter fighter)
        {
            Label fighterHpLabel = _uiDoc.rootVisualElement.Q<Label>(HP_LABEL_UI_NAME);
            VisualElement fighterHpProgress = _uiDoc.rootVisualElement.Q<VisualElement>(HP_PROGRESS_UI_NAME);
            fighterHpLabel.text = $"{fighter.GetHealth()}/{fighter.GetMaxHealth()}";
            fighterHpProgress.style.width = new Length(
                (float)fighter.GetHealth() / fighter.GetMaxHealth() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateActionBar(Fighter fighter)
        {
            Label fighterApLabel = _uiDoc.rootVisualElement.Q<Label>(AP_LABEL_UI_NAME);
            VisualElement fighterApProgress = _uiDoc.rootVisualElement.Q<VisualElement>(AP_PROGRESS_UI_NAME);
            fighterApLabel.text = $"{fighter.GetActionPoints()}/{fighter.GetMaxActionPoints()}";
            fighterApProgress.style.width = new Length(
                (float)fighter.GetActionPoints() / fighter.GetMaxActionPoints() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateMoveBar(Fighter fighter)
        {
            Label fighterMpLabel = _uiDoc.rootVisualElement.Q<Label>(MP_LABEL_UI_NAME);
            VisualElement fighterMpProgress = _uiDoc.rootVisualElement.Q<VisualElement>(MP_PROGRESS_UI_NAME);
            fighterMpLabel.text = $"{fighter.GetMovePoints()}/{fighter.GetMaxMovePoints()}";
            fighterMpProgress.style.width = new Length(
                (float)fighter.GetMovePoints() / fighter.GetMaxMovePoints() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateStatuses(Fighter fighter)
        {
            int maxStatusesContainers = _uiDoc.rootVisualElement.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME).childCount;
            Dictionary<AStatus, (bool isActive, int duration)> currentFighterStatuses = fighter.GetStatuses();
            for (int i = 1; i <= currentFighterStatuses.Count; i++)
            {
                if (i > maxStatusesContainers)
                {
                    break;
                }
                VisualElement statusContainer = _uiDoc.rootVisualElement.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = new(currentFighterStatuses.ElementAt(i - 1).Key.Icon);
            }

            for (int i = currentFighterStatuses.Count + 1; i <= maxStatusesContainers; i++)
            {
                VisualElement statusContainer = _uiDoc.rootVisualElement.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = null;
            }
        }

        private void UpdateFighterIcons(Fighter playingFighter)
        {
            // Get UI elements
            VisualElement playingFighterIconContainer = _uiDoc.rootVisualElement.Q<VisualElement>(PLAYING_FIGHTER_ICON_CONTAINER_UI_NAME);
            VisualElement mate1IconContainer = _uiDoc.rootVisualElement.Q<VisualElement>(MATE1_ICON_CONTAINER_UI_NAME);
            VisualElement mate2IconContainer = _uiDoc.rootVisualElement.Q<VisualElement>(MATE2_ICON_CONTAINER_UI_NAME);

            // Update playing fighter icon & fighter details panel
            playingFighterIconContainer.style.backgroundImage = new(playingFighter.DiamondIcon);
            playingFighterIconContainer.RegisterCallback<MouseOverEvent>(evt =>
                {
                    Vector2Int panelDisplayPosition = new(
                        (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.x,
                        (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.y - _fighterDetailsPanelYOffset
                    );
                    _fighterDetailsPanelController.Display(playingFighter, panelDisplayPosition);
                }
            );
            playingFighterIconContainer.RegisterCallback<MouseOutEvent>(
                evt => _fighterDetailsPanelController.Hide()
            );

            // Set mate icons to null by default
            mate1IconContainer.style.backgroundImage = null;
            mate2IconContainer.style.backgroundImage = null;

            Fighter[] mates = _fightManager.GetMatesOfFighter(playingFighter);
            if (mates.Length > 0)
            {
                // Update mate 1 icon & fighter details panel
                mate1IconContainer.style.backgroundImage = new(mates[0].DiamondIcon);
                mate1IconContainer.RegisterCallback<MouseOverEvent>(evt =>
                    {
                        Vector2Int panelDisplayPosition = new(
                            (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.x,
                            (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.y - _fighterDetailsPanelYOffset
                        );
                        _fighterDetailsPanelController.Display(mates[0], panelDisplayPosition);
                    }
                );
                mate1IconContainer.RegisterCallback<MouseOutEvent>(
                    evt => _fighterDetailsPanelController.Hide()
                );
            }
            if (mates.Length > 1)
            {
                // Update mate 2 icon & fighter details panel
                mate2IconContainer.style.backgroundImage = new(mates[1].DiamondIcon);
                mate2IconContainer.RegisterCallback<MouseOverEvent>(evt =>
                    {
                        Vector2Int panelDisplayPosition = new(
                            (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.x,
                            (int)_uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).layout.y - _fighterDetailsPanelYOffset
                        );
                        _fighterDetailsPanelController.Display(mates[1], panelDisplayPosition);
                    }
                );
                mate2IconContainer.RegisterCallback<MouseOutEvent>(
                    evt => _fighterDetailsPanelController.Hide()
                );
            }
        }

        private void UpdateAbilityButtons(Fighter fighter)
        {
            _buttonToActiveAbility.Clear();
            int abilitiesButtonCount = GetAbilitiesButtons().Length;

            int i = 1;
            fighter.ActiveAbilities.ToList().ForEach(ability =>
            {
                if (i <= abilitiesButtonCount)
                {
                    SetupAbilityButton(ability, i);
                }
                else
                {
                    Debug.LogWarning($"Not enough abilities buttons for fighter {fighter.name}");
                }
                i++;
            });
        }

        private void SetupAbilityButton(ActiveAbilitySO activeAbility, int slotIndex)
        {
            Button abilityButton = _uiDoc.rootVisualElement.Q<Button>($"{ABILITY_BUTTON_UI_NAME}{slotIndex}");
            abilityButton.style.backgroundImage = new(activeAbility.IconSprite);
            _buttonToActiveAbility.Add(abilityButton, activeAbility);
        }

        private void OnFighterTurnEnded(Fighter currentFighter, bool isAlly)
        {
            SetIsVisible(!isAlly);
        }

        private void OnDirectAttackButtonClicked(ClickEvent _clickEvent)
        {
            onDirectAttackClicked?.Invoke();
        }

        private void OnActiveAbilityButtonClicked(ClickEvent clickEvent)
        {
            if (clickEvent.currentTarget is Button abilityButton && _buttonToActiveAbility.ContainsKey(abilityButton))
            {
                onActiveAbilityClicked?.Invoke(_buttonToActiveAbility[abilityButton]);
            }
        }

        private void OnEndTurnButtonClicked(ClickEvent _clickEvent)
        {
            onEndTurnClicked?.Invoke();
        }

        private Button[] GetAbilitiesButtons()
        {
            List<Button> abilitiesButtons = new();

            int i = 1;
            VisualElement abilitiesPanel = _uiDoc.rootVisualElement.Q("AbilitiesPanel");
            while (i < abilitiesPanel.childCount)
            {
                abilitiesButtons.Add(abilitiesPanel.Q<Button>($"{ABILITY_BUTTON_UI_NAME}{i}"));
                i++;
            }

            return abilitiesButtons.ToArray();
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_uiDoc == null)
            {
                _uiDoc = GetComponent<UIDocument>();
            }
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (_fightManager == null)
            {
                _fightManager = FindObjectOfType<FightManager>();
            }
            if (_fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            Button directAttackButton = _uiDoc.rootVisualElement.Q<Button>(DIRECT_ATTACK_BUTTON_UI_NAME);
            Button endTurnButton = _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME);

            directAttackButton.RegisterCallback<ClickEvent>(OnDirectAttackButtonClicked);
            GetAbilitiesButtons().ToList().ForEach(
                abilityButton => abilityButton.RegisterCallback<ClickEvent>(OnActiveAbilityButtonClicked)
            );
            endTurnButton.RegisterCallback<ClickEvent>(OnEndTurnButtonClicked);

            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
            _fightManager.onFighterTurnEnded += OnFighterTurnEnded;
        }

        private void OnDisable()
        {
            if (_uiDoc != null && _uiDoc.rootVisualElement != null)
            {
                Button directAttackButton = _uiDoc.rootVisualElement.Q<Button>(DIRECT_ATTACK_BUTTON_UI_NAME);
                Button endTurnButton = _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME);

                directAttackButton.UnregisterCallback<ClickEvent>(OnDirectAttackButtonClicked);
                GetAbilitiesButtons().ToList().ForEach(
                    abilityButton => abilityButton.UnregisterCallback<ClickEvent>(OnActiveAbilityButtonClicked)
                );
                endTurnButton.UnregisterCallback<ClickEvent>(OnEndTurnButtonClicked);
            }
            if (_fightManager != null)
            {
                _fightManager.onFighterTurnBegan -= OnFighterTurnBegan;
                _fightManager.onFighterTurnEnded -= OnFighterTurnEnded;
            }
        }

        #endregion
    }
}