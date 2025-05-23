using System;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.GameState;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Audio;
using FrostfallSaga.Utils.Scenes;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace FrostfallSaga.City.UI
{
    public class CityMenuController : BaseUIController
    {
        private static readonly string HIGH_MARGIN_BOTTOM_LETTERS = "g";

        #region UXML Names & classes
        private static readonly string CITY_NAME_LABEL_UI_NAME = "CityNameLabel";
        private static readonly string CITY_BG_UI_NAME = "MainMenuContainer";
        private static readonly string TAVERN_BUTTON_UI_NAME = "TavernButton";
        private static readonly string EXPLORE_BUTTON_UI_NAME = "ExploreButton";
        private static readonly string EXIT_BUTTON_UI_NAME = "ExitButton";
        private static readonly string LEFT_CONTAINER_UI_NAME = "LeftContainer";
        private static readonly string TAVERN_MENU_ROOT_UI_NAME = "CityTavernMenu";
        private static readonly string SITUATIONS_MENU_ROOT_UI_NAME = "CitySituationsMenu";

        private static readonly string CITY_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME = "cityNameLabelHighMarginBottom";
        #endregion

        [SerializeField] private InCityConfigurationSO _devCityConfiguration;
        [SerializeField] private SituationsController _situationsController;
        [SerializeField] private VisualTreeAsset _situationsButtonTemplate;
        [SerializeField] private float _menuLaunchDelay = 1f;
        private InCityConfigurationSO _cityConfiguration;
        private LeftContainerController _leftContainerController;
        private CitySituationsMenuController _situationsMenuController;
        private CityTavernMenuController _tavernMenuController;

        public Action<ACitySituationSO> onCitySituationClicked;

        #region Class setup

        private void Awake()
        {
            // Get city configuration to load
            InCityConfigurationSO gameStateConf = GameStateManager.Instance.GetCityConfigurationToLoad();
            _cityConfiguration = gameStateConf != null ? gameStateConf : _devCityConfiguration;

            // Setup main menu
            _leftContainerController = new LeftContainerController(
                _uiDoc.rootVisualElement.Q<VisualElement>(LEFT_CONTAINER_UI_NAME)
            );

            // Setup tavern menu
            _tavernMenuController = new CityTavernMenuController(
                _uiDoc.rootVisualElement.Q<VisualElement>(TAVERN_MENU_ROOT_UI_NAME)
            );
            _tavernMenuController.onExitClicked += OnTavernMenuExitButtonClicked;

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

        #region Main menu setup

        private void Start()
        {
            HeroTeamQuests.Instance.InitializeQuests(this);
            SetupMainMenu();
            StartCoroutine(LaunchMainMenu());
        }

        private void SetupMainMenu()
        {
            // Main menu setup
            _uiDoc.rootVisualElement.Q<Label>(CITY_NAME_LABEL_UI_NAME).text = _cityConfiguration.Name;
            AdjustCityNameLabelMargin();

            _uiDoc.rootVisualElement.Q<VisualElement>(CITY_BG_UI_NAME).style.backgroundImage =
                new StyleBackground(_cityConfiguration.CityBackground);
            _uiDoc.rootVisualElement.Q<Button>(TAVERN_BUTTON_UI_NAME).clicked += OnTavernButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXPLORE_BUTTON_UI_NAME).clicked += OnExploreButtonClicked;
            _uiDoc.rootVisualElement.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;

            // Tavern menu setup
            _tavernMenuController.SetupTavernMenu(_cityConfiguration.TavernConfiguration);

            // City situations menu setup
            _situationsMenuController.Init(
                _uiDoc.rootVisualElement.Q<VisualElement>(SITUATIONS_MENU_ROOT_UI_NAME),
                _situationsButtonTemplate
            );
            _situationsMenuController.SetupSituationsMenu(_cityConfiguration.CitySituations);
        }

        private void AdjustCityNameLabelMargin()
        {
            Label cityNameLabel = _uiDoc.rootVisualElement.Q<Label>(CITY_NAME_LABEL_UI_NAME);
            foreach (char letter in cityNameLabel.text)
            {
                if (HIGH_MARGIN_BOTTOM_LETTERS.Contains(letter))
                {
                    cityNameLabel.AddToClassList(CITY_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME);
                    return;
                }
            }
            cityNameLabel.RemoveFromClassList(CITY_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME);
        }

        private IEnumerator LaunchMainMenu()
        {
            if (_cityConfiguration.CityMusic != null) AudioManager.Instance.PlayMusic(_cityConfiguration.CityMusic);
            SceneTransitioner.FadeInCurrentScene();
            yield return new WaitForSeconds(_menuLaunchDelay);
            _leftContainerController.Display();
        }

        #endregion

        #region Navigation

        private void OnTavernButtonClicked()
        {
            _leftContainerController.Hide();
            StartCoroutine(_tavernMenuController.Display());
        }
        private void OnTavernMenuExitButtonClicked()
        {
            if (_cityConfiguration.CityMusic != null) AudioManager.Instance.PlayMusic(_cityConfiguration.CityMusic);
            StartCoroutine(_tavernMenuController.Hide());
            StartCoroutine(WaitAndDispayLeftContainer(0.6f));
        }

        private void OnExploreButtonClicked()
        {
            _leftContainerController.Hide();
            _situationsMenuController.SetupSituationsMenu(_cityConfiguration.CitySituations);
            StartCoroutine(_situationsMenuController.Display());
        }

        private void OnSituationsMenuReturnClicked()
        {
            StartCoroutine(_situationsMenuController.Hide());
            StartCoroutine(WaitAndDispayLeftContainer(0.3f + 0.4f * _cityConfiguration.CitySituations.Length));
        }

        private void OnExitButtonClicked()
        {
            if (_cityConfiguration.CityMusic != null) AudioManager.Instance.StopCurrentMusic();
            SceneTransitioner.TransitionToScene(EScenesName.KINGDOM.ToSceneString());
        }

        private IEnumerator WaitAndDispayLeftContainer(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _leftContainerController.Display();
        }

        #endregion

        #region Situations interactions

        private void OnSituationButtonClicked(ACitySituationSO citySituation)
        {
            StartCoroutine(_situationsMenuController.Hide());
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