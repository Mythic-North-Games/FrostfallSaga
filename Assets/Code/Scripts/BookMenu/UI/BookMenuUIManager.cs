using System.Collections;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.UI;
using FrostfallSaga.InventorySystem.UI;
using FrostfallSaga.Quests.UI;
using FrostfallSaga.AbilitySystem.UI;
using FrostfallSaga.Settings.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.BookMenu.UI
{
    public class BookMenuUIManager : BaseUIController
    {
        [SerializeField] private BookMenuBarUIController _bookMenuBarController;
        [SerializeField] private BookInventoryMenuUIController _inventoryMenuController;
        [SerializeField] private BookQuestsMenuUIController _questsMenuController;
        [SerializeField] private BookAbilitySystemMenuUIController _abilitySystemMenuController;
        [SerializeField] private BookSettingsMenuUIController _settingsMenuController;

        private VisualElement _bookMenuContainer;
        private VisualElement _bookMenuRoot;
        private ABookMenuUIController _currentMenuController;

        private void OnInventoryMenuClicked()
        {
            _currentMenuController = _inventoryMenuController;
            _inventoryMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private void OnQuestsMenuClicked()
        {
            _currentMenuController = _questsMenuController;
            _questsMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private void OnAbilitySystemMenuClicked()
        {
            _currentMenuController = _abilitySystemMenuController;
            _abilitySystemMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private void OnSettingsMenuClicked()
        {
            _currentMenuController = _settingsMenuController;
            _settingsMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private IEnumerator ShowBookMenu()
        {
            _bookMenuRoot.pickingMode = PickingMode.Position;
            _bookMenuContainer.RemoveFromClassList(BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.5f);
            _bookMenuRoot.RemoveFromClassList(BOOK_MENU_ROOT_HIDDEN_CLASSNAME);
        }

        private IEnumerator HideBookMenu()
        {
            _bookMenuRoot.pickingMode = PickingMode.Ignore;
            _bookMenuRoot.AddToClassList(BOOK_MENU_ROOT_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.5f);
            _bookMenuContainer.AddToClassList(BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME);
            _currentMenuController.ClearMenu();
        }

        #region UI Elements Names & Classes

        private static readonly string BOOK_MENU_CONTAINER_UI_NAME = "BookMenuContainer";
        private static readonly string BOOK_MENU_ROOT_UI_NAME = "BookRoot";

        private static readonly string BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME = "bookMenuContainerHidden";
        private static readonly string BOOK_MENU_ROOT_HIDDEN_CLASSNAME = "bookRootHidden";

        #endregion

        #region Setup

        private void Awake()
        {
            if (_bookMenuBarController == null)
            {
                Debug.LogError("BookMenuBarController is not set in the inspector.");
                return;
            }

            if (_questsMenuController == null)
            {
                Debug.LogError("QuestsMenuController is not set in the inspector.");
                return;
            }

            if (_inventoryMenuController == null)
            {
                Debug.LogError("InventoryMenuController is not set in the inspector.");
                return;
            }
            if (_abilitySystemMenuController == null)
            {
                Debug.LogError("AbilityMenuController is not set in the inspector.");
                return;
            }

            _bookMenuContainer = _uiDoc.rootVisualElement.Q<VisualElement>(BOOK_MENU_CONTAINER_UI_NAME);
            _bookMenuContainer.AddToClassList(BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME);
            _bookMenuRoot = _uiDoc.rootVisualElement.Q<VisualElement>(BOOK_MENU_ROOT_UI_NAME);
            _bookMenuRoot.AddToClassList(BOOK_MENU_ROOT_HIDDEN_CLASSNAME);

            SetupBookMenuBarEvents();
            _bookMenuContainer.RegisterCallback<ClickEvent>(
                clickEvent =>
                {
                    // To prevent hiding the book menu if click comes from children.
                    if (clickEvent.target == _bookMenuContainer)
                    {
                        StartCoroutine(HideBookMenu());
                    }
                }
            );
        }

        private void SetupBookMenuBarEvents()
        {
            _bookMenuBarController.onQuestsMenuClicked += OnQuestsMenuClicked;
            _bookMenuBarController.onInventoryMenuClicked += OnInventoryMenuClicked;
            _bookMenuBarController.onAbilitySystemMenuClicked += OnAbilitySystemMenuClicked;
            _bookMenuBarController.onSettingsMenuClicked += OnSettingsMenuClicked;
        }

        #endregion
    }
}