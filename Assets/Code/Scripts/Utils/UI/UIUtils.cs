using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    public static class UIUtils
    {
        /// <summary>
        /// Sets the picking mode for a VisualElement and all its children recursively.
        /// </summary>
        /// <param name="hierarchyRoot">The root VisualElement of the hierarchy.</param>
        /// <param name="pickingMode">The PickingMode to set for the VisualElement and its children.</param>
        public static void SetHierachyPickingMode(VisualElement hierarchyRoot, PickingMode pickingMode)
        {
            hierarchyRoot.pickingMode = pickingMode;

            foreach (var child in hierarchyRoot.Children())
            {
                SetHierachyPickingMode(child, pickingMode);
            }
        }
    }
}