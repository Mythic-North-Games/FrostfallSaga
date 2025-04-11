using FrostfallSaga.Core.Fight;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

<<<<<<< Updated upstream:Assets/Code/Scripts/AbilitySystem/UI/AbilityVisualizerUIController.cs
namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityVisualizerUIController
    {
      
=======
namespace FrostfallSaga {
    public class AbilityVisualizerUIController {
        #region UI Element Names
        private static readonly string ABILITY_ICON_UI_NAME = "abilityIcon";
        private static readonly string ABILITY_NAME_UI_NAME = "abilityName";
        private static readonly string ABILITY_COST_UI_NAME = "abilityCost";
        private static readonly string ABILITY_UNLOCK_BUTTON_UI_NAME = "abilityUnlockButton";
        private static readonly string ABILITY_DESCRIPTION_UI_NAME = "abilityDescription";
        #endregion

        /// <summary>
        /// Root visual element of the Ability Visualizer.
        /// </summary>
        public VisualElement Root {
            get; private set;
        }

        private readonly VisualElement _abilityIcon;
        private readonly Label _abilityName;
        private readonly Label _abilityCost;
        private readonly Button _abilityUnlockButton;
        private readonly Label _abilityDescription;

        /// <summary>
        /// Gets the currently associated ability.
        /// </summary>
        public ABaseAbility CurrentAbility => _currentAbility;
        private ABaseAbility _currentAbility;

        /// <summary>
        /// Initializes the ability visualizer and retrieves UI elements by name.
        /// </summary>
        /// <param name="root">Root visual element for the ability UI.</param>
        public AbilityVisualizerUIController(VisualElement root) {
            Root = root;
            _abilityIcon = Root.Q<VisualElement>(ABILITY_ICON_UI_NAME);
            _abilityName = Root.Q<Label>(ABILITY_NAME_UI_NAME);
            _abilityCost = Root.Q<Label>(ABILITY_COST_UI_NAME);
            _abilityUnlockButton = Root.Q<Button>(ABILITY_UNLOCK_BUTTON_UI_NAME);
            _abilityDescription = Root.Q<Label>(ABILITY_DESCRIPTION_UI_NAME);

            // Add callback for the unlock button click event
            if (_abilityUnlockButton != null) {
                _abilityUnlockButton.clicked += OnUnlockButtonClicked;
            }
        }

        /// <summary>
        /// Sets the visual display for the ability.
        /// </summary>
        /// <param name="ability">The ability to display.</param>
        public void SetAbilityVisualyzer(ABaseAbility ability) {
            _currentAbility = ability;
            bool abilityIsNull = ability == null;
            _abilityIcon.style.backgroundImage = abilityIsNull ? null : new StyleBackground(ability.IconSprite);
            _abilityName.text = abilityIsNull ? string.Empty : ability.Name;
            _abilityCost.text = abilityIsNull ? string.Empty : ability.UnlockPoints.ToString();
            _abilityDescription.text = abilityIsNull ? string.Empty : ability.Description;
        }

        /// <summary>
        /// Updates the style of the unlock button based on the ability's state.
        /// For example, it can apply "lock", "unlockable", or "unlock" styles.
        /// </summary>
        /// <param name="state">The visual state to apply.</param>
        public void SetAbilityStatusStyle(string state) {
            _abilityUnlockButton.RemoveFromClassList("lock");
            _abilityUnlockButton.RemoveFromClassList("unlockable");
            _abilityUnlockButton.RemoveFromClassList("unlock");
            _abilityUnlockButton.AddToClassList(state);
        }

        /// <summary>
        /// Callback triggered when the unlock button is clicked.
        /// TODO: Implement the unlock functionality for the tree ability.
        /// </summary>
        private void OnUnlockButtonClicked() {
            // TODO: Implement the unlock function for the tree ability.
            Debug.Log("Unlock button clicked. TODO: implement unlock logic.");
        }
>>>>>>> Stashed changes:Assets/Code/Scripts/AbilitySystem/AbilityVisualizerUIController.cs
    }
}
