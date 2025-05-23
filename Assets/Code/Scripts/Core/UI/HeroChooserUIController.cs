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
        private static readonly string HERO_ICON_FRAME_SELECTED_UI_NAME = "HeroIconFrameSelected";

        private static readonly string HERO_ICON_ACTIVE_UI_NAME = "heroIconActive";
        private static readonly string HERO_ICON_FRAME_SELECTED_ACTIVE_UI_NAME = "heroIconFrameSelectedActive";
        #endregion

        private static readonly float HOVER_TRANSLATION_AMOUNT = 20f;

        public Action<Hero> onHeroChosen;

        private readonly VisualElement _heroButtonLeft;
        private readonly VisualElement _heroButtonMiddle;
        private readonly VisualElement _heroButtonRight;
        private VisualElement _currentChosenHeroButton;
        private Hero _currentLeftHero;
        private Hero _currentMiddleHero;
        private Hero _currentRightHero;

        public HeroChooserUIController(VisualElement root)
        {
            _heroButtonLeft = root.Q(HERO_BUTTON_LEFT_UI_NAME);
            _heroButtonMiddle = root.Q(HERO_BUTTON_MIDDLE_UI_NAME);
            _heroButtonRight = root.Q(HERO_BUTTON_RIGHT_UI_NAME);

            _heroButtonLeft.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
            _heroButtonLeft.RegisterCallback<MouseEnterEvent>(OnHeroButtonHovered);
            _heroButtonLeft.RegisterCallback<MouseLeaveEvent>(OnHeroButtonUnhovered);

            _heroButtonMiddle.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
            _heroButtonMiddle.RegisterCallback<MouseEnterEvent>(OnHeroButtonHovered);
            _heroButtonMiddle.RegisterCallback<MouseLeaveEvent>(OnHeroButtonUnhovered);

            _heroButtonRight.RegisterCallback<ClickEvent>(OnHeroButtonClicked);
            _heroButtonRight.RegisterCallback<MouseEnterEvent>(OnHeroButtonHovered);
            _heroButtonRight.RegisterCallback<MouseLeaveEvent>(OnHeroButtonUnhovered);

            _currentChosenHeroButton = null;
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

        public void ActivateHero(Hero hero)
        {
            if (_currentChosenHeroButton != null) SetHeroActive(_currentChosenHeroButton, false);

            if (hero == _currentLeftHero)
            {
                _currentChosenHeroButton = _heroButtonLeft;
            }
            else if (hero == _currentMiddleHero)
            {
                _currentChosenHeroButton = _heroButtonMiddle;
            }
            else if (hero == _currentRightHero)
            {
                _currentChosenHeroButton = _heroButtonRight;
            }

            SetHeroActive(_currentChosenHeroButton, true);
            AdjustHeroButtonsTranslate(_currentChosenHeroButton);
        }

        private void OnHeroButtonClicked(ClickEvent evt)
        {
            if (_currentChosenHeroButton != null) SetHeroActive(_currentChosenHeroButton, false);

            if (evt.currentTarget == _heroButtonLeft && _currentChosenHeroButton != _heroButtonLeft)
            {
                onHeroChosen?.Invoke(_currentLeftHero);
                _currentChosenHeroButton = _heroButtonLeft;
            }
            else if (evt.currentTarget == _heroButtonMiddle && _currentChosenHeroButton != _heroButtonMiddle)
            {
                onHeroChosen?.Invoke(_currentMiddleHero);
                _currentChosenHeroButton = _heroButtonMiddle;
            }
            else if (evt.currentTarget == _heroButtonRight && _currentChosenHeroButton != _heroButtonRight)
            {
                onHeroChosen?.Invoke(_currentRightHero);
                _currentChosenHeroButton = _heroButtonRight;
            }

            SetHeroActive(_currentChosenHeroButton, true);
        }

        private void OnHeroButtonHovered(MouseEnterEvent evt)
        {
            VisualElement hoveredHeroButton = evt.currentTarget as VisualElement;
            if (_currentChosenHeroButton == hoveredHeroButton) return;

            if (_currentChosenHeroButton != null)
            {
                SetHeroActive(_currentChosenHeroButton, false);
                if (_currentLeftHero != null) _heroButtonLeft.style.translate = new();
                if (_currentMiddleHero != null) _heroButtonMiddle.style.translate = new();
                if (_currentRightHero != null) _heroButtonRight.style.translate = new();
            }

            AdjustHeroButtonsTranslate(hoveredHeroButton);
        }

        private void OnHeroButtonUnhovered(MouseLeaveEvent evt)
        {
            if (evt.currentTarget == _heroButtonLeft)
            {
                if (_currentMiddleHero != null) _heroButtonMiddle.style.translate = new();
            }
            else if (evt.currentTarget == _heroButtonMiddle)
            {
                if (_currentLeftHero != null) _heroButtonLeft.style.translate = new();
                if (_currentRightHero != null) _heroButtonRight.style.translate = new();
            }
            else if (evt.currentTarget == _heroButtonRight)
            {
                if (_currentMiddleHero != null) _heroButtonMiddle.style.translate = new();
            }

            if (_currentChosenHeroButton != null)
            {
                SetHeroActive(_currentChosenHeroButton, true);
                AdjustHeroButtonsTranslate(_currentChosenHeroButton);
            }
        }

        private void SetHeroActive(VisualElement heroButton, bool isActive)
        {
            heroButton.Q(HERO_ICON_UI_NAME).EnableInClassList(HERO_ICON_ACTIVE_UI_NAME, isActive);
            heroButton.Q(HERO_ICON_FRAME_SELECTED_UI_NAME).EnableInClassList(HERO_ICON_FRAME_SELECTED_ACTIVE_UI_NAME, isActive);
        }

        private void AdjustHeroButtonsTranslate(VisualElement activeHeroButton)
        {
            if (activeHeroButton == _heroButtonLeft)
            {
                if (_currentMiddleHero != null)
                {
                    _heroButtonMiddle.style.translate = new(
                        new Translate(
                            new(HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent),
                            new(HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent)
                        )
                    );
                }
            }
            else if (activeHeroButton == _heroButtonMiddle)
            {
                if (_currentLeftHero != null)
                {
                    _heroButtonLeft.style.translate = new(
                        new Translate(
                            new(-HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent),
                            new(-HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent)
                        )
                    );
                }
                if (_currentRightHero != null)
                {
                    _heroButtonRight.style.translate = new(
                        new Translate(
                            new(HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent),
                            new(-HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent)
                        )
                    );
                }
            }
            else if (activeHeroButton == _heroButtonRight)
            {
                if (_currentMiddleHero != null)
                {
                    _heroButtonMiddle.style.translate = new(
                        new Translate(
                            new(-HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent),
                            new(HOVER_TRANSLATION_AMOUNT, LengthUnit.Percent)
                        )
                    );
                }
            }
        }
    }
}