using System;
using System.Collections;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.Dialogues;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.City.UI
{
    public class CitySituationsMenuController : MonoBehaviour
    {
        #region UXML Names and classes
        private static readonly string SITUATIONS_CONTAINER_UI_NAME = "SituationsContainer";
        private static readonly string SITUATION_ILLUSTRATION_UI_NAME = "SituationIllustration";
        private static readonly string RETURN_BUTTON_UI_NAME = "ReturnButton";

        private static readonly string SITUATIONS_MENU_HIDDEN_CLASSNAME = "situationMenuHidden";
        private static readonly string SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME = "situationIllustrationHidden";
        private static readonly string SITUATION_BUTTON_CONTAINER_DEFAULT_CLASSNAME = "situationContainer";
        private static readonly string SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME = "situationContainerHidden";
        #endregion

        public Action<ACitySituationSO> onCitySituationClicked;
        public Action onReturnClicked;

        private VisualElement _situationsMenuRoot;
        private VisualElement _situationsContainer;
        private VisualTreeAsset _citySituationButtonTemplate;
        private VisualElement _situationIllustration;
        private Button _returnButton;
        private ACitySituationSO[] _currentCitySituations;

        public void Init(
            VisualElement citySituationsMenuRoot,
            VisualTreeAsset citySituationButtonTemplate
        )
        {
            // Assign all parameters and retreive the necessary UI elements
            _situationsMenuRoot = citySituationsMenuRoot;
            SetPickingMode(_situationsMenuRoot, PickingMode.Ignore);
            _situationsContainer = _situationsMenuRoot.Q<VisualElement>(SITUATIONS_CONTAINER_UI_NAME);
            _situationIllustration = _situationsMenuRoot.Q<VisualElement>(SITUATION_ILLUSTRATION_UI_NAME);
            _returnButton = _situationsMenuRoot.Q<Button>(RETURN_BUTTON_UI_NAME);
            _citySituationButtonTemplate = citySituationButtonTemplate;
            _returnButton.RegisterCallback<ClickEvent>(evt => onReturnClicked?.Invoke());
        }

        public void SetupSituationsMenu(ACitySituationSO[] citySituations)
        {
            _currentCitySituations = citySituations;
            _situationsContainer.Clear();
            _situationIllustration.AddToClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);

            foreach (ACitySituationSO citySituation in citySituations)
            {
                VisualElement situationButtonContainer = _citySituationButtonTemplate.Instantiate();
                situationButtonContainer.AddToClassList(SITUATION_BUTTON_CONTAINER_DEFAULT_CLASSNAME);
                situationButtonContainer.AddToClassList(SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME);

                if (!citySituation.IsReplayable && citySituation.IsDone)
                {
                    situationButtonContainer.SetEnabled(false);
                    return;
                }

                situationButtonContainer.RegisterCallback<MouseEnterEvent>(OnSituationButtonHovered);
                situationButtonContainer.RegisterCallback<MouseLeaveEvent>(OnSituationButtonUnhovered);

                LeftArrowButtonUIController situationButtonController = new(situationButtonContainer, citySituation.Name);
                situationButtonController.onButtonClicked += OnSituationButtonClicked;

                _situationsContainer.Add(situationButtonContainer);
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

        private void OnSituationButtonClicked(LeftArrowButtonUIController situationButtonController)
        {
            ACitySituationSO clickedSituation = GetSituationFromButton(situationButtonController.Root);
            onCitySituationClicked?.Invoke(clickedSituation);
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
            yield return new WaitForSeconds(0.3f);
            _situationsMenuRoot.RemoveFromClassList(SITUATIONS_MENU_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(DisplaySituationsButtons());
            SetPickingMode(_situationsMenuRoot, PickingMode.Position);
        }

        public IEnumerator Hide()
        {
            SetPickingMode(_situationsMenuRoot, PickingMode.Ignore);
            _situationIllustration.AddToClassList(SITUATION_ILLUSTRATION_HIDDEN_CLASSNAME);
            StartCoroutine(HideSituationsButtons());
            yield return new WaitForSeconds(0.4f * _currentCitySituations.Length);
            _situationsMenuRoot.AddToClassList(SITUATIONS_MENU_HIDDEN_CLASSNAME);
        }

        private IEnumerator DisplaySituationsButtons()
        {
            foreach (VisualElement situationButtonContainer in _situationsContainer.Children())
            {
                situationButtonContainer.RemoveFromClassList(SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator HideSituationsButtons()
        {
            foreach (VisualElement situationButtonContainer in _situationsContainer.Children())
            {
                situationButtonContainer.AddToClassList(SITUATION_BUTTON_CONTAINER_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SetPickingMode(VisualElement element, PickingMode mode)
        {
            element.pickingMode = mode;
            foreach (VisualElement child in element.Children())
            {
                SetPickingMode(child, mode);
            }
        }

        #endregion
    }
}