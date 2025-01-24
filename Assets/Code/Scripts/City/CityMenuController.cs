using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.City
{
    public class CityMenuController : BaseUIController
    {
        private readonly static string CITY_NAME_LABEL_UI_NAME = "CityNameLabel";
        private readonly static string CITY_BG_UI_NAME = "MainMenuContainer";
        private readonly static string TAVERN_BUTTON_UI_NAME = "TavernButton";
        private readonly static string EXIT_BUTTON_UI_NAME = "ExitButton";

        private CityConfigurationSO _cityConfiguration;

        #region Main menu setup
        private void Start()
        {
            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            _uiDoc.rootVisualElement.Q<Label>(CITY_NAME_LABEL_UI_NAME).text = _cityConfiguration.Name;
            _uiDoc.rootVisualElement.Q<VisualElement>(CITY_BG_UI_NAME).style.backgroundImage = new(_cityConfiguration.CityBackground);
            _uiDoc.rootVisualElement.Q<Button>(TAVERN_BUTTON_UI_NAME).clicked += OnTavernButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;
        }
        #endregion

        #region Button callbacks
        private void OnTavernButtonClicked()
        {
            Debug.Log("Tavern button clicked.");
        }

        private void OnExitButtonClicked()
        {
            Debug.Log("Exit button clicked.");
        }
        #endregion

        #region Class setup
        private void Awake()
        {
            _cityConfiguration = CityLoadData.Instance.cityConfiguration;

        }
        #endregion
    }
}