using System;
using UnityEngine.UIElements;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.BookMenu.UI
{
    public class BookMenuBarUIController : BaseUIController
    {
        #region UI Elements Names & Classes
        private static readonly string QUESTS_MENU_BUTTON_UI_NAME = "QuestsMenuButton";
        #endregion

        #region Buttons events
        public Action onQuestsMenuClicked;
        #endregion

        private void Awake()
        {
            _uiDoc.rootVisualElement.Q<Button>(QUESTS_MENU_BUTTON_UI_NAME).RegisterCallback<ClickEvent>(
                (_evt) => onQuestsMenuClicked?.Invoke()
            );
        }
    }
}