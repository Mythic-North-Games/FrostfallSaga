using System.Collections;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class BaseOverlayUIController
    {
        private static readonly float OVERLAY_DISPLAY_DELAY = 0.25f;

        public VisualElement OverlayRoot { get; private set; }
        protected string _overlayVisibleClassname;
        protected string _overlayHiddenClassname;

        protected BaseOverlayUIController(
            VisualTreeAsset overlayTemplate,
            string overlayVisibleClassname,
            string overlayHiddenClassname
        )
        {
            _overlayVisibleClassname = overlayVisibleClassname;
            _overlayHiddenClassname = overlayHiddenClassname;
            OverlayRoot = overlayTemplate.Instantiate();
            OverlayRoot.AddToClassList(_overlayVisibleClassname);
            OverlayRoot.AddToClassList(_overlayHiddenClassname);
            UIOverlayManager.Instance.onOverlayAddedCallback += OnOverlayAdded;
            UIOverlayManager.Instance.onOverlayRemovedCallback += OnOverlayRemoved;
        }

        public virtual void ShowOverlay(
            Vector2 customMouseOffset = default,
            Vector2 overridenPosition = default,
            bool followMouse = false
        )
        {
            UIOverlayManager.Instance.AddOverlay(
                OverlayRoot,
                overridenPosition,
                customMouseOffset,
                followMouse
            );
        }

        public virtual void HideOverlay()
        {
            UIOverlayManager.Instance.RemoveOverlay(OverlayRoot);
        }

        protected virtual void OnOverlayAdded(VisualElement overlay)
        {
            if (overlay != OverlayRoot) return;
            CoroutineRunner.Instance.StartCoroutine(ShowOverlayRootCoroutine());
        }

        protected virtual void OnOverlayRemoved(VisualElement overlay)
        {
            if (overlay != OverlayRoot) return;
            CoroutineRunner.Instance.StartCoroutine(HideOverlayRootCoroutine());
        }

        protected virtual IEnumerator ShowOverlayRootCoroutine()
        {
            yield return new WaitForSeconds(OVERLAY_DISPLAY_DELAY);
            OverlayRoot.RemoveFromClassList(_overlayHiddenClassname);
        }

        protected virtual IEnumerator HideOverlayRootCoroutine()
        {
            yield return new WaitForSeconds(OVERLAY_DISPLAY_DELAY);
            OverlayRoot.AddToClassList(_overlayHiddenClassname);
        }
    }
}