using System;
using System.Collections;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Kingdom.UI
{
    public class EnterCityPanelController : BaseUIController
    {
        private static readonly string PANEL_CONTAINER_UI_NAME = "ScreenPanelContainer";
        private static readonly string CITY_PANEL_CONTAINER_UI_NAME = "CityPanelContainer";
        private static readonly string NAME_LABEL_UI_NAME = "CityNameLabel";
        private static readonly string PREVIEW_CONTAINER_UI_NAME = "CityPreview";
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "DescriptionLabel";
        private static readonly string ENTER_CITY_BUTTON_UI_NAME = "EnterButton";
        private static readonly string EXIT_CITY_BUTTON_UI_NAME = "ExitButton";
        private static readonly string PANEL_HIDDEN_CLASSNAME = "panelContainerHidden";
        private static readonly string CITY_PANEL_HIDDEN_CLASSNAME = "cityPanelContainerHidden";

        [field: SerializeField] public VisualTreeAsset EnterCityPanelTemplate { get; private set; }
        [SerializeField] private KingdomManager _kingdomManager;

        private TemplateContainer _cityGatePanel;
        private CityBuildingConfigurationSO _currentCity;

        public Action<CityBuildingConfigurationSO> onCityEnterClicked;

        private void Awake()
        {
            if (_kingdomManager == null)
            {
                Debug.LogError("KingdomManager is not set. Won't be able to display city enter panel.");
                return;
            }

            _kingdomManager.OnInterestPointEncountered += OnCityBuildingEncountered;
        }

        private void OnCityBuildingEncountered(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not CityBuildingConfigurationSO cityBuildingConfiguration) return;

            _currentCity = cityBuildingConfiguration;
            _cityGatePanel = EnterCityPanelTemplate.Instantiate();
            _cityGatePanel.Q<Button>(ENTER_CITY_BUTTON_UI_NAME).clicked += OnDisplayClicked;
            _cityGatePanel.Q<Button>(EXIT_CITY_BUTTON_UI_NAME).clicked += OnExitClicked;
            SetupCityPanel(cityBuildingConfiguration);
            _uiDoc.rootVisualElement.Add(_cityGatePanel);

            StartCoroutine(DisplayPanel());
        }

        private IEnumerator DisplayPanel()
        {
            _cityGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).RemoveFromClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _cityGatePanel.Q<VisualElement>(CITY_PANEL_CONTAINER_UI_NAME)
                .RemoveFromClassList(CITY_PANEL_HIDDEN_CLASSNAME);
        }

        private IEnumerator HidePanel()
        {
            _cityGatePanel.Q<VisualElement>(CITY_PANEL_CONTAINER_UI_NAME).AddToClassList(CITY_PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.1f);
            _cityGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).AddToClassList(PANEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.4f);
            _cityGatePanel.RemoveFromHierarchy();
        }

        private void SetupCityPanel(CityBuildingConfigurationSO cityBuildingConfiguration)
        {
            _cityGatePanel.style.flexGrow = 1;
            _cityGatePanel.Q<VisualElement>(PANEL_CONTAINER_UI_NAME).AddToClassList(PANEL_HIDDEN_CLASSNAME);
            _cityGatePanel.Q<VisualElement>(CITY_PANEL_CONTAINER_UI_NAME).AddToClassList(CITY_PANEL_HIDDEN_CLASSNAME);
            _cityGatePanel.Q<VisualElement>(PREVIEW_CONTAINER_UI_NAME).style.backgroundImage =
                new StyleBackground(cityBuildingConfiguration.CityPreview);
            _cityGatePanel.Q<Label>(NAME_LABEL_UI_NAME).text = cityBuildingConfiguration.Name;
            _cityGatePanel.Q<Label>(DESCRIPTION_LABEL_UI_NAME).text = cityBuildingConfiguration.Description;
        }

        private void OnDisplayClicked()
        {
            StartCoroutine(HidePanel());
            onCityEnterClicked?.Invoke(_currentCity);
        }

        private void OnExitClicked()
        {
            StartCoroutine(HidePanel());
            _currentCity = null;
        }
    }
}