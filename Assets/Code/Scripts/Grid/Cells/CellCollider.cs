using System;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    /// <summary>
    ///     Specific collider used to collides only with Cells.
    ///     Meant to be included in weapons and projectiles.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CellCollider : MonoBehaviour
    {
        public Action<Cell> OnCellEnter;
        public Action<Cell> OnCellExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Cell cell)) OnCellEnter?.Invoke(cell);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Cell cell)) OnCellExit?.Invoke(cell);
        }
    }
}