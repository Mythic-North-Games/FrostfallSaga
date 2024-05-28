using System;
using UnityEngine;
using FrostfallSaga.Core;

namespace FrostfallSaga.Grid.Cells
{

    /// <summary>
    /// Check mouse actions related to the cell and send events when an action has happened.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CellMouseEventsController : MonoBehaviour
    {
        public Action<Cell> OnCellHover;
        public Action<Cell> OnCellUnhover;
        public Action<Cell> OnCellClick;

        private void OnEnable()
        {
            MouseController.Instance.OnElementHover += OnMouseControllerElementHover;
            MouseController.Instance.OnElementUnhover += OnMouseControllerElementUnhover;
            MouseController.Instance.OnLeftMouseDown += OnMouseControllerLeftMouseDown;
        }

        private void OnMouseControllerElementHover(RaycastHit hoveredElement)
        {
            if (hoveredElement.transform.TryGetComponent(out CellMouseEventsController hoveredCellMouseEventsController) && hoveredCellMouseEventsController == this)
            {
                OnCellHover?.Invoke(GetComponentInParent<Cell>());
            }
        }

        private void OnMouseControllerElementUnhover(RaycastHit hoveredElement)
        {
            if (hoveredElement.transform.TryGetComponent(out CellMouseEventsController hoveredCellMouseEventsController) && hoveredCellMouseEventsController == this)
            {
                OnCellUnhover?.Invoke(GetComponentInParent<Cell>());
            }
        }

        private void OnMouseControllerLeftMouseDown(RaycastHit clickedElement)
        {
            if (clickedElement.transform.TryGetComponent(out CellMouseEventsController clickedCellMouseEventsController) && clickedCellMouseEventsController == this)
            {
                OnCellClick?.Invoke(GetComponentInParent<Cell>());
            }
        }

        private void OnDisable()
        {
            MouseController.Instance.OnElementHover -= OnMouseControllerElementHover;
            MouseController.Instance.OnElementUnhover -= OnMouseControllerElementUnhover;
            MouseController.Instance.OnLeftMouseDown -= OnMouseControllerLeftMouseDown;
        }
    }
}
