using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core
{
    public class BaseUIController : MonoBehaviour
    {
        [SerializeField] protected UIDocument _uiDoc;

        /// <summary>
        /// Check if the mouse is currently over any UI element in the UI Document.
        /// </summary>
        /// <returns>True if the mouse is over the UI, false otherwise.</returns>
        public bool IsMouseOverUIDocument()
        {
            if (_uiDoc == null)
            {
                Debug.LogError($"No UIDocument assigned to UI controller {name}.");
                return false;
            }

            // Get the root VisualElement of the UIDocument
            VisualElement root = _uiDoc.rootVisualElement;

            // Get the current mouse position
            Vector2 mousePosition = Input.mousePosition;
            mousePosition.y = Screen.height - mousePosition.y; // Convert to UI Toolkit's coordinate system

            // Check if the mouse is over any element in the root
            VisualElement pickedElement = root.panel.Pick(mousePosition);

            return pickedElement != null; // Returns true if the mouse is over any UI element
        }
    }
}