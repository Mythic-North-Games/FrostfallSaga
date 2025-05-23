using System.Collections;
using FrostfallSaga.Core.Fight;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class EquippedAbilityUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_BUTTON_UI_NAME = "EqquipedAbility";
        private static readonly string ABILITY_ICON_UI_NAME = "Icon";
        private static readonly string ABILITY_FRAME_UI_NAME = "IconFrame";

        private static readonly string ABILITY_ICON_HIDDEN_CLASSNAME = "equippedAbilityIconHidden";
        private static readonly string ABILITY_FRAME_ACTIVE_UI_NAME = "abilityIconFrameActive";
        private static readonly string ABILITY_ROOT_JUMP_CLASSNAME = "equippedAbilityJump";
        #endregion

        private readonly VisualElement _root;
        private readonly Button _abilityButton;
        private readonly VisualElement _abilityIcon;
        private readonly VisualElement _abilityFrame;

        public ABaseAbility CurrentAbility { get; private set; }

        public EquippedAbilityUIController(VisualElement root)
        {
            _root = root;
            _abilityButton = root.Q<Button>(ABILITY_BUTTON_UI_NAME);
            _abilityIcon = root.Q(ABILITY_ICON_UI_NAME);
            _abilityFrame = root.Q(ABILITY_FRAME_UI_NAME);
        }

        public void SetAbility(ABaseAbility ability)
        {
            bool hasAbility = ability != null;

            _root.SetEnabled(hasAbility);
            _abilityIcon.EnableInClassList(ABILITY_ICON_HIDDEN_CLASSNAME, !hasAbility);
            _abilityIcon.style.backgroundImage = hasAbility ? new StyleBackground(ability.Icon) : null;
            _abilityFrame.SetEnabled(hasAbility);
            _abilityFrame.visible = hasAbility;

            CurrentAbility = ability;
        }

        public void SetActive(bool isActive)
        {
            _abilityFrame.EnableInClassList(ABILITY_FRAME_ACTIVE_UI_NAME, isActive);
        }

        public void HideIcon()
        {
            _abilityIcon.AddToClassList(ABILITY_ICON_HIDDEN_CLASSNAME);
        }

        public IEnumerator PlayJumpAimation()
        {
            _abilityButton.AddToClassList(ABILITY_ROOT_JUMP_CLASSNAME);
            yield return new WaitForSeconds(0.2f);
            _abilityButton.RemoveFromClassList(ABILITY_ROOT_JUMP_CLASSNAME);
        }
    }
}
