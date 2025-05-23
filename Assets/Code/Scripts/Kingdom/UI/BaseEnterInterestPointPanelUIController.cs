using System;
using System.Collections;
using FrostfallSaga.Core;
using FrostfallSaga.Core.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Kingdom.UI
{
    public abstract class BaseEnterInterestPointPanelUIController : BaseUIController
    {
        #region UXML Names and classes
        private static readonly string PANEL_CONTAINER_UI_NAME = "ScreenPanelContainer";
        private static readonly string INTEREST_POINT_PANEL_CONTAINER_UI_NAME = "InterestPointPanelContainer";
        private static readonly string NAME_LABEL_UI_NAME = "InterestPointNameLabel";
        private static readonly string PREVIEW_CONTAINER_UI_NAME = "InterestPointPreview";
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "DescriptionLabel";
        private static readonly string ENTER_INTEREST_POINT_BUTTON_UI_NAME = "EnterButton";
        private static readonly string EXIT_INTEREST_POINT_BUTTON_UI_NAME = "ExitButton";
        private static readonly string PANEL_HIDDEN_CLASSNAME = "panelContainerHidden";
        private static readonly string INTEREST_POINT_PANEL_HIDDEN_CLASSNAME = "interestPointPanelContainerHidden";
        #endregion

        public Action<AInterestPointConfigurationSO> onInterestPointEnterClicked;

        [field: SerializeField] public VisualTreeAsset EnterInterestPointPanelTemplate { get; private set; }
        [SerializeField] private KingdomManager _kingdomManager;

        private TemplateContainer _interestPointGatePanel;
        protected AInterestPointConfigurationSO _currentInterestPoint;

        private void Awake()
        {
            if (_kingdomManager == null)
            {
                Debug.LogError("KingdomManager is not set. Won't be able to display interestPoint enter panel.");
                return;
            }

            _kingdomManager.OnInterestPointEncountered += OnInterestPointBuildingEncountered;
        }

        /// <summary>
        ///   Called when an interest point is encountered on the kingdom grid. Should be overridden in derived classes to
        ///   handle specific interest point types.
        /// </summary>
        /// <param name="interestPointConfiguration">The interest point configuration attached.</param>
        protected abstract void OnInterestPointBuildingEncountered(AInterestPointConfigurationSO interestPointConfiguration);

        protected virtual void InstantiateAndSetupPanel(AInterestPointConfigurationSO interestPointConfiguration)
        {
            _currentInterestPoint = interestPointConfiguration;
            _interestPointGatePanel = EnterInterestPointPanelTemplate.Instantiate();
            _interestPointGatePanel.Q<Button>(ENTER_INTEREST_POINT_BUTTON_UI_NAME).clicked += OnEnterClicked;
            _interestPointGatePanel.Q<Button>(EXIT_INTEREST_POINT_BUTTON_UI_NAME).clicked += OnExitClicked;
            SetupInterestPointPanel(interestPointConfiguration);
            _uiDoc.rootVisualElement.Add(_interestPointGatePanel);

            StartCoroutine(DisplayPanel());
        }

        protected virtual IEnumerator DisplayPanel()
        {
            _interestPointGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME)
                .RemoveFromClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _interestPointGatePanel.Q<VisualElement>(INTEREST_POINT_PANEL_CONTAINER_UI_NAME)
                .RemoveFromClassList(INTEREST_POINT_PANEL_HIDDEN_CLASSNAME);
        }

        protected virtual IEnumerator HidePanel()
        {
            _interestPointGatePanel.Q<VisualElement>(INTEREST_POINT_PANEL_CONTAINER_UI_NAME)
                .AddToClassList(INTEREST_POINT_PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.4f);
            _interestPointGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME)
                .AddToClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _interestPointGatePanel.RemoveFromHierarchy();
        }

        protected virtual void SetupInterestPointPanel(AInterestPointConfigurationSO interestPointBuildingConfiguration)
        {
            _interestPointGatePanel.StretchToParentSize();
            _interestPointGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).AddToClassList(PANEL_HIDDEN_CLASSNAME);
            _interestPointGatePanel.Q<VisualElement>(INTEREST_POINT_PANEL_CONTAINER_UI_NAME)
                .AddToClassList(INTEREST_POINT_PANEL_HIDDEN_CLASSNAME);
            _interestPointGatePanel.Q<VisualElement>(PREVIEW_CONTAINER_UI_NAME).style.backgroundImage =
                new StyleBackground(interestPointBuildingConfiguration.InterestPointPreview);
            _interestPointGatePanel.Q<Label>(NAME_LABEL_UI_NAME).text = interestPointBuildingConfiguration.Name;
            _interestPointGatePanel.Q<Label>(DESCRIPTION_LABEL_UI_NAME).text = interestPointBuildingConfiguration.Description;
        }

        protected virtual void OnEnterClicked()
        {
            StartCoroutine(HidePanel());
            onInterestPointEnterClicked?.Invoke(_currentInterestPoint);
        }

        protected virtual void OnExitClicked()
        {
            _currentInterestPoint = null;
            StartCoroutine(HidePanel());
        }
    }
}