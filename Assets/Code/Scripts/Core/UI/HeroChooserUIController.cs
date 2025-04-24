using System;
using System.Collections.Generic;
using FrostfallSaga.Core.HeroTeam;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class HeroChooserUIController
    {
        #region UI Elements Names & Classes
        private static readonly string HERO_BUTTON_LEFT_UI_NAME = "HeroChoiceLeft";
        private static readonly string HERO_BUTTON_MIDDLE_UI_NAME = "HeroChoiceMiddle";
        private static readonly string HERO_BUTTON_RIGHT_UI_NAME = "HeroChoiceRight";
        private static readonly string HERO_ICON_UI_NAME = "HeroIcon";
        #endregion

        public Action<Hero> onHeroChosen;

        private readonly VisualElement _heroButtonLeft;
        private readonly VisualElement _heroButtonMiddle;
        private readonly VisualElement _heroButtonRight;
        private Hero _currentLeftHero;
        private Hero _currentMiddleHero;
        private Hero _currentRightHero;

        public HeroChooserUIController(VisualElement root)
        {
            _heroButtonLeft = root.Q(HERO_BUTTON_LEFT_UI_NAME);
            _heroButtonMiddle = root.Q(HERO_BUTTON_MIDDLE_UI_NAME);
            _heroButtonRight = root.Q(HERO_BUTTON_RIGHT_UI_NAME);

            _heroButtonLeft.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
            _heroButtonMiddle.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
            _heroButtonRight.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
        }

        public void SetHeroes(List<Hero> heroes)
        {
            _heroButtonLeft.style.display = DisplayStyle.Flex;
            _heroButtonLeft.Q(HERO_ICON_UI_NAME).style.backgroundImage = new(
                heroes[0].EntityConfiguration.DiamondIcon
            );
            _currentLeftHero = heroes[0];

            _heroButtonMiddle.style.display = DisplayStyle.None;
            _heroButtonRight.style.display = DisplayStyle.None;

            if (heroes.Count > 1)
            {
                _heroButtonMiddle.style.display = DisplayStyle.Flex;
                _heroButtonMiddle.Q(HERO_ICON_UI_NAME).style.backgroundImage = new(
                    heroes[1].EntityConfiguration.DiamondIcon
                );
                _currentMiddleHero = heroes[1];
            }

            if (heroes.Count > 2)
            {
                _heroButtonRight.style.display = DisplayStyle.Flex;
                _heroButtonRight.Q(HERO_ICON_UI_NAME).style.backgroundImage = new(
                    heroes[2].EntityConfiguration.DiamondIcon
                );
                _currentRightHero = heroes[2];
            }
        }

        private void OnHeroButtonClicked(ClickEvent evt)
        {
            if (evt.currentTarget == _heroButtonLeft)
            {
                onHeroChosen?.Invoke(_currentLeftHero);
            }
            else if (evt.currentTarget == _heroButtonMiddle)
            {
                onHeroChosen?.Invoke(_currentMiddleHero);
            }
            else if (evt.currentTarget == _heroButtonRight)
            {
                onHeroChosen?.Invoke(_currentRightHero);
            }
        }
    }
}