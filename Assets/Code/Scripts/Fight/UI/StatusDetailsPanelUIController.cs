using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class StatusDetailsPanelUIController
    {
        #region UXML Element names and classes
        private static readonly string STATUS_DETAILS_LABEL_UI_NAME = "StatusDetailsLabel";

        private static readonly string STATUS_DETAILS_PANEL_HIDDEN_CLASSNAME = "statusDetailsPanelHidden";
        #endregion

        private readonly Label _statusDetailsLabel;

        public StatusDetailsPanelUIController(VisualElement root)
        {
            Root = root;
            _statusDetailsLabel = Root.Q<Label>(STATUS_DETAILS_LABEL_UI_NAME);
        }

        public VisualElement Root { get; private set; }

        public void Setup(AStatus status, int lastingDuration)
        {
            _statusDetailsLabel.text = status.GetUIString(lastingDuration);
        }

        public void Display()
        {
            Root.RemoveFromClassList(STATUS_DETAILS_PANEL_HIDDEN_CLASSNAME);
        }

        public void Hide()
        {
            Root.AddToClassList(STATUS_DETAILS_PANEL_HIDDEN_CLASSNAME);
        }
    }
}