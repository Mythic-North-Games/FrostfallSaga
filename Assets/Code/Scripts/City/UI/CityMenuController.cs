using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Utils.Scenes;
using FrostfallSaga.Core.GameState;

namespace FrostfallSaga.City.UI
{
    public class CityMenuController : BaseUIController
    {
        private readonly static string CITY_NAME_LABEL_UI_NAME = "CityNameLabel";
        private readonly static string CITY_BG_UI_NAME = "MainMenuContainer";
        private readonly static string TAVERN_BUTTON_UI_NAME = "TavernButton";
        private readonly static string EXIT_BUTTON_UI_NAME = "ExitButton";
        private readonly static string LEFT_CONTAINER_UI_NAME = "LeftContainer";
        private readonly static string TAVERN_DIALOG_ROOT_UI_NAME = "TavernDialog";

        [SerializeField] private InCityConfigurationSO _devCityConfiguration;
        [SerializeField] private string _kingdomSceneName;
        private InCityConfigurationSO _cityConfiguration;
        private LeftContainerController _leftContainerController;
        private TavernDialogController _tavernDialogController;

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
            _tavernDialogController.SetupTavernDialog(_cityConfiguration.TavernConfiguration);
        }
        #endregion

        #region Navigation
        private void OnTavernButtonClicked()
        {
            Debug.Log("Tavern button clicked.");
            _leftContainerController.Hide();
            _tavernDialogController.Display();
        }

        private void OnTavernDialogExitButtonClicked()
        {
            _tavernDialogController.Hide();
            _leftContainerController.Display();
        }

        private void OnExitButtonClicked()
        {
            SceneTransitioner.Instance.FadeInToScene(_kingdomSceneName);
        }
        #endregion

        #region Class setup
        private void Awake()
        {
            InCityConfigurationSO gameStateConf = GameStateManager.Instance.GetCityConfigurationToLoad();
            _cityConfiguration = gameStateConf != null ? gameStateConf : _devCityConfiguration;
            _leftContainerController = new(_uiDoc.rootVisualElement.Q<VisualElement>(LEFT_CONTAINER_UI_NAME));
            _tavernDialogController = new(_uiDoc.rootVisualElement.Q<VisualElement>(TAVERN_DIALOG_ROOT_UI_NAME));
            _tavernDialogController.onExitClicked += OnTavernDialogExitButtonClicked;
        }
        #endregion
    }
}