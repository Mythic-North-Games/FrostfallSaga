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
        public Action<Cell> onCellEnter;
        public Action<Cell> onCellExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Cell cell)) onCellEnter?.Invoke(cell);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Cell cell)) onCellExit?.Invoke(cell);
        }
    }
}