using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public static class StatusContainerUIController
    {
        #region UXML UI Names & Classes

        private static readonly string STATUS_ICON_UI_NAME = "StatusIcon";

        #endregion

        public static void SetupStatusContainer(VisualElement root, AStatus status)
        {
            root.Q<VisualElement>(STATUS_ICON_UI_NAME).style.backgroundImage = new(status.Icon);
        }
    }
}