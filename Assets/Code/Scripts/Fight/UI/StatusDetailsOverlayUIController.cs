using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class StatusDetailsOverlayUIController : BaseOverlayUIController
    {
        private static readonly string STATUS_DETAILS_PANEL_DEFAULT_CLASSNAME = "statusDetailsContainer";
        private static readonly string STATUS_DETAILS_PANEL_HIDDEN_CLASSNAME = "statusDetailsContainerHidden";

        private readonly StatusDetailsPanelUIController _statusDetailsPanelController;

        public StatusDetailsOverlayUIController(
            VisualTreeAsset overlayTemplate
        ) : base(overlayTemplate, STATUS_DETAILS_PANEL_DEFAULT_CLASSNAME, STATUS_DETAILS_PANEL_HIDDEN_CLASSNAME)
        {
            _statusDetailsPanelController = new(OverlayRoot);
        }

        public void SetStatus(AStatus status, int lastingDuration)
        {
            _statusDetailsPanelController.Setup(status, lastingDuration);
        }
    }
}