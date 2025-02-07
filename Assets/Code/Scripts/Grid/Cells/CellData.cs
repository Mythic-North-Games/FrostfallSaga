using System;
using FrostfallSaga.Grid.Cells;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    [Serializable]
    public class CellData
    {
        [field: SerializeField] public Vector2Int Coordinates { get; private set; }
        [field: SerializeField] public Vector2Int AxialCoordinates { get; private set; }
        [field: SerializeField] public TerrainTypeSO TerrainType { get; set; }
        [field: SerializeField] public BiomeTypeSO BiomeType { get; private set; }
        [field: SerializeField] public ECellHeight Height { get; set; }
        [field: SerializeField] public float WorldHeightPerUnit { get; private set; } = 0.8f;

        public CellData(Vector2Int coordinates, ECellHeight height, TerrainTypeSO terrainType, BiomeTypeSO biomeType)
        {
            Coordinates = coordinates;
            Height = height;
            TerrainType = terrainType;
            BiomeType = biomeType;
            AxialCoordinates = HexMetrics.OffsetToAxial(Coordinates);
        }

        public bool IsTerrainAccessible() => TerrainType.IsAccessible;
        public bool IsFree() => TerrainType.IsAccessible;

        public override string ToString()
        {
            return $"CellData: \n" +
                    $"- Coordinates: {Coordinates}\n" +
                    $"- AxialCoordinates: {AxialCoordinates}\n" +
                    $"- TerrainType: {TerrainType}\n" +
                    $"- BiomeType: {BiomeType}\n" +
                    $"- Height: {Height.ToString()}\n" +
                    $"- WorldHeightPerUnit: {WorldHeightPerUnit}";
        }

    }
}
