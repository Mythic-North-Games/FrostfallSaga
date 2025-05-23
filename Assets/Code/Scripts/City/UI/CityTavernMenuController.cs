using System;
using System.Collections;
using FrostfallSaga.Audio;
using FrostfallSaga.Core.Cities;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class CityTavernMenuController
    {
        private static readonly string HIGH_MARGIN_BOTTOM_LETTERS = "g";

        #region UXML Names and classes
        private static readonly string TAVERN_MENU_CONTAINER_UI_NAME = "TavernMenuContainer";
        private static readonly string TAVERN_LEFT_CONTAINER_UI_NAME = "TavernLeftContainer";
        private static readonly string TAVERN_NAME_LABEL_UI_NAME = "TavernNameLabel";
        private static readonly string REST_BUTTON_UI_NAME = "RestButton";
        private static readonly string REST_COST_LABEL_UI_NAME = "RestCostLabel";
        private static readonly string STYCAS_COUNT_LABEL_UI_NAME = "StycasCountLabel";
        private static readonly string HERO_1_STATE_CONTAINER = "Hero1State";
        private static readonly string HERO_2_STATE_CONTAINER = "Hero2State";
        private static readonly string HERO_3_STATE_CONTAINER = "Hero3State";
        private static readonly string EXIT_BUTTON_UI_NAME = "ExitButton";

        private static readonly string TAVERN_MENU_HIDDEN_CLASSNAME = "cityTavernMenuRootHidden";
        private static readonly string TAVERN_LEFT_CONTAINER_HIDDEN_CLASSNAME = "tavernLeftContainerHidden";
        private static readonly string TAVERN_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME = "tavernNameLabelHighMarginBottom";
        #endregion

        public Action onExitClicked;

        private readonly VisualElement _root;
        private readonly VisualElement _menuContainer;
        private readonly VisualElement _leftContainer;
        private readonly Label _tavernNameLabel;
        private readonly Button _restButton;
        private readonly Label _restCostLabel;
        private readonly Label _stycasCountLabel;
        private readonly VisualElement _hero1StateContainer;
        private readonly VisualElement _hero2StateContainer;
        private readonly VisualElement _hero3StateContainer;

        private readonly HeroTeam _heroTeam;

        private TavernConfiguration _tavernConfiguration;

        public CityTavernMenuController(VisualElement tavernDialogRoot)
        {
            _root = tavernDialogRoot;
            _menuContainer = _root.Q<VisualElement>(TAVERN_MENU_CONTAINER_UI_NAME);
            _leftContainer = _root.Q<VisualElement>(TAVERN_LEFT_CONTAINER_UI_NAME);
            _tavernNameLabel = _root.Q<Label>(TAVERN_NAME_LABEL_UI_NAME);
            _restButton = _root.Q<Button>(REST_BUTTON_UI_NAME);
            _restCostLabel = _root.Q<Label>(REST_COST_LABEL_UI_NAME);
            _stycasCountLabel = _root.Q<Label>(STYCAS_COUNT_LABEL_UI_NAME);
            _hero1StateContainer = _root.Q<VisualElement>(HERO_1_STATE_CONTAINER);
            _hero2StateContainer = _root.Q<VisualElement>(HERO_2_STATE_CONTAINER);
            _hero3StateContainer = _root.Q<VisualElement>(HERO_3_STATE_CONTAINER);

            _heroTeam = HeroTeam.Instance;

            _root.Q<Button>(EXIT_BUTTON_UI_NAME).clicked += OnExitButtonClicked;
        }

        public void SetupTavernMenu(TavernConfiguration tavernConfiguration)
        {
            _tavernConfiguration = tavernConfiguration;

            _menuContainer.style.backgroundImage = new(tavernConfiguration.TavernIllustration);
            _tavernNameLabel.text = tavernConfiguration.Name;
            AdjustTavernNameLabelMargin();
            _restCostLabel.text = tavernConfiguration.RestCost.ToString();
            UpdateHeroTeamStycasLabel();
            UpdateHeroTeamState();

            bool canPayRest = _heroTeam.Stycas >= tavernConfiguration.RestCost;
            _restButton.SetEnabled(canPayRest);
            if (canPayRest) _root.Q<Button>(REST_BUTTON_UI_NAME).clicked += OnRestButtonClicked;
        }

        private void AdjustTavernNameLabelMargin()
        {
            foreach (char letter in _tavernNameLabel.text)
            {
                if (HIGH_MARGIN_BOTTOM_LETTERS.Contains(letter))
                {
                    _tavernNameLabel.AddToClassList(TAVERN_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME);
                    return;
                }
            }
            _tavernNameLabel.RemoveFromClassList(TAVERN_NAME_LABEL_HIGH_MARGIN_BOTTOM_CLASSNAME);
        }

        public IEnumerator Display()
        {
            UpdateHeroTeamStycasLabel();
            UpdateHeroTeamState();
            if (_tavernConfiguration.TavernMusic != null) AudioManager.Instance.PlayMusic(_tavernConfiguration.TavernMusic);
            yield return new WaitForSeconds(0.3f);
            _root.RemoveFromClassList(TAVERN_MENU_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.3f);
            _leftContainer.RemoveFromClassList(TAVERN_LEFT_CONTAINER_HIDDEN_CLASSNAME);
        }

        public IEnumerator Hide()
        {
            _leftContainer.AddToClassList(TAVERN_LEFT_CONTAINER_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.3f);
            _root.AddToClassList(TAVERN_MENU_HIDDEN_CLASSNAME);
        }

        private void OnRestButtonClicked()
        {
            _heroTeam.WithdrawStycas(_tavernConfiguration.RestCost);
            _heroTeam.FullHealTeam();

            UpdateHeroTeamStycasLabel();
            UpdateHeroTeamState();
        }

        private void OnExitButtonClicked()
        {
            onExitClicked?.Invoke();
        }

        private void UpdateHeroTeamStycasLabel()
        {
            _stycasCountLabel.text = _heroTeam.Stycas.ToString();
        }

        private void UpdateHeroTeamState()
        {
            UpdateHeroContainer(_heroTeam.Heroes[0], _hero1StateContainer);

            _hero2StateContainer.style.display = DisplayStyle.None;
            _hero3StateContainer.style.display = DisplayStyle.None;

            if (_heroTeam.Heroes.Count > 1)
            {
                _hero2StateContainer.style.display = DisplayStyle.Flex;
                UpdateHeroContainer(_heroTeam.Heroes[1], _hero2StateContainer);
            }
            if (_heroTeam.Heroes.Count > 2)
            {
                _hero3StateContainer.style.display = DisplayStyle.Flex;
                UpdateHeroContainer(_heroTeam.Heroes[2], _hero3StateContainer);
            }
        }

        private void UpdateHeroContainer(Hero hero, VisualElement heroContainer)
        {
            CharacterStateContainerUIController.Setup(
                root: heroContainer,
                diamondIcon: hero.EntityConfiguration.DiamondIcon,
                currentHealth: hero.PersistedFighterConfiguration.Health,
                maxHealth: hero.PersistedFighterConfiguration.MaxHealth
            );
        }
    }
}