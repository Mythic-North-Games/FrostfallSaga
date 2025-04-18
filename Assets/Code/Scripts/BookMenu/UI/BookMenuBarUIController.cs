using System;
using FrostfallSaga.Core.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.BookMenu.UI
{
    public class BookMenuBarUIController : BaseUIController
    {
        #region UI Elements Names & Classes
        private static readonly string QUESTS_MENU_BUTTON_UI_NAME = "QuestsMenuButton";
        private static readonly string INVENTORY_MENU_BUTTON_UI_NAME = "InventoryMenuButton";
        private static readonly string ABILITY_SYSTEM_MENU_BUTTON_UI_NAME = "AbilitySystemMenuButton";
        #endregion

        #region Buttons events
        public Action onQuestsMenuClicked;
        public Action onInventoryMenuClicked;
        public Action onAbilitySystemMenuClicked;
        #endregion

        private void Awake()
        {
            _uiDoc.rootVisualElement.Q<Button>(QUESTS_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onQuestsMenuClicked?.Invoke()
            );
            _uiDoc.rootVisualElement.Q<Button>(INVENTORY_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onInventoryMenuClicked?.Invoke()
            );
            _uiDoc.rootVisualElement.Q<Button>(ABILITY_SYSTEM_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onAbilitySystemMenuClicked?.Invoke()
            );
        }
    }
}