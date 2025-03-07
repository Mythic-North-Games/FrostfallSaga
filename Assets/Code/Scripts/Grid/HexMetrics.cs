using UnityEngine;

namespace FrostfallSaga.Grid
{
    /// <summary>
    ///     HexMetrics provides all the maths methods needed to create and manage the HexGrid.
    /// </summary>
    public static class HexMetrics
    {
        private const float INNER_RADIUS_RATIO = 0.866025404f;

        public static float OuterRadius(float hexSize)
        {
            return hexSize;
        }

        public static float InnerRadius(float hexSize)
        {
            return hexSize * INNER_RADIUS_RATIO;
        }

        public static Vector3[] Corners(float hexSize)
        {
            Vector3[] corners = new Vector3[6];
            for (var i = 0; i < 6; i++) corners[i] = Corner(hexSize, i);
            return corners;
        }

        private static Vector3 Corner(float hexSize, int index)
        {
            var angle = 60f * index + 30f;
            Vector3 corner = new(hexSize * Mathf.Cos(angle * Mathf.Deg2Rad), 0f,
                hexSize * Mathf.Sin(angle * Mathf.Deg2Rad));
            return corner;
        }

        public static Vector3 Center(float hexSize, int x, int z)
        {
            Vector3 centerPosition;
            centerPosition.x = (x + z * 0.5f - z / 2) * (InnerRadius(hexSize) * 2f);
            centerPosition.y = 0f;
            centerPosition.z = z * (OuterRadius(hexSize) * 1.5f);

            return centerPosition;
        }

        public static Vector2Int OffsetToAxial(Vector2Int offsetCoords)
        {
            var q = offsetCoords.x - (offsetCoords.y - (offsetCoords.y & 1)) / 2;
            var r = offsetCoords.y;
            return new Vector2Int(q, r);
        }

        public static Vector2Int AxialToOffset(Vector2Int axialCoords)
        {
            var x = axialCoords.x + (axialCoords.y - (axialCoords.y & 1)) / 2;
            var y = axialCoords.y;
            return new Vector2Int(x, y);
        }

        public static Vector2Int RotateAxialVector(Vector2Int axialVector, Vector2Int direction)
        {
            // Define the rotation matrix based on direction (you can add more cases if needed)
            if (direction.x == 1 && direction.y == -1)
                return new Vector2Int(axialVector.y + axialVector.x, -axialVector.x);
            if (direction.x == 0 && direction.y == -1) return new Vector2Int(axialVector.y, -axialVector.x);
            if (direction.x == 0 && direction.y == 1) return new Vector2Int(axialVector.y, axialVector.x);
            if (direction.x == -1 && direction.y == 0) return new Vector2Int(-axialVector.x, -axialVector.y);
            if (direction.x == -1 && direction.y == 1)
                return new Vector2Int(-axialVector.x - axialVector.y, axialVector.x);
            return axialVector;
        }
    }
}