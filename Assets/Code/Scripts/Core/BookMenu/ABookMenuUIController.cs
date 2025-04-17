using FrostfallSaga.Core.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.BookMenu
{
    public abstract class ABookMenuUIController : BaseUIController
    {
        protected VisualElement _leftPageContainer;
        protected VisualElement _rightPageContainer;

        protected virtual void Awake()
        {
            _leftPageContainer = _uiDoc.rootVisualElement.Q<VisualElement>(LEFT_PAGE_CONTAINER_UI_NAME);
            _rightPageContainer = _uiDoc.rootVisualElement.Q<VisualElement>(RIGHT_PAGE_CONTAINER_UI_NAME);
        }

        /// <summary>
        /// Setup the book menu with the left and right page containers.
        /// </summary>
        public abstract void SetupMenu();

        /// <summary>
        /// Destroys the content under the left and right page containers.
        /// </summary>
        public virtual void ClearMenu()
        {
            _leftPageContainer.Clear();
            _rightPageContainer.Clear();
        }

        #region UI Elements Names & Classes

        private static readonly string LEFT_PAGE_CONTAINER_UI_NAME = "LeftPageContainer";
        private static readonly string RIGHT_PAGE_CONTAINER_UI_NAME = "RightPageContainer";

        #endregion
    }
}