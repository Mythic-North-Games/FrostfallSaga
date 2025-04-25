using System;
using UnityEngine;

namespace FrostfallSaga.Core.GameState.Grid
{
    [Serializable]
    public class CellVisualData
    {
        public string prefabPath;
        public Vector3 position;
        public Quaternion rotation;

        public CellVisualData(string prefabPath, Vector3 position, Quaternion rotation)
        {
            this.prefabPath = prefabPath;
            this.position = position;
            this.rotation = rotation;
        }
    }
}