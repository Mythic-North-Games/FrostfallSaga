using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    /// <summary>
    ///     Class that attaches a UI element to a game object in the world.
    /// </summary>
    public class WorldUIPositioner : MonoBehaviour
    {
        private UIDocument _uiDocToDisplayOn;
        private VisualElement _uiToDisplay;
        private Transform _anchorTransform;
        private Vector3 _offset;
        private bool _centerX;
        private bool _centerY;
        private bool _isInitialized;

        /// <summary>
        /// Initializes the positioner with required references.
        /// </summary>
        public void Setup(
            UIDocument uiDocToDisplayOn,
            VisualElement uiToDisplay,
            Transform anchorTransform,
            Vector3? offset = null
        )
        {
            _uiDocToDisplayOn = uiDocToDisplayOn;
            _uiToDisplay = uiToDisplay;
            _anchorTransform = anchorTransform;
            _offset = offset ?? Vector3.zero;

            if (_uiDocToDisplayOn == null || _uiToDisplay == null || _anchorTransform == null)
            {
                Debug.LogError("WorldUIPositioner: Setup failed. One or more required parameters are null.");
                enabled = false;
                return;
            }

            _isInitialized = true;
        }

        /// <summary>
        /// Optional: Set a vertical or world-space offset for the UI element.
        /// </summary>
        public void SetOffset(Vector3 offset)
        {
            _offset = offset;
        }

        private void LateUpdate()
        {
            if (!_isInitialized || _uiToDisplay == null || UnityEngine.Camera.main == null) return;

            Vector3 worldPos = _anchorTransform.position;
            Vector3 screenPos = UnityEngine.Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0)
            {
                _uiToDisplay.style.display = DisplayStyle.None;
                return;
            }

            _uiToDisplay.style.display = DisplayStyle.Flex;

            // Convert screen space to panel (UI Toolkit) space
            var panel = _uiToDisplay.panel;
            if (panel == null) return;

            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(panel, screenPos);

            // Flip Y coordinate to match the UI space
            panelPos.y = panel.visualTree.layout.height - panelPos.y;

            // Apply offset
            panelPos.x += _offset.x;
            panelPos.y += _offset.y;

            _uiToDisplay.style.left = panelPos.x;
            _uiToDisplay.style.top = panelPos.y;
        }

        private void OnDestroy()
        {
            if (_isInitialized && _uiToDisplay != null)
            {
                // Remove the UI element from the panel
                if (_uiToDisplay.parent != null)
                {
                    _uiToDisplay.RemoveFromHierarchy();
                }

                // Clean up references
                _uiDocToDisplayOn = null;
                _uiToDisplay = null;
                _anchorTransform = null;
            }
        }
    }
}