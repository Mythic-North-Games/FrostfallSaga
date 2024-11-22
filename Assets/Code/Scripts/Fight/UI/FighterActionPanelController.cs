using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Abilities;

namespace FrostfallSaga.Fight.UI
{
    public class FighterActionPanelController : BaseUIController
    {
        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onActiveAbilityClicked;
        public Action onEndTurnClicked;

        [SerializeField] private FightManager _fightManager;
        private readonly Dictionary<Button, ActiveAbilitySO> _buttonToActiveAbility = new();

        private static readonly string ACTION_PANEL_ROOT = "RootActionPanel";
        private static readonly string ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string DIRECT_ATTACK_BUTTON_UI_NAME = "AbilityButton0";
        private static readonly string END_TURN_BUTTON_UI_NAME = "EndTurnButton";        

        /// <summary>
        /// Displays or hides the entire action panel.
        /// </summary>
        /// <param name="isVisible">True to display, False to hide.</param>
        public void SetIsVisible(bool isVisible)
        {
            _uiDoc.rootVisualElement.Q(ACTION_PANEL_ROOT).style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnFighterTurnBegan(Fighter currentFighter, bool isAlly)
        {
            SetIsVisible(isAlly);
            if (isAlly)
            {
                UpdateActionPanelForFighter(currentFighter);
            }
        }

        private void UpdateActionPanelForFighter(Fighter fighter)
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