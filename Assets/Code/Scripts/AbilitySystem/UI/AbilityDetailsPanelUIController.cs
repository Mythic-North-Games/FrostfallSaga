using System;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityDetailsPanelUIController
    {
        #region UXML Element names & classes
        private static readonly string CONTENT_CONTAINER_UI_NAME = "AbilityDetailsContainer";
        private static readonly string ABILITY_DETAILS_CONTAINER_UI_NAME = "ObjectDetailsContainer";
        private static readonly string ABILITY_POINTS_COST_CONTAINER_UI_NAME = "AbilityPointsCostContainer";
        private static readonly string ABILITY_POINTS_COST_LABEL_UI_NAME = "AbilityPointsCostLabel";
        private static readonly string UNLOCK_BUTTON_UI_NAME = "UnlockButton";
        private static readonly string UNLOCK_BUTTON_FILLED_UI_NAME = "UnlockButtonFilled";

        private static readonly string UNLOCK_BUTTON_LOCKED_CLASSNAME = "unlockButtonLocked";
        private static readonly string ABILITY_POINTS_LABEL_LOCKED_CLASSNAME = "abilityPointsCostLabelLocked";
        #endregion

        public Action<ABaseAbility> onUnlockButtonClicked;

        private readonly VisualElement _contentContainer;
        private readonly VisualElement _abilityPointsCostContainer;
        private readonly Label _abilityPointsCostLabel;
        private readonly Button _unlockButton;
        private readonly Button _unlockButtonFilled;

        private readonly ObjectDetailsUIController _objectDetailsUIController;
        private ABaseAbility _currentAbility;
        private EAbilityState _currentAbilityState;
        private int _currentFighterAbilityPoints;

        private readonly CoroutineRunner _coroutineRunner;

        public AbilityDetailsPanelUIController(
            VisualElement root,
            VisualTreeAsset statContainerTemplate,
            string effectLineClassname,
            Color statValueColor,
            Color statIconTitlColor
        )
        {
            _contentContainer = root.Q(CONTENT_CONTAINER_UI_NAME);
            _abilityPointsCostContainer = root.Q(ABILITY_POINTS_COST_CONTAINER_UI_NAME);
            _abilityPointsCostLabel = root.Q<Label>(ABILITY_POINTS_COST_LABEL_UI_NAME);
            _unlockButton = root.Q<Button>(UNLOCK_BUTTON_UI_NAME);
            _unlockButton.RegisterCallback<ClickEvent>(OnUnlockButtonClicked);
            _unlockButtonFilled = root.Q<Button>(UNLOCK_BUTTON_FILLED_UI_NAME);
            _unlockButtonFilled.RegisterCallback<ClickEvent>(OnUnlockButtonClicked);

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

            _coroutineRunner = CoroutineRunner.Instance;
        }

        public void UpdatePanel(ABaseAbility ability, EAbilityState abilityState, int fighterAbilityPoints)
        {
            if (ability == null)
            {
                _contentContainer.style.display = DisplayStyle.None;
                return;
            }
            _currentAbility = ability;
            _currentAbilityState = abilityState;
            _currentFighterAbilityPoints = fighterAbilityPoints;

            _objectDetailsUIController.Setup(ability);

            _abilityPointsCostContainer.visible = abilityState != EAbilityState.Unlocked;
            _abilityPointsCostLabel.EnableInClassList(ABILITY_POINTS_LABEL_LOCKED_CLASSNAME, fighterAbilityPoints < ability.UnlockPoints);
            _abilityPointsCostLabel.text = ability.UnlockPoints.ToString();

            bool canUnlock = abilityState == EAbilityState.Unlockable && fighterAbilityPoints >= ability.UnlockPoints;
            _unlockButton.EnableInClassList(UNLOCK_BUTTON_LOCKED_CLASSNAME, !canUnlock);
            _unlockButton.text = abilityState.GetUIString();

            _unlockButtonFilled.style.display = canUnlock ? DisplayStyle.Flex : DisplayStyle.None;
            _unlockButtonFilled.text = abilityState.GetUIString();

            _contentContainer.style.display = DisplayStyle.Flex;
        }

        private void OnUnlockButtonClicked(ClickEvent evt)
        {
            if (_currentAbilityState != EAbilityState.Unlockable || _currentFighterAbilityPoints < _currentAbility.UnlockPoints)
            {
                _coroutineRunner.StartCoroutine(CommonUIAnimations.PlayShakeAnimation(_unlockButton));
                if (_currentAbilityState == EAbilityState.Unlockable)
                {
                    _coroutineRunner.StartCoroutine(
                        CommonUIAnimations.PlayScaleAnimation(_abilityPointsCostContainer, new(1.25f, 1.25f), 1f)
                    );
                }
                return;
            }
            onUnlockButtonClicked?.Invoke(_currentAbility);
        }
    }
}
