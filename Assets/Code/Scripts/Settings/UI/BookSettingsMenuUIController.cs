using FrostfallSaga.Core.BookMenu;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Settings.UI
{
    public class BookSettingsMenuUIController  : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset _controlPanelTemplate;
        [SerializeField] private VisualTreeAsset _settingsPanelTemplate;

        public override void SetupMenu()
        {
            // Setup control panel
            VisualElement controlPanelRoot = _controlPanelTemplate.Instantiate();
            controlPanelRoot.StretchToParentSize();
            ControlMenuUIController.Setup(controlPanelRoot);

            // Setup settings panel
            VisualElement settingsPanelRoot = _settingsPanelTemplate.Instantiate();
            settingsPanelRoot.StretchToParentSize();
            SettingsMenuUIController.Setup(settingsPanelRoot);

            _leftPageContainer.Add(controlPanelRoot);
            _rightPageContainer.Add(settingsPanelRoot);
        }
    }
}
