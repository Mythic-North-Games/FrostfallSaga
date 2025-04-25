using System;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class CityMenuController : BaseUIController
    {
        private static readonly string CITY_NAME_LABEL_UI_NAME = "CityNameLabel";
        private static readonly string CITY_BG_UI_NAME = "MainMenuContainer";
        private static readonly string TAVERN_BUTTON_UI_NAME = "TavernButton";
        private static readonly string EXPLORE_BUTTON_UI_NAME = "ExploreButton";
        private static readonly string EXIT_BUTTON_UI_NAME = "ExitButton";
        private static readonly string LEFT_CONTAINER_UI_NAME = "LeftContainer";
        private static readonly string TAVERN_DIALOG_ROOT_UI_NAME = "TavernDialog";
        private static readonly string SITUATIONS_MENU_ROOT_UI_NAME = "CitySituationsMenu";

        [SerializeField] private InCityConfigurationSO _devCityConfiguration;
        [SerializeField] private HeroTeamHUDUIController _heroTeamHUDUIController;
        [SerializeField] private SituationsController _situationsController;
        [SerializeField] private VisualTreeAsset _situationsButtonTemplate;
        private InCityConfigurationSO _cityConfiguration;
        private LeftContainerController _leftContainerController;
        private CitySituationsMenuController _situationsMenuController;
        private TavernDialogController _tavernDialogController;

        public Action<ACitySituationSO> onCitySituationClicked;

        #region Class setup

        private void Awake()
        {
            // Get city configuration to load
            InCityConfigurationSO gameStateConf = GameStateManager.Instance.GetCityConfigurationToLoad();
            _cityConfiguration = gameStateConf != null ? gameStateConf : _devCityConfiguration;

            // Setup main menu
            _leftContainerController =
                new LeftContainerController(_uiDoc.rootVisualElement.Q<VisualElement>(LEFT_CONTAINER_UI_NAME));

            // Setup tavern dialog
            _tavernDialogController =
                new TavernDialogController(_uiDoc.rootVisualElement.Q<VisualElement>(TAVERN_DIALOG_ROOT_UI_NAME));
            _tavernDialogController.onRestButtonClicked += OnTavernDialogRestButtonClicked;
            _tavernDialogController.onExitClicked += OnTavernDialogExitButtonClicked;

            // Setup city situations
            _situationsMenuController = GetComponent<CitySituationsMenuController>();
            _situationsMenuController.onCitySituationClicked += OnSituationButtonClicked;
            _situationsMenuController.onReturnClicked += OnSituationsMenuReturnClicked;

            // Setup hero team HUD
            _heroTeamHUDUIController ??= FindObjectOfType<HeroTeamHUDUIController>();
            if (_heroTeamHUDUIController == null)
            {
                Debug.LogError("HeroTeamHUDUIController is not present in the scene.");
                return;
            }

            // Setup situations controller
            if (_situationsController == null)
            {
                Debug.LogError("SituationsController is not assigned in CityMenuController.");
                return;
            }

            _situationsController.onSituationResolved += OnSituationResolved;
        }

        #endregion

        #region Main menu setup

        private void Start()
        {
            HeroTeamQuests.Instance.InitializeQuests(this);
            SetupMainMenu();
            SceneTransitioner.FadeInCurrentScene();
        }

        private void SetupMainMenu()
        {
            // Main menu setup
            _uiDoc.rootVisualElement.Q<Label>(CITY_NAME_LABEL_UI_NAME).text = _cityConfiguration.Name;
            _uiDoc.rootVisualElement.Q<VisualElement>(CITY_BG_UI_NAME).style.backgroundImage =
                new StyleBackground(_cityConfiguration.CityBackground);
            _uiDoc.rootVisualElement.Q<Button>(TAVERN_BUTTON_UI_NAME).clicked += OnTavernButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXPLORE_BUTTON_UI_NAME).clicked += OnExploreButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;

            // Tavern dialog setup
            _tavernDialogController.SetupTavernDialog(_cityConfiguration.TavernConfiguration);

            // City situations menu setup
            _situationsMenuController.Init(
                _uiDoc.rootVisualElement.Q<VisualElement>(SITUATIONS_MENU_ROOT_UI_NAME),
                _situationsButtonTemplate
            );
            _situationsMenuController.SetupSituationsMenu(_cityConfiguration.CitySituations);
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
            _heroTeamHUDUIController.UpdateHeroContainers();
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
            _situationsMenuController.SetupSituationsMenu(_cityConfiguration.CitySituations);
            StartCoroutine(_situationsMenuController.Display());
        }

        private void OnSituationsMenuReturnClicked()
        {
            _situationsMenuController.Hide();
            _leftContainerController.Display();
        }

        private static void OnExitButtonClicked()
        {
            SceneTransitioner.TransitionToScene(EScenesName.KINGDOM.ToSceneString());
        }

        #endregion

        #region Situations interactions

        private void OnSituationButtonClicked(ACitySituationSO citySituation)
        {
            _situationsMenuController.Hide();
            onCitySituationClicked?.Invoke(citySituation);
        }

        private void OnSituationResolved(ACitySituationSO citySituation)
        {
            _situationsMenuController.SetupSituationsMenu(_cityConfiguration.CitySituations);
            StartCoroutine(_situationsMenuController.Display());
        }

        #endregion
    }
}