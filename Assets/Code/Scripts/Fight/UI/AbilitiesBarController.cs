using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    ///     Controller for the fighter statuses bars in the UI.
    /// </summary>
    public class AbilitiesBarController
    {
        #region UXML UI Names & Classes
        private static readonly string DIRECT_ATTACK_BUTTON_UI_NAME = "DirectAttackButton";
        private static readonly string ABILITY_BUTTON_BASE_UI_NAME = "AbilityButton";
        private static readonly string ABILITIES_ICON_UI_NAME = "Icon";
        #endregion

        public Action onDirectAttackClicked;
        public Action<ActiveAbilitySO> onAbilityButtonClicked;

        private readonly Button _directAttackButton;
        private readonly Dictionary<Button, ActiveAbilitySO> _abilitiesButtons = new();

        public AbilitiesBarController(VisualElement abilitiesBarRoot)
        {
            _directAttackButton = abilitiesBarRoot.Q<Button>(DIRECT_ATTACK_BUTTON_UI_NAME);
            _directAttackButton.RegisterCallback<ClickEvent>(_ => onDirectAttackClicked?.Invoke());
            for (int i = 0; i < abilitiesBarRoot.childCount - 1; i++)
            {
                Button abilityButton = abilitiesBarRoot.Q<Button>(ABILITY_BUTTON_BASE_UI_NAME + i);
                abilityButton.RegisterCallback<ClickEvent>(OnAbilityButtonClicked);
                _abilitiesButtons.Add(abilityButton, null);
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
            _directAttackButton.pickingMode = canUseDirectAttack ? PickingMode.Position : PickingMode.Ignore;
            _directAttackButton.Children().ToList().ForEach(
                child => child.pickingMode = canUseDirectAttack ? PickingMode.Position : PickingMode.Ignore
            );

            // Setup abilities buttons
            for (int i = 0; i < _abilitiesButtons.Count; i++)
            {
                Button abilityButton = _abilitiesButtons.Keys.ElementAt(i);
                VisualElement abilityIcon = abilityButton.Q<VisualElement>(ABILITIES_ICON_UI_NAME);

                // Set ability icon
                if (i < fighter.ActiveAbilities.Length)
                {
                    abilityIcon.style.backgroundImage = new(fighter.ActiveAbilities[i].IconSprite);
                }
                else
                {
                    abilityIcon.style.backgroundImage = null;
                }

                bool canUseAbility =
                    i < fighter.ActiveAbilities.Length &&
                    fighter.GetActionPoints() >= fighter.ActiveAbilities[i].ActionPointsCost;

                // Set enabled or not
                abilityButton.SetEnabled(canUseAbility);
                abilityButton.pickingMode = canUseAbility ? PickingMode.Position : PickingMode.Ignore;
                abilityButton.Children().ToList().ForEach(
                    child => child.pickingMode = canUseAbility ? PickingMode.Position : PickingMode.Ignore
                );

                // Bind active ability to button
                _abilitiesButtons[abilityButton] = canUseAbility ? fighter.ActiveAbilities[i] : null;
            }
        }

        private void OnAbilityButtonClicked(ClickEvent evt)
        {
            Button abilityButton = (Button)evt.target;
            if (_abilitiesButtons.TryGetValue(abilityButton, out ActiveAbilitySO ability) && ability != null)
            {
                onAbilityButtonClicked?.Invoke(ability);
            }
        }
    }
}