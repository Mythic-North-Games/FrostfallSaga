using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    public class UIOverlayManager : MonoBehaviourPersistingSingleton<UIOverlayManager>
    {
        static UIOverlayManager()
        {
            PersistAcrossScenes = false;
        }

        private static readonly string OVERLAYS_CONTAINER_UI_NAME = "OverlaysRoot";

        public Action<VisualElement> onOverlayAddedCallback;
        public Action<VisualElement> onOverlayRemovedCallback;

        [SerializeField] private UIDocument _overlayUIDocument;
        [SerializeField] private Vector2 _defaultMouseOffset = new(10, 10);
        private VisualElement _overlaysRoot;
        private readonly Dictionary<VisualElement, (bool followMouse, Vector2 customMouseOffset)> _attachedOverlays = new();

        protected override void Init()
        {
            if (_overlayUIDocument == null)
            {
                Debug.LogError("Overlay UIDocument is not assigned in the inspector.");
                return;
            }

            _overlaysRoot = _overlayUIDocument.rootVisualElement.Q(OVERLAYS_CONTAINER_UI_NAME);
        }

        /// <summary>
        /// Adds an overlay to the UI. The overlay will spawn at the mouse position unless specified otherwise.
        /// If followMouse is true, the overlay will follow the mouse position.
        /// </summary>
        /// <param name="overlay">The overlay to add.</param>
        /// <param name="overridenPosition">The position to spawn the overlay at. If not specified, the overlay will spawn at the mouse position.</param>
        /// <param name="followMouse">If true, the overlay will follow the mouse position.</param>
        public void AddOverlay(
            VisualElement overlay,
            Vector2 overridenPosition = default,
            Vector2 customMouseOffset = default,
            bool followMouse = false
        )
        {
            if (overlay == null)
            {
                Debug.LogError("Overlay is null.");
                return;
            }

            if (_attachedOverlays.ContainsKey(overlay))
            {
                Debug.LogWarning("Overlay is already attached.");
                return;
            }

            _attachedOverlays.Add(overlay, (followMouse, customMouseOffset));
            overlay.style.position = Position.Absolute;
            _overlaysRoot.Add(overlay);

            if (overridenPosition != default)
            {
                overlay.style.left = overridenPosition.x;
                overlay.style.top = overridenPosition.y;
                onOverlayAddedCallback?.Invoke(overlay);
            }
            else
            {
                // Defer setting the position since it needs the size of the overlay to be computed by Unity first
                overlay.RegisterCallback<GeometryChangedEvent>(OnOverlayFirstGeometryChanged);
            }
        }

        public void RemoveOverlay(VisualElement overlay)
        {
            if (overlay == null)
            {
                Debug.LogError("Overlay is null.");
                return;
            }

            if (!_attachedOverlays.ContainsKey(overlay)) return;

            _attachedOverlays.Remove(overlay);
            overlay.RemoveFromHierarchy();

            onOverlayRemovedCallback?.Invoke(overlay);
        }

        private void Update()
        {
            Vector2 currentMousePosition = GetCurrentMousePosition();
            _attachedOverlays.Where(overlay => overlay.Value.followMouse).ToList().ForEach(overlay =>
            {
                SetOverlayPositionOnMouse(overlay.Key);
            });
        }

        private void OnOverlayFirstGeometryChanged(GeometryChangedEvent evt)
        {
            VisualElement overlay = evt.target as VisualElement;
            overlay.UnregisterCallback<GeometryChangedEvent>(OnOverlayFirstGeometryChanged);
            SetOverlayPositionOnMouse(overlay);
            onOverlayAddedCallback?.Invoke(overlay);
        }

        private void SetOverlayPositionOnMouse(VisualElement overlay)
        {
            Vector2 currentMousePosition = GetCurrentMousePosition();
            Vector2 mouseOffset = GetOverlayMouseOffset(overlay, currentMousePosition);
            overlay.style.left = currentMousePosition.x + mouseOffset.x;
            overlay.style.top = currentMousePosition.y + mouseOffset.y;
        }

        private Vector2 GetCurrentMousePosition()
        {
            Vector2 screenMousePosition = Input.mousePosition;
            screenMousePosition.y = Screen.height - screenMousePosition.y;  // Invert Y coordinate
            return RuntimePanelUtils.ScreenToPanel(_overlaysRoot.panel, screenMousePosition);
        }

        private Vector2 GetOverlayMouseOffset(VisualElement overlay, Vector2 currentMousePosition)
        {
            if (!_attachedOverlays.ContainsKey(overlay))
            {
                Debug.LogError("Overlay is not attached.");
                return Vector2.zero;
            }

            var (_, customMouseOffset) = _attachedOverlays[overlay];

            if (customMouseOffset != default)
                return customMouseOffset;

            if (currentMousePosition.y + overlay.resolvedStyle.height > _overlaysRoot.resolvedStyle.height)
            {
                float height = overlay.resolvedStyle.height > 0 ? overlay.resolvedStyle.height : 0;
                return new Vector2(0, -height - _defaultMouseOffset.y);
            }
            if (currentMousePosition.x + overlay.resolvedStyle.width > _overlaysRoot.resolvedStyle.width)
            {
                float width = overlay.resolvedStyle.width > 0 ? overlay.resolvedStyle.width : 0;
                return new Vector2(-width - _defaultMouseOffset.x, 0);
            }

            return _defaultMouseOffset;
        }

    }
}