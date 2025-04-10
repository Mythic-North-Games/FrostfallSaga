using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.HeroTeam;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class HeroChooserUIController
    {
        private readonly Dictionary<VisualElement, Hero> _heroChoiceButtons = new();

        public Action<Hero> OnHeroChosen;

        public HeroChooserUIController(VisualElement root)
        {
            VisualElement heroChooserContainer = root.Q<VisualElement>(HERO_CHOOSER_CONTAINER_UI_NAME);
            foreach (VisualElement child in heroChooserContainer.Children())
            {
                VisualElement heroChoice = child.Q<VisualElement>(HERO_CHOICE_BUTTON_UI_NAME);
                heroChoice.RegisterCallback<ClickEvent>(_ => OnHeroChosen?.Invoke(_heroChoiceButtons[heroChoice]));
                _heroChoiceButtons.Add(heroChoice, null);
            }
        }

        public void SetHeroes(List<Hero> heroes)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                VisualElement heroChoice = _heroChoiceButtons.ElementAt(i).Key;
                heroChoice.Q<VisualElement>(HERO_ICON_UI_NAME).style.backgroundImage =
                    new(heroes[i].EntityConfiguration.DiamondIcon);
                _heroChoiceButtons[heroChoice] = heroes[i];
            }
        }

        #region UI Elements Names & Classes

        private static readonly string HERO_CHOOSER_CONTAINER_UI_NAME = "HeroChooserContainer";
        private static readonly string HERO_CHOICE_BUTTON_UI_NAME = "HeroChoiceContainer";
        private static readonly string HERO_ICON_UI_NAME = "HeroIcon";

        #endregion
    }
}