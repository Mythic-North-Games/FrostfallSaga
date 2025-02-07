using System;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    public class Cell : MonoBehaviour
    {
        [field: SerializeField] public CellData Data { get; private set; }
        public CellInteraction Interaction { get; private set; }
        public CellVisual Visual { get; private set; }
        private HexGrid _parentGrid;
        private void Awake()
        {
            Interaction = GetComponent<CellInteraction>();
            if (Interaction == null)
            {
                Debug.LogError("Interaction Script is mmissing !");
            }
            Visual = GetComponent<CellVisual>();
            if (Visual == null)
            {
                Debug.LogError("Visual Script is missing !");
            }
            _parentGrid = GetComponentInParent<HexGrid>();
            if (_parentGrid == null)
            {
                Debug.LogError("HexGrid parent is missing");
            }
        }

        public void Setup(Vector2Int coordinates, ECellHeight height, TerrainTypeSO terrainType, BiomeTypeSO biomeType)
        {
            Awake();
            transform.localScale = Vector3.one * _parentGrid.HexSize / 2.68f;
            Visual.Initializer();
            Interaction.Initializer();
            Data = new CellData(coordinates, height, terrainType, biomeType);
            UpdateHeight(height, 0f);
            SetTerrain();
        }


        public Vector3 GetCenter()
        {
            Vector3 center = HexMetrics.Center(_parentGrid.HexSize, Data.Coordinates.x, Data.Coordinates.y);
            center.y = GetYPosition();
            return center;
        }

        public float GetYPosition()
        {
            return Data.WorldHeightPerUnit + ((int)Data.Height + 1);
        }

        public static Vector2Int GetHexDirection(Cell initiatorCell, Cell targetCell)
        {
            Vector2Int initiatorAxial = initiatorCell.Data.AxialCoordinates;
            Vector2Int targetAxial = targetCell.Data.AxialCoordinates;
            return targetAxial - initiatorAxial;
        }

        public void UpdateHeight(ECellHeight newCellHeight, float duration)
        {
            Data.Height = newCellHeight;
            Visual.UpdateHeightVisual((float)newCellHeight, duration);
        }

        public void SetTerrain()
        {
            Visual.SetTerrainVisual(Data.TerrainType);
        }

        public override string ToString()
        {
            return $"Cell: \n" +
                    $"- CellData: {Data}\n" +
                    $"- Interaction: {(Interaction != null ? Interaction : "NULL")}\n" +
                    $"- VisualCell: {Visual}\n" +
                    $"- _parentGrid: {_parentGrid}";
        }
    }
}
