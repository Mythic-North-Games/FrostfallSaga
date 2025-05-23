using System.Collections.Generic;
using FrostfallSaga.Core.HeroTeam;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class HeroTeamHUDUIController : BaseUIController
    {
        #region UXML Names & Classes
        private static readonly string HERO_CONTAINER_LEFT_UI_NAME = "HeroContainerLeft";
        private static readonly string HERO_CONTAINER_MIDDLE_UI_NAME = "HeroContainerMiddle";
        private static readonly string HERO_CONTAINER_RIGHT_UI_NAME = "HeroContainerRight";
        #endregion

        private VisualElement _heroContainerLeft;
        private VisualElement _heroContainerMiddle;
        private VisualElement _heroContainerRight;

        private void Awake()
        {
            _heroContainerLeft = _uiDoc.rootVisualElement.Q(HERO_CONTAINER_LEFT_UI_NAME);
            _heroContainerMiddle = _uiDoc.rootVisualElement.Q(HERO_CONTAINER_MIDDLE_UI_NAME);
            _heroContainerRight = _uiDoc.rootVisualElement.Q(HERO_CONTAINER_RIGHT_UI_NAME);
        }

        private void Start()
        {
            UpdateHeroContainers();
        }

        public void UpdateHeroContainers()
        {
            List<Hero> heroes = HeroTeam.HeroTeam.Instance.Heroes;

            UpdateHeroContainer(heroes[0], _heroContainerLeft);

            _heroContainerMiddle.style.display = DisplayStyle.None;
            _heroContainerRight.style.display = DisplayStyle.None;

            if (heroes.Count > 1)
            {
                _heroContainerMiddle.style.display = DisplayStyle.Flex;
                UpdateHeroContainer(heroes[1], _heroContainerMiddle);
            }

            if (heroes.Count > 2)
            {
                _heroContainerRight.style.display = DisplayStyle.Flex;
                UpdateHeroContainer(heroes[2], _heroContainerRight);
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