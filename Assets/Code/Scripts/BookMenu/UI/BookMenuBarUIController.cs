using System;
using FrostfallSaga.Core.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.BookMenu.UI
{
    public class BookMenuBarUIController : BaseUIController
    {
        private void Awake()
        {
            _uiDoc.rootVisualElement.Q<Button>(QUESTS_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onQuestsMenuClicked?.Invoke()
            );
            _uiDoc.rootVisualElement.Q<Button>(INVENTORY_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onInventoryMenuClicked?.Invoke()
            );
        }

        #region UI Elements Names & Classes

        private static readonly string QUESTS_MENU_BUTTON_UI_NAME = "QuestsMenuButton";
        private static readonly string INVENTORY_MENU_BUTTON_UI_NAME = "InventoryMenuButton";

        #endregion

        #region Buttons events

        public Action onQuestsMenuClicked;
        public Action onInventoryMenuClicked;

        #endregion
    }
}