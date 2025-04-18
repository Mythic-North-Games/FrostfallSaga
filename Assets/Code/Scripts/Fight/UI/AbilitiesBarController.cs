using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    ///     Controller for the fighter statuses bars in the UI.
    /// </summary>
    public class AbilitiesBarController
    {
        private readonly Dictionary<Button, ActiveAbilitySO> _abilitiesButtons = new();

        private readonly Button _directAttackButton;
        public Action<ActiveAbilitySO> onAbilityClicked;
        public Action<ActiveAbilitySO> onAbilityLongHovered;
        public Action<ActiveAbilitySO> onAbilityLongUnhovered;

        public Action onDirectAttackClicked;
        public Action onDirectAttackLongHovered;
        public Action onDirectAttackLongUnhovered;

        public AbilitiesBarController(VisualElement abilitiesBarRoot)
        {
            // Setup direct attack button
            _directAttackButton = abilitiesBarRoot.Q<Button>(DIRECT_ATTACK_BUTTON_UI_NAME);

            // Setup direct attack button events
            _directAttackButton.RegisterCallback<ClickEvent>(_ => onDirectAttackClicked?.Invoke());
            LongHoverEventController<Button> directAttcklongHoverEventController = new(_directAttackButton);
            directAttcklongHoverEventController.onElementLongHovered += OnDirectAttackLongHovered;
            directAttcklongHoverEventController.onElementLongUnhovered += OnDirectAttackLongUnhovered;

            // Setup ability buttons
            for (int i = 0; i < abilitiesBarRoot.childCount - 1; i++)
            {
                Button abilityButton = abilitiesBarRoot.Q<Button>(ABILITY_BUTTON_BASE_UI_NAME + i);
                _abilitiesButtons.Add(abilityButton, null);

                // Setup ability button events
                abilityButton.RegisterCallback<ClickEvent>(OnAbilityClicked);
                LongHoverEventController<Button> abilityLongHoverEventController = new(abilityButton);
                abilityLongHoverEventController.onElementLongHovered += OnAbilityLongHovered;
                abilityLongHoverEventController.onElementLongUnhovered += OnAbilityLongUnhovered;
            }
        }

        public void UpdateAbilities(Fighter fighter)
        {
            // Setup direct attack button
            VisualElement directAttackIcon = _directAttackButton.Q<VisualElement>(ABILITIES_ICON_UI_NAME);
            directAttackIcon.style.backgroundImage = new(fighter.Weapon.IconSprite);

            // Set enabled or not depending on action points
            bool canUseDirectAttack = fighter.GetActionPoints() >= fighter.Weapon.UseActionPointsCost;
            _directAttackButton.SetEnabled(canUseDirectAttack);

            // Setup abilities buttons
            for (int i = 0; i < _abilitiesButtons.Count; i++)
            {
                Button abilityButton = _abilitiesButtons.Keys.ElementAt(i);
                VisualElement abilityIcon = abilityButton.Q<VisualElement>(ABILITIES_ICON_UI_NAME);

                // Set ability icon
                if (i < fighter.ActiveAbilities.Length)
                {
                    abilityIcon.style.backgroundImage = new(fighter.ActiveAbilities[i].Icon);
                }
                else
                {
                    abilityIcon.style.backgroundImage = null;
                    abilityButton.pickingMode = PickingMode.Ignore;
                    abilityButton.Children().ToList().ForEach(child => child.pickingMode = PickingMode.Ignore);
                }

                bool canUseAbility =
                    i < fighter.ActiveAbilities.Length &&
                    fighter.GetActionPoints() >= fighter.ActiveAbilities[i].ActionPointsCost;

                // Set enabled or not
                abilityButton.SetEnabled(canUseAbility);

                // Bind active ability to button
                _abilitiesButtons[abilityButton] = canUseAbility ? fighter.ActiveAbilities[i] : null;
            }
        }

        private void OnAbilityClicked(ClickEvent evt)
        {
            Button abilityButton = (Button)evt.target;
            if (_abilitiesButtons.TryGetValue(abilityButton, out ActiveAbilitySO ability) && ability != null)
            {
                onAbilityClicked?.Invoke(ability);
            }
        }

        private void OnDirectAttackLongHovered(Button directAttackButton)
        {
            onDirectAttackLongHovered?.Invoke();
        }

        private void OnDirectAttackLongUnhovered(Button directAttackButton)
        {
            onDirectAttackLongUnhovered?.Invoke();
        }

        private void OnAbilityLongHovered(Button abilityButton)
        {
            if (_abilitiesButtons.TryGetValue(abilityButton, out ActiveAbilitySO ability) && ability != null)
            {
                onAbilityLongHovered?.Invoke(ability);
            }
        }

        private void OnAbilityLongUnhovered(Button abilityButton)
        {
            if (_abilitiesButtons.TryGetValue(abilityButton, out ActiveAbilitySO ability) && ability != null)
            {
                onAbilityLongUnhovered?.Invoke(ability);
            }
        }

        #region UXML UI Names & Classes

        private static readonly string DIRECT_ATTACK_BUTTON_UI_NAME = "DirectAttackButton";
        private static readonly string ABILITY_BUTTON_BASE_UI_NAME = "AbilityButton";
        private static readonly string ABILITIES_ICON_UI_NAME = "Icon";

        #endregion
    }
}