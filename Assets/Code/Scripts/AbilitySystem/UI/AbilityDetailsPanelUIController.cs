using System;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.InventorySystem.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityDetailsPanelUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_DETAILS_CONTAINER_UI_NAME = "ObjectDetailsContainer";
        private static readonly string ABILITY_POINTS_COST_CONTAINER_UI_NAME = "AbilityPointsCostContainer";
        private static readonly string ABILITY_POINTS_TITLE_LABEL_UI_NAME = "AbilityPointsTitleLabel";
        private static readonly string ABILITY_POINTS_COST_LABEL_UI_NAME = "AbilityPointsCostLabel";
        private static readonly string UNLOCK_BUTTON_UI_NAME = "UnlockButton";

        private static readonly string UNLOCK_BUTTON_LOCKED_CLASS_NAME = "unlockButtonLocked";
        private static readonly string ABILITY_POINTS_LABEL_LOCKED_CLASS_NAME = "abilityPointsLabelLocked";
        #endregion

        public Action<ABaseAbility> onUnlockButtonClicked;

        private readonly VisualElement _root;
        private readonly VisualElement _abilityPointsCostContainer;
        private readonly Label _abilityPointsTitleLabel;
        private readonly Label _abilityPointsCostLabel;
        private readonly Button _unlockButton;

        private readonly ObjectDetailsUIController _objectDetailsUIController;
        private ABaseAbility _currentAbility;
        private EAbilityState _currentAbilityState;

        public AbilityDetailsPanelUIController(
            VisualElement root,
            VisualTreeAsset statContainerTemplate,
            string effectLineClassname,
            Color statValueColor,
            Color statIconTitlColor
        )
        {
            _root = root;
            _abilityPointsCostContainer = root.Q(ABILITY_POINTS_COST_CONTAINER_UI_NAME);
            _abilityPointsTitleLabel = root.Q<Label>(ABILITY_POINTS_TITLE_LABEL_UI_NAME);
            _abilityPointsCostLabel = root.Q<Label>(ABILITY_POINTS_COST_LABEL_UI_NAME);
            _unlockButton = root.Q<Button>(UNLOCK_BUTTON_UI_NAME);
            _unlockButton.RegisterCallback<ClickEvent>(OnUnlockButtonClicked);

            VisualElement abilityDetailsContainer = root.Q(ABILITY_DETAILS_CONTAINER_UI_NAME);
            _objectDetailsUIController = new ObjectDetailsUIController(
                abilityDetailsContainer,
                statContainerTemplate,
                effectLineClassname,
                statValueColor,
                statIconTitlColor
            );

            _currentAbility = null;
            _currentAbilityState = EAbilityState.Locked;
        }

        public void UpdatePanel(ABaseAbility ability, EAbilityState abilityState, int fighterAbilityPoints)
        {
            if (ability == null)
            {
                _root.style.display = DisplayStyle.None;
                return;
            }
            _currentAbility = ability;
            _currentAbilityState = abilityState;

            _objectDetailsUIController.Setup(
                icon: ability.Icon,
                name: ability.Name,
                description: ability.Description,
                stats: ability.GetStatsUIData(),
                primaryEffectsTitle: "Effects",
                primaryEffects: ability.GetEffectsUIData(),
                secondaryEffectsTitle: "Masterstroke Effects",
                secondaryEffects: ability.GetMasterstrokeEffectsUIData()
            );

            _abilityPointsCostContainer.visible = abilityState != EAbilityState.Unlocked;
            _abilityPointsTitleLabel.EnableInClassList(ABILITY_POINTS_LABEL_LOCKED_CLASS_NAME, fighterAbilityPoints < ability.UnlockPoints);
            _abilityPointsCostLabel.EnableInClassList(ABILITY_POINTS_LABEL_LOCKED_CLASS_NAME, fighterAbilityPoints < ability.UnlockPoints);
            _abilityPointsCostLabel.text = $"{fighterAbilityPoints}/{ability.UnlockPoints}";

            bool canUnlock = abilityState == EAbilityState.Unlockable && fighterAbilityPoints >= ability.UnlockPoints;
            _unlockButton.EnableInClassList(UNLOCK_BUTTON_LOCKED_CLASS_NAME, !canUnlock);
            _unlockButton.SetEnabled(canUnlock);
            _unlockButton.text = abilityState.GetUIString();

            _root.style.display = DisplayStyle.Flex;
        }

        private void OnUnlockButtonClicked(ClickEvent evt)
        {
            if (_currentAbilityState != EAbilityState.Unlockable)
            {
                return;
            }
            onUnlockButtonClicked?.Invoke(_currentAbility);
        }
    }
}
