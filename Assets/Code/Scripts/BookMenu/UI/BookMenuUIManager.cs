using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Quests.UI;
using FrostfallSaga.InventorySystem.UI;

namespace FrostfallSaga.BookMenu.UI
{
    public class BookMenuUIManager : BaseUIController
    {
        #region UI Elements Names & Classes
        private static readonly string BOOK_MENU_CONTAINER_UI_NAME = "BookMenuContainer";
        private static readonly string BOOK_MENU_ROOT_UI_NAME = "BookRoot";

        private static readonly string BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME = "bookMenuContainerHidden";
        private static readonly string BOOK_MENU_ROOT_HIDDEN_CLASSNAME = "bookRootHidden";
        #endregion

        [SerializeField] private BookMenuBarUIController _bookMenuBarController;
        [SerializeField] private BookQuestsMenuUIController _questsMenuController;
        [SerializeField] private BookInventoryMenuUIController _inventoryMenuController;

        private VisualElement _bookMenuContainer;
        private VisualElement _bookMenuRoot;
        private ABookMenuUIController _currentMenuController;

        private void OnQuestsMenuClicked()
        {
            _currentMenuController = _questsMenuController;
            _questsMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private void OnInventoryMenuClicked()
        {
            _currentMenuController = _inventoryMenuController;
            _inventoryMenuController.SetupMenu();
            StartCoroutine(ShowBookMenu());
        }

        private IEnumerator ShowBookMenu()
        {
            _bookMenuContainer.RemoveFromClassList(BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.5f);
            _bookMenuRoot.RemoveFromClassList(BOOK_MENU_ROOT_HIDDEN_CLASSNAME);
        }

        private IEnumerator HideBookMenu()
        {
            _bookMenuRoot.AddToClassList(BOOK_MENU_ROOT_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.5f);
            _bookMenuContainer.AddToClassList(BOOK_MENU_CONTAINER_HIDDEN_CLASSNAME);
            _currentMenuController.ClearMenu();
        }

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
        }
        #endregion
    }
}