using System.Collections;
using FrostfallSaga.Core.Fight;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityContainerUIController
    {
        private static readonly float UNLOCK_ANIMATION_DURATION = 0.5f;

        #region UXML Element names & classes
        private static readonly string ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string ABILITY_ICON_UI_NAME = "AbilityIcon";
        private static readonly string LOCK_UI_NAME = "Lock";
        private static readonly string ABILITY_BUTTON_SELECTED_FRAME_UI_NAME = "AbilityButtonSelectedFrame";

        private static readonly string ABILITY_ICON_LOCKED_CLASSNAME = "abilityIconLocked";
        private static readonly string ABILITY_ICON_UNLOCKED_CLASSNAME = "abilityIconUnlocked";
        private static readonly string LOCK_VISIBLE_CLASSNAME = "lockVisible";
        private static readonly string LOCK_HIDDEN_CLASSNAME = "lockHidden";
        private static readonly string ABILITY_BUTTON_SELECTED_FRAME_ACTIVE_CLASSNAME = "abilityButtonSelectedFrameActive";
        private static readonly string ABILITY_BUTTON_UNLOCK_CLASSNAME = "abilityButtonUnlock";
        #endregion
        
        public ABaseAbility CurrentAbility { get; private set; }
        public VisualElement Root { get; private set; }

        private readonly Button _abilityButton;
        private readonly VisualElement _abilityIcon;
        private readonly VisualElement _lock;
        private readonly VisualElement _abilityButtonSelectedFrame;
        private EAbilityState _abilityState;

        public AbilityContainerUIController(VisualElement root)
        {
            Root = root;
            _abilityButton = root.Q<Button>(ABILITY_BUTTON_UI_NAME);
            _abilityIcon = root.Q<VisualElement>(ABILITY_ICON_UI_NAME);
            _lock = root.Q<VisualElement>(LOCK_UI_NAME);
            _abilityButtonSelectedFrame = root.Q<VisualElement>(ABILITY_BUTTON_SELECTED_FRAME_UI_NAME);
        }

        public void SetAbility(ABaseAbility ability, EAbilityState abilityState)
        {
            CurrentAbility = ability;
            _abilityState = abilityState;
            UpdateUI();
        }

        public void SetDefautState()
        {
            CurrentAbility = null;
            _abilityState = EAbilityState.Locked;

            _abilityIcon.style.backgroundImage = null;
            _abilityIcon.ClearClassList();
            _abilityIcon.AddToClassList(ABILITY_ICON_LOCKED_CLASSNAME);

            _lock.ClearClassList();
            _lock.AddToClassList(LOCK_VISIBLE_CLASSNAME);
        }

        public void SetActive(bool isActive)
        {
            _abilityButtonSelectedFrame.EnableInClassList(ABILITY_BUTTON_SELECTED_FRAME_ACTIVE_CLASSNAME, isActive);
        }

        public void HideIcon()
        {
            _abilityIcon.style.opacity = 0;
        }

        public IEnumerator PlayUnlockAnimation()
        {
            _abilityButton.AddToClassList(ABILITY_BUTTON_UNLOCK_CLASSNAME);
            yield return new WaitForSeconds(UNLOCK_ANIMATION_DURATION);
            _abilityButton.RemoveFromClassList(ABILITY_BUTTON_UNLOCK_CLASSNAME);
        }

        private void UpdateUI()
        {
            _abilityIcon.style.opacity = 1;
            _abilityIcon.style.backgroundImage = new(CurrentAbility.Icon);

            _abilityIcon.ClearClassList();
            bool iconLocked = _abilityState == EAbilityState.Locked || _abilityState == EAbilityState.Unlockable;
            _abilityIcon.AddToClassList(iconLocked ? ABILITY_ICON_LOCKED_CLASSNAME : ABILITY_ICON_UNLOCKED_CLASSNAME);

            _lock.ClearClassList();
            _lock.AddToClassList(_abilityState == EAbilityState.Locked ? LOCK_VISIBLE_CLASSNAME : LOCK_HIDDEN_CLASSNAME);
        }
    }
}
