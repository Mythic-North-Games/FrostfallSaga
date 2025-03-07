using System;
using System.Collections;
using FrostfallSaga.Core.Cities.CitySituations;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class CitySituationsMenuController : MonoBehaviour
    {
        #region UXML Names and classes
        private static readonly string SITUATIONS_CONTAINER_UI_NAME = "SituationsContainer";
        private static readonly string SITUATION_ILLUSTRATION_UI_NAME = "SituationIllustration";
        private static readonly string SITUATION_BUTTON_UI_NAME = "SituationButton";
        private static readonly string RETURN_BUTTON_UI_NAME = "ReturnButton";

        private static readonly string SITUATIONS_MENU_HIDDEN_CLASSNAME = "situationMenuHidden";
        private static readonly string SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME = "situationIllustrationHidden";
        private static readonly string SITUATION_BUTTON_CONTAINER_DEFAULT_CLASSNAME = "situationButtonContainer";
        private static readonly string SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME = "situationButtonContainerHidden";
        #endregion

        public Action<ACitySituationSO> OnCitySituationClicked;
        public Action OnReturnClicked;

        private VisualElement _situationsMenuRoot;
        private VisualElement _situationsContainer;
        private VisualElement _situationIllustration;
        private Button _returnButton;
        private VisualTreeAsset _citySituationButtonTemplate;
        private ACitySituationSO[] _currentCitySituations;
        private float _timeBeforeSituationsButtonDisplay;
        private float _timeBetweenSituationsButtonDisplay;

        public void Init(
            VisualElement citySituationsMenuRoot,
            VisualTreeAsset citySituationButtonTemplate,
            ACitySituationSO[] citySituations,
            float timeBeforeSituationsButtonDisplay = 0.5f,
            float timeBetweenSituationsButtonDisplay = 0.2f
        )
        {
            // Assign all parameters and retreive the necessary UI elements
            _situationsMenuRoot = citySituationsMenuRoot;
            _situationsContainer = _situationsMenuRoot.Q<VisualElement>(SITUATIONS_CONTAINER_UI_NAME);
            _situationIllustration = _situationsMenuRoot.Q<VisualElement>(SITUATION_ILLUSTRATION_UI_NAME);
            _returnButton = _situationsMenuRoot.Q<Button>(RETURN_BUTTON_UI_NAME);
            _citySituationButtonTemplate = citySituationButtonTemplate;
            _timeBeforeSituationsButtonDisplay = timeBeforeSituationsButtonDisplay;
            _timeBetweenSituationsButtonDisplay = timeBetweenSituationsButtonDisplay;
            _returnButton.RegisterCallback<ClickEvent>(evt => OnReturnClicked?.Invoke());

            // Setup the situations menu
            SetupSituationsMenu(citySituations);
        }

        public void SetupSituationsMenu(ACitySituationSO[] citySituations)
        {
            _currentCitySituations = citySituations;
            _situationsContainer.Clear();
            _situationIllustration.AddToClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);

            foreach (ACitySituationSO citySituation in citySituations)
            {
                VisualElement situationButtonContainer = _citySituationButtonTemplate.Instantiate();
                situationButtonContainer.Q<Button>(SITUATION_BUTTON_UI_NAME).text = citySituation.Name;
                situationButtonContainer.AddToClassList(SITUATION_BUTTON_CONTAINER_DEFAULT_CLASSNAME);
                situationButtonContainer.AddToClassList(SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME);
                _situationsContainer.Add(situationButtonContainer);

                if (!citySituation.IsReplayable && citySituation.IsDone)
                {
                    situationButtonContainer.SetEnabled(false);
                    return;
                }

                situationButtonContainer.RegisterCallback<MouseEnterEvent>(OnSituationButtonHovered);
                situationButtonContainer.RegisterCallback<MouseLeaveEvent>(OnSituationButtonUnhovered);
                situationButtonContainer.RegisterCallback<ClickEvent>(OnSituationButtonClicked);
            }
        }

        #region Situations interactions

        private void OnSituationButtonHovered(MouseEnterEvent mouseEnterEvent)
        {
            ACitySituationSO hoveredSituation = GetSituationFromButton(mouseEnterEvent.currentTarget as VisualElement);
            _situationIllustration.style.backgroundImage = new StyleBackground(hoveredSituation.Illustration);
            _situationIllustration.RemoveFromClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);
        }

        private void OnSituationButtonUnhovered(MouseLeaveEvent mouseLeaveEvent)
        {
            _situationIllustration.AddToClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);
        }

        private void OnSituationButtonClicked(ClickEvent clickEvent)
        {
            ACitySituationSO clickedSituation = GetSituationFromButton(clickEvent.currentTarget as VisualElement);
            OnCitySituationClicked?.Invoke(clickedSituation);
        }

        private ACitySituationSO GetSituationFromButton(VisualElement situationButton)
        {
            int situationIndex = _situationsContainer.IndexOf(situationButton);
            return _currentCitySituations[situationIndex];
        }

        #endregion

        #region Menu display/hide

        public IEnumerator Display()
        {
            _situationsMenuRoot.RemoveFromClassList(SITUATIONS_MENU_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(_timeBeforeSituationsButtonDisplay);
            StartCoroutine(DisplaySituationsButton());
        }

        public void Hide()
        {
            _situationIllustration.AddToClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);
            _situationsMenuRoot.AddToClassList(SITUATIONS_MENU_HIDDEN_CLASSNAME);
        }

        private IEnumerator DisplaySituationsButton()
        {
            foreach (VisualElement situationButtonContainer in _situationsContainer.Children())
            {
                situationButtonContainer.RemoveFromClassList(SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(_timeBetweenSituationsButtonDisplay);
            }
        }

        #endregion
    }
}