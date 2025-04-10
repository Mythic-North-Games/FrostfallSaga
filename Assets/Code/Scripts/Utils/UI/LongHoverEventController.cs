using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Utils.UI
{
    public class LongHoverEventController<T> where T : VisualElement
    {
        private readonly float _hoverDuration;

        private readonly T _targetElement;
        private IVisualElementScheduledItem _hoverCheckItem;
        private float _hoverStartTime;
        private bool _isHovering;
        public Action<T> onElementLongHovered;
        public Action<T> onElementLongUnhovered;

        public LongHoverEventController(T element, float duration = 1f)
        {
            _targetElement = element;
            _hoverDuration = duration;

            _targetElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            _targetElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            _isHovering = true;
            _hoverStartTime = Time.time;
            _hoverCheckItem = _targetElement.schedule.Execute(CheckHoverTime).Every(100);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            _isHovering = false;
            _hoverCheckItem.Pause();
            onElementLongUnhovered?.Invoke(_targetElement);
            _hoverCheckItem = null;
        }

        private void CheckHoverTime()
        {
            if (_isHovering && (Time.time - _hoverStartTime) >= _hoverDuration)
            {
                onElementLongHovered?.Invoke(_targetElement);
                _isHovering = false; // Prevent multiple triggers
            }
        }
    }
}