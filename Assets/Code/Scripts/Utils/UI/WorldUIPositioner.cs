using UnityEngine;
using UnityEngine.UIElements;


namespace FrostfallSaga.Utils.UI
{
    /// <summary>
    /// Class that attaches a UI element to a game object in the world.
    /// </summary>
    public class WorldUIPositioner : MonoBehaviour
    {
        private IPanel _displayPanel;
        private VisualElement _elementToAttach;
        private Transform _anchor;
        private bool _centerOnX;
        private bool _centerOnY;
        private Vector2 _offset;

        public void Setup(
            UIDocument documentToDisplayOn,
            VisualElement elementToAttach,
            Transform anchor,
            bool centerOnX = false,
            bool centerOnY = false,
            Vector2 offset = new()
        )
        {
            _displayPanel = documentToDisplayOn.rootVisualElement.panel;
            _elementToAttach = elementToAttach;
            _anchor = anchor;
            _centerOnX = centerOnX;
            _centerOnY = centerOnY;
            _offset = offset;
        }

        private void Update()
        {
            if (_elementToAttach == null || _anchor == null)
            {
                return;
            }
            _elementToAttach.transform.position = ComputeDisplayPosition();
        }

        private Vector2 ComputeDisplayPosition()
        {
            Vector3 anchorScreenPosition = UnityEngine.Camera.main.WorldToScreenPoint(_anchor.position);

            // Convert the anchor's screen position to UI space
            Vector2 anchorUIPosition = RuntimePanelUtils.ScreenToPanel(_displayPanel, anchorScreenPosition);

            // Compute position if should center
            Vector2 basePosition = new(
                _centerOnX ? anchorUIPosition.x - _elementToAttach.layout.width / 2 : anchorUIPosition.x,
                _centerOnY ? anchorUIPosition.y - _elementToAttach.layout.height / 2 : anchorUIPosition.y
            );
            basePosition.y = Screen.height - basePosition.y;

            // Add offset to position
            return basePosition + _offset;
        }
    }
}