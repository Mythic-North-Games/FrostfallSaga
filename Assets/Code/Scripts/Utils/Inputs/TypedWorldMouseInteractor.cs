using System;
using UnityEngine;

namespace FrostfallSaga.Utils.Inputs
{
    public class TypedWorldMouseInteractor<T> where T : Component
    {
        public Action<T> onLeftDown;
        public Action<T> onRightDown;
        public Action<T> onMiddleDown;

        public Action<T> onLeftUp;
        public Action<T> onRightUp;
        public Action<T> onMiddleUp;

        public Action<T> onLeftClickHold;
        public Action<T> onRightClickHold;
        public Action<T> onMiddleClickHold;

        public Action<T> onLeftClickHoldReleased;
        public Action<T> onRightClickHoldReleased;
        public Action<T> onMiddleClickHoldReleased;

        public Action<T> onHovered;
        public Action<T> onUnhovered;

        public TypedWorldMouseInteractor()
        {
            GlobalWorldMouseInteractor.Instance.onLeftUp += HandleLeftUp;
            GlobalWorldMouseInteractor.Instance.onLeftDown += HandleLeftDown;
            GlobalWorldMouseInteractor.Instance.onRightUp += HandleRightUp;
            GlobalWorldMouseInteractor.Instance.onRightDown += HandleRightDown;
            GlobalWorldMouseInteractor.Instance.onMiddleUp += HandleMiddleUp;
            GlobalWorldMouseInteractor.Instance.onMiddleDown += HandleMiddleDown;
            GlobalWorldMouseInteractor.Instance.onHovered += HandleHovered;
            GlobalWorldMouseInteractor.Instance.onUnhovered += HandleUnhovered;
            GlobalWorldMouseInteractor.Instance.onLeftClickHold += HandleLeftClickHold;
            GlobalWorldMouseInteractor.Instance.onRightClickHold += HandleRightClickHold;
            GlobalWorldMouseInteractor.Instance.onMiddleClickHold += HandleMiddleClickHold;
            GlobalWorldMouseInteractor.Instance.onLeftClickHoldReleased += HandleLeftClickHoldReleased;
            GlobalWorldMouseInteractor.Instance.onRightClickHoldReleased += HandleRightClickHoldReleased;
            GlobalWorldMouseInteractor.Instance.onMiddleClickHoldReleased += HandleMiddleClickHoldReleased;
        }

        private void HandleLeftUp(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onLeftUp?.Invoke(component);
        }

        private void HandleLeftDown(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onLeftDown?.Invoke(component);
        }

        private void HandleRightUp(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onRightUp?.Invoke(component);
        }

        private void HandleRightDown(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onRightDown?.Invoke(component);
        }

        private void HandleMiddleUp(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onMiddleUp?.Invoke(component);
        }

        private void HandleMiddleDown(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onMiddleDown?.Invoke(component);
        }

        private void HandleHovered(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onHovered?.Invoke(component);
        }

        private void HandleUnhovered(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onUnhovered?.Invoke(component);
        }

        private void HandleLeftClickHold(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onLeftClickHold?.Invoke(component);
        }

        private void HandleRightClickHold(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onRightClickHold?.Invoke(component);
        }

        private void HandleMiddleClickHold(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onMiddleClickHold?.Invoke(component);
        }

        private void HandleLeftClickHoldReleased(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onLeftClickHoldReleased?.Invoke(component);
        }

        private void HandleRightClickHoldReleased(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onRightClickHoldReleased?.Invoke(component);
        }

        private void HandleMiddleClickHoldReleased(GameObject go)
        {
            if (go.TryGetComponent<T>(out var component))
                onMiddleClickHoldReleased?.Invoke(component);
        }
    }
}
