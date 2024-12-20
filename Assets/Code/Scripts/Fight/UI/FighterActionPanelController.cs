using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        private static readonly string NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string PROGRESS_BARS_CONTAINER_UI_NAME = "ProgressBarsContainer";
        private static readonly string STATUSES_BAR_CONTAINER_UI_NAME = "StatusesBar";
        private static readonly string PLAYING_CHARACTER_ICON_CONTAINER_UI_NAME = "PlayingCharacterIconContainer";
        private static readonly string MATE1_ICON_CONTAINER_UI_NAME = "TeamMateIconContainer1";
        private static readonly string MATE2_ICON_CONTAINER_UI_NAME = "TeamMateIconContainer2";
        private static readonly string ACTION_PANEL_ROOT_UI_NAME = "ActionPanel";
        private static readonly string ABILITIES_CONTAINER_UI_NAME = "AbilitiesContainer";
        private static readonly string ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string DIRECT_ATTACK_BUTTON_UI_NAME = "DirectAttackButton";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";
        private static readonly string ABILITY_UNUSABLE_CLASS_NAME = "abilitiesUnusable";

        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action onEndTurnClicked;

        [SerializeField] private FightManager _fightManager;
        private FighterProgressBarsController _progressBarsController;
        private FighterStatusesBarController _statusesBarController;
        private readonly Dictionary<Button, ActiveAbilitySO> _buttonToActiveAbility = new();

        /// <summary>
        /// Displays or hides the entire action panel.
        /// </summary>
        /// <param name="isVisible">True to display, False to hide.</param>
        public void SetIsVisible(bool isVisible)
        {
            _uiDoc.rootVisualElement.Q(ACTION_PANEL_ROOT_UI_NAME).style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            _uiDoc.rootVisualElement.Q(END_TURN_BUTTON_UI_NAME).style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnFighterTurnBegan(Fighter currentFighter, bool isAlly)
        {
            SetIsVisible(isAlly);
            if (isAlly)
            {
                RegisterFighterEvents(currentFighter);
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

            _progressBarsController.UpdateHealthBar(fighter);
            _progressBarsController.UpdateActionBar(fighter);
            _progressBarsController.UpdateMoveBar(fighter);
            _statusesBarController.UpdateStatuses(fighter);
        }

        private void UpdateFighterIcons(Fighter playingFighter)
        {
            // Get UI elements
            VisualElement playingFighterIconContainer = _uiDoc.rootVisualElement.Q(PLAYING_CHARACTER_ICON_CONTAINER_UI_NAME).Q("WhiteDiamondBackground");
            VisualElement mate1IconContainer = _uiDoc.rootVisualElement.Q(MATE1_ICON_CONTAINER_UI_NAME).Q("WhiteDiamondBackground");
            VisualElement mate2IconContainer = _uiDoc.rootVisualElement.Q(MATE2_ICON_CONTAINER_UI_NAME).Q("WhiteDiamondBackground");

            // Update playing fighter icon
            playingFighterIconContainer.style.backgroundImage = new(playingFighter.DiamondIcon);

            // Set mate icons to null by default
            mate1IconContainer.style.backgroundImage = null;
            mate2IconContainer.style.backgroundImage = null;

            Fighter[] mates = _fightManager.GetMatesOfFighter(playingFighter);
            if (mates.Length > 0) mate1IconContainer.style.backgroundImage = new(mates[0].DiamondIcon);
            if (mates.Length > 1) mate2IconContainer.style.backgroundImage = new(mates[1].DiamondIcon);
        }

        private void UpdateAbilityButtons(Fighter fighter)
        {
            _buttonToActiveAbility.Clear();
            int abilitiesButtonCount = GetAbilitiesButtons().Length;

            int i = 0;
            fighter.ActiveAbilities.ToList().ForEach(ability =>
            {
                if (i <= abilitiesButtonCount)
                {
                    SetupAbilityButton(fighter, ability, i);
                }
                else
                {
                    Debug.LogWarning($"Not enough abilities buttons for fighter {fighter.name}");
                }
                i++;
            });
        }

        private void SetupAbilityButton(Fighter fighter, ActiveAbilitySO activeAbility, int slotIndex)
        {
            Button abilityButton = _uiDoc.rootVisualElement.Q<Button>($"{ABILITY_BUTTON_UI_NAME}{slotIndex}");
            abilityButton.style.backgroundImage = new(activeAbility.IconSprite);
            if (!fighter.CanUseActiveAbility(_fightManager.FightGrid, activeAbility, _fightManager.FighterTeams))
            {
                abilityButton.AddToClassList(ABILITY_UNUSABLE_CLASS_NAME);
                abilityButton.SetEnabled(false);
            }
            else
            {
                abilityButton.RemoveFromClassList(ABILITY_UNUSABLE_CLASS_NAME);
                abilityButton.SetEnabled(true);
            }
            _buttonToActiveAbility.Add(abilityButton, activeAbility);
        }

        private void OnFighterTurnEnded(Fighter currentFighter, bool isAlly)
        {
            SetIsVisible(!isAlly);
            if (isAlly)
            {
                UnregisterFighterEvents(currentFighter);
            }
        }

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            Destroy(_uiDoc);
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

            VisualElement abilitiesContainer = _uiDoc.rootVisualElement.Q(ABILITIES_CONTAINER_UI_NAME);
            for (int i = 0; i < abilitiesContainer.childCount; i++)
            {
                abilitiesButtons.Add(abilitiesContainer.Q<Button>($"{ABILITY_BUTTON_UI_NAME}{i}"));
                i++;
            }

            return abilitiesButtons.ToArray();
        }

        private void RegisterFighterEvents(Fighter fighter)
        {
            fighter.onDamageReceived += (fighter, damage, isMasterstroke) => _progressBarsController.UpdateHealthBar(fighter);
            fighter.onHealReceived += (fighter, damage, isMasterstroke) => _progressBarsController.UpdateHealthBar(fighter);
            fighter.onDirectAttackStarted += (fighter) =>
            {
                _progressBarsController.UpdateActionBar(fighter);
                UpdateAbilityButtons(fighter);
            };
            fighter.onDirectAttackEnded += (fighter) =>
            {
                _progressBarsController.UpdateActionBar(fighter);
                UpdateAbilityButtons(fighter);
            };
            fighter.onActiveAbilityStarted += (fighter, usedAbility) =>
            {
                _progressBarsController.UpdateActionBar(fighter);
                UpdateAbilityButtons(fighter);
            };
            fighter.onActiveAbilityEnded += (fighter, usedAbility) =>
            {
                _progressBarsController.UpdateActionBar(fighter);
                UpdateAbilityButtons(fighter);
            };
            fighter.onFighterMoved += (fighter) => _progressBarsController.UpdateMoveBar(fighter);
            fighter.onStatusApplied += (fighter, status) => _statusesBarController.UpdateStatuses(fighter);
            fighter.onStatusRemoved += (fighter, status) => _statusesBarController.UpdateStatuses(fighter);
            fighter.onNonMagicalStatMutated += (fighter, mutatedStat, amount) =>
            {
                _progressBarsController.UpdateHealthBar(fighter);
                _progressBarsController.UpdateActionBar(fighter);
                _progressBarsController.UpdateMoveBar(fighter);
                UpdateAbilityButtons(fighter);
            };
        }

        private void UnregisterFighterEvents(Fighter fighter)
        {
            fighter.onDamageReceived = null;
            fighter.onHealReceived = null;
            fighter.onDirectAttackStarted = null;
            fighter.onDirectAttackEnded = null;
            fighter.onActiveAbilityStarted = null;
            fighter.onActiveAbilityEnded = null;
            fighter.onFighterMoved = null;
            fighter.onStatusApplied = null;
            fighter.onStatusRemoved = null;
            fighter.onNonMagicalStatMutated = null;
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

            _progressBarsController = new FighterProgressBarsController(_uiDoc.rootVisualElement.Q(PROGRESS_BARS_CONTAINER_UI_NAME));
            _statusesBarController = new FighterStatusesBarController(_uiDoc.rootVisualElement.Q(STATUSES_BAR_CONTAINER_UI_NAME));

            VisualElement directAttackButton = _uiDoc.rootVisualElement.Q<VisualElement>(DIRECT_ATTACK_BUTTON_UI_NAME);
            Button endTurnButton = _uiDoc.rootVisualElement.Q<Button>(END_TURN_BUTTON_UI_NAME);

            directAttackButton.RegisterCallback<ClickEvent>(OnDirectAttackButtonClicked);
            GetAbilitiesButtons().ToList().ForEach(
                abilityButton => abilityButton.RegisterCallback<ClickEvent>(OnActiveAbilityButtonClicked)
            );
            endTurnButton.RegisterCallback<ClickEvent>(OnEndTurnButtonClicked);

            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
            _fightManager.onFighterTurnEnded += OnFighterTurnEnded;
            _fightManager.onFightEnded += OnFightEnded;
        }

        #endregion
    }
}