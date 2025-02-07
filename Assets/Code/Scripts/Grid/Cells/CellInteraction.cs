using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class CellInteraction : MonoBehaviour
    {
        public CellMouseEventsController CellMouseEventsController { get; private set; }

        private void Awake()
        {
            CellMouseEventsController = GetComponent<CellMouseEventsController>();
            if (CellMouseEventsController == null)
            {
                Debug.LogError("CellInteraction : CellMouseEventsController not found.");
            }
        }

        public void Initializer()
        {
            Awake();
        }
    }
}
