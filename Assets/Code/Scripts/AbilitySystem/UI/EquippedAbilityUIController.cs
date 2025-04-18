using FrostfallSaga.Core.Fight;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class EquippedAbilityUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_ICON_UI_NAME = "Icon";
        private static readonly string ABILITY_HOVER_UI_NAME = "IconHover";
        #endregion

        private readonly VisualElement _abilityIcon;
        private readonly VisualElement _abilityHover;

        public ABaseAbility CurrentAbility { get; private set; }

        public EquippedAbilityUIController(VisualElement root)
        {
            _abilityIcon = root.Q(ABILITY_ICON_UI_NAME);
            _abilityHover = root.Q(ABILITY_HOVER_UI_NAME);
        }

        public void SetAbility(ABaseAbility ability)
        {
            CurrentAbility = ability;

            if (ability == null)
            {
                _abilityIcon.style.backgroundImage = null;
                _abilityHover.SetEnabled(false);
                _abilityHover.visible = false;
                return;
            }
            _abilityIcon.style.backgroundImage = new StyleBackground(ability.Icon);
            _abilityHover.SetEnabled(true);
            _abilityHover.visible = true;
        }
    }
}
