using System;
using System.Collections;
using FrostfallSaga.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class LeftArrowButtonUIController
    {
        private static readonly float ARROW_ANIMATION_DURATION = 0.5f;

        #region UXML Names and classes
        private static readonly string LEFT_ARROW_UI_NAME = "LeftArrow";
        private static readonly string BUTTON_UI_NAME = "Button";

        private static readonly string LEFT_ARROW_HIDDEN_CLASSNAME = "leftArrowHidden";
        private static readonly string LEFT_ARROW_LEFT_CLASSNAME = "leftArrowLeft";
        #endregion

        public Action<LeftArrowButtonUIController> onButtonClicked;
        public VisualElement Root { get; private set; }

        private readonly Button _button;
        private readonly VisualElement _leftArrow;
        private Coroutine _arrowAnimCoroutine;
        private bool _arrowAnimToggle;
        private bool _isArrowActiveOnUnhover;

        public LeftArrowButtonUIController(VisualElement root, string answerText)
        {
            Root = root;

            _button = root.Q<Button>(BUTTON_UI_NAME);
            _button.text = answerText;
            _button.RegisterCallback<ClickEvent>(OnButtonClicked);

            _leftArrow = root.Q<VisualElement>(LEFT_ARROW_UI_NAME);
            _leftArrow.AddToClassList(LEFT_ARROW_HIDDEN_CLASSNAME);
            root.RegisterCallback<MouseEnterEvent>(OnMouseHovered);
            root.RegisterCallback<MouseLeaveEvent>(OnMouseUnhovered);

            _isArrowActiveOnUnhover = false;
        }

        public void SetArrowActiveOnUnhover(bool isActive)
        {
            _isArrowActiveOnUnhover = isActive;
        }

        public void EnableInButtonClassList(string className, bool enable)
        {
            _button.EnableInClassList(className, enable);
        }

        private void OnButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            onButtonClicked?.Invoke(this);
        }

        private void OnMouseHovered(MouseEnterEvent evt)
        {
            _leftArrow.RemoveFromClassList(LEFT_ARROW_HIDDEN_CLASSNAME);
            _arrowAnimCoroutine ??= CoroutineRunner.Instance.StartCoroutine(ArrowAnimationLoop());
        }

        private void OnMouseUnhovered(MouseLeaveEvent evt)
        {
            _leftArrow.AddToClassList(LEFT_ARROW_HIDDEN_CLASSNAME);
            if (!_isArrowActiveOnUnhover && _arrowAnimCoroutine != null)
            {
                CoroutineRunner.Instance.StopCoroutine(_arrowAnimCoroutine);
                _arrowAnimCoroutine = null;
            }
        }

        private IEnumerator ArrowAnimationLoop()
        {
            _arrowAnimToggle = true;

            while (true)
            {   
                _leftArrow.EnableInClassList(LEFT_ARROW_LEFT_CLASSNAME, _arrowAnimToggle);
                _arrowAnimToggle = !_arrowAnimToggle;
                yield return new WaitForSeconds(ARROW_ANIMATION_DURATION);
            }
        }
    }
}