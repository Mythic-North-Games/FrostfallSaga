using FrostfallSaga.Core.BookMenu;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Settings.UI
{
    public class BookSettingsMenuUIController  : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset _controlPanelTemplate;

        public override void SetupMenu()
        {
            // Setup control panel
            VisualElement controlPanelRoot = _controlPanelTemplate.Instantiate();
            controlPanelRoot.StretchToParentSize();
            ControlMenuUIController.Setup(controlPanelRoot);

            _leftPageContainer.Add(controlPanelRoot);
        }
    }
}
