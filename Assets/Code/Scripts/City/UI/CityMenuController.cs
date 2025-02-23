using System;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Utils.Scenes;

namespace FrostfallSaga.City.UI
{
    public class CityMenuController : BaseUIController
    {
        private readonly static string CITY_NAME_LABEL_UI_NAME = "CityNameLabel";
        private readonly static string CITY_BG_UI_NAME = "MainMenuContainer";
        private readonly static string TAVERN_BUTTON_UI_NAME = "TavernButton";
        private readonly static string EXPLORE_BUTTON_UI_NAME = "ExploreButton";
        private readonly static string EXIT_BUTTON_UI_NAME = "ExitButton";
        private readonly static string LEFT_CONTAINER_UI_NAME = "LeftContainer";
        private readonly static string TAVERN_DIALOG_ROOT_UI_NAME = "TavernDialog";
        private readonly static string SITUATIONS_MENU_ROOT_UI_NAME = "CitySituationsMenu";

        public Action<ACitySituationSO> onCitySituationClicked;

        [SerializeField] private InCityConfigurationSO _devCityConfiguration;
        [SerializeField] private SituationsController _situationsController;
        [SerializeField] private VisualTreeAsset _situationsButtonTemplate;
        private InCityConfigurationSO _cityConfiguration;
        private LeftContainerController _leftContainerController;
        private TavernDialogController _tavernDialogController;
        private CitySituationsMenuController _situationsMenuController;

        #region Main menu setup
        private void Start()
        {
            HeroTeamQuests.Instance.InitializeQuests(this);
            SetupMainMenu();
        }

        private void SetupMainMenu()
        {
            // Main menu setup
            _uiDoc.rootVisualElement.Q<Label>(CITY_NAME_LABEL_UI_NAME).text = _cityConfiguration.Name;
            _uiDoc.rootVisualElement.Q<VisualElement>(CITY_BG_UI_NAME).style.backgroundImage = new(_cityConfiguration.CityBackground);
            _uiDoc.rootVisualElement.Q<Button>(TAVERN_BUTTON_UI_NAME).clicked += OnTavernButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXPLORE_BUTTON_UI_NAME).clicked += OnExploreButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;

            // Tavern dialog setup
            _tavernDialogController.SetupTavernDialog(_cityConfiguration.TavernConfiguration);

            // City situations menu setup
            _situationsMenuController.Init(
                _uiDoc.rootVisualElement.Q<VisualElement>(SITUATIONS_MENU_ROOT_UI_NAME),
                _situationsButtonTemplate,
                _cityConfiguration.CitySituations
            );
        }
        #endregion

        #region Navigation
        private void OnTavernButtonClicked()
        {
            _leftContainerController.Hide();
            _tavernDialogController.Display();
        }

        private void OnTavernDialogRestButtonClicked()
        {
            _tavernDialogController.Hide();
            _leftContainerController.Display();
        }

        private void OnTavernDialogExitButtonClicked()
        {
            _tavernDialogController.Hide();
            _leftContainerController.Display();
        }

        private void OnExploreButtonClicked()
        {
            _leftContainerController.Hide();
            StartCoroutine(_situationsMenuController.Display());
        }

        private void OnSituationsMenuReturnClicked()
        {
            _situationsMenuController.Hide();
            _leftContainerController.Display();
        }

        private void OnExitButtonClicked()
        {
            SceneTransitioner.Instance.FadeInToScene(EScenesName.KINGDOM.ToSceneString());
        }
        #endregion

        #region Situations interactions

        private void OnSituationButtonClicked(ACitySituationSO citySituation)
        {
            _situationsMenuController.Hide();
            onCitySituationClicked?.Invoke(citySituation);
        }

        private void OnSituationResolved(ACitySituationSO _citySituation)
        {
            StartCoroutine(_situationsMenuController.Display());
        }

        #endregion

        #region Class setup
        private void Awake()
        {
            // Get city configuration to load
            InCityConfigurationSO gameStateConf = GameStateManager.Instance.GetCityConfigurationToLoad();
            _cityConfiguration = gameStateConf != null ? gameStateConf : _devCityConfiguration;

            // Setup main menu
            _leftContainerController = new(_uiDoc.rootVisualElement.Q<VisualElement>(LEFT_CONTAINER_UI_NAME));

            // Setup tavern dialog
            _tavernDialogController = new(_uiDoc.rootVisualElement.Q<VisualElement>(TAVERN_DIALOG_ROOT_UI_NAME));
            _tavernDialogController.onRestButtonClicked += OnTavernDialogRestButtonClicked;
            _tavernDialogController.onExitClicked += OnTavernDialogExitButtonClicked;

            // Setup city situations
            _situationsMenuController = GetComponent<CitySituationsMenuController>();
            _situationsMenuController.onCitySituationClicked += OnSituationButtonClicked;
            _situationsMenuController.onReturnClicked += OnSituationsMenuReturnClicked;

            // Setup situations controller
            if (_situationsController == null)
            {
                Debug.LogError("SituationsController is not assigned in CityMenuController.");
                return;
            }
            _situationsController.onSituationResolved += OnSituationResolved;
        }
        #endregion
    }
}