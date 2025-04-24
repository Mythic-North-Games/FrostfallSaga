using System.Collections;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    public class CellPositionController
    {
        private readonly float _cellSize;
        private readonly Transform _cellTransform;
        private readonly Vector2Int _coordinates;
        private readonly float _worldHeightPerUnit;

        public CellPositionController(Transform cellTransform, float worldHeightPerUnit, float cellSize,
            Vector2Int coordinates)
        {
            _cellTransform = cellTransform;
            _worldHeightPerUnit = worldHeightPerUnit;
            _cellSize = cellSize;
            _coordinates = coordinates;
        }

        public Vector3 GetCenter(ECellHeight height)
        {
            Vector3 center = HexMetrics.Center(_cellSize, _coordinates.x, _coordinates.y);
            center.y = GetYPosition(height);
            return center;
        }

        public float GetYPosition(ECellHeight height)
        {
            return _worldHeightPerUnit + ((int)height + 1);
        }

        public void SetInstanceHeight(ECellHeight height)
        {
            _cellTransform.position = new Vector3(_cellTransform.position.x, (float)height, _cellTransform.position.z);
        }

        public IEnumerator SmoothMoveToHeight(ECellHeight height, float duration)
        {
            float startY = _cellTransform.position.y;
            float endY = (float)height;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newY = Mathf.Lerp(startY, endY, elapsedTime / duration);
                _cellTransform.position = new Vector3(_cellTransform.position.x, newY, _cellTransform.position.z);
                yield return null;
            }
            _cellTransform.position = new Vector3(_cellTransform.position.x, endY, _cellTransform.position.z);
        }
    }
}