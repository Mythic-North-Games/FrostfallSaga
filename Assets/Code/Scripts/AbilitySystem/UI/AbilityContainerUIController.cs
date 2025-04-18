using FrostfallSaga.Core.Fight;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityContainerUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_ICON_UI_NAME = "AbilityIcon";
        private static readonly string ICE_LOCK_UI_NAME = "IceLock";

        private static readonly string ABILITY_ICON_LOCKED_CLASSNAME = "abilityIconLocked";
        private static readonly string ABILITY_ICON_UNLOCKED_CLASSNAME = "abilityIconUnlocked";
        private static readonly string ICE_LOCK_VISIBLE_CLASSNAME = "iceLockVisible";
        private static readonly string ICE_LOCK_HIDDEN_CLASSNAME = "iceLockHidden";
        #endregion
        
        public ABaseAbility CurrentAbility { get; private set; }

        private readonly VisualElement _abilityIcon;
        private readonly VisualElement _iceLock;
        private EAbilityState _abilityState;

        public AbilityContainerUIController(VisualElement root)
        {
            _abilityIcon = root.Q<VisualElement>(ABILITY_ICON_UI_NAME);
            _iceLock = root.Q<VisualElement>(ICE_LOCK_UI_NAME);
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

            _iceLock.ClearClassList();
            _iceLock.AddToClassList(ICE_LOCK_VISIBLE_CLASSNAME);
        }

        private void UpdateUI()
        {
            _abilityIcon.style.backgroundImage = new(CurrentAbility.Icon);
            _abilityIcon.ClearClassList();
            bool iconLocked = _abilityState == EAbilityState.Locked || _abilityState == EAbilityState.Unlockable;
            _abilityIcon.AddToClassList(iconLocked ? ABILITY_ICON_LOCKED_CLASSNAME : ABILITY_ICON_UNLOCKED_CLASSNAME);

            _iceLock.ClearClassList();
            _iceLock.AddToClassList(_abilityState == EAbilityState.Locked ? ICE_LOCK_VISIBLE_CLASSNAME : ICE_LOCK_HIDDEN_CLASSNAME);
        }
    }
}
