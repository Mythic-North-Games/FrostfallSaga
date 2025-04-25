using FrostfallSaga.Core.GameState.Grid;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    public class KingdomCellBuilder : MonoBehaviourPersistingSingleton<KingdomCellBuilder>
    {
        static KingdomCellBuilder()
        {
            PersistAcrossScenes = false;
        }

        /// <summary>
        /// Builds a kingdom cell from saved data.
        /// </summary>
        public static KingdomCell BuildKingdomCell(KingdomCellData kingdomCellData, KingdomHexGrid grid)
        {
            // Retrieve the cell prefab from resources
            GameObject cellPrefab = Resources.Load<GameObject>(kingdomCellData.prefabPath);
            if (cellPrefab == null)
            {
                Debug.LogError($"Prefab not found at path: {kingdomCellData.prefabPath}");
                return null;
            }

            // Instantiate prefab at previous position
            Vector3 cellPosition = HexMetrics.Center(AHexGrid.CELL_SIZE, kingdomCellData.cellX, kingdomCellData.cellY);
            KingdomCell kingdomCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, grid.transform).GetComponent<KingdomCell>();

            // Set the data in the cell script
            kingdomCell.Setup(
                coordinates: new Vector2Int(kingdomCellData.cellX, kingdomCellData.cellY),
                cellHeight: kingdomCellData.height,
                terrainType: kingdomCellData.terrainType,
                biomeType: kingdomCellData.biomeType
            );
            kingdomCell.LoadAccessibility(kingdomCellData.isAccessible);

            // Set the cell visual
            if (kingdomCellData.cellVisualData != null)
            {
                kingdomCell.LoadVisual(
                    terrainType: kingdomCellData.terrainType,
                    visualPrefabPath: kingdomCellData.cellVisualData.prefabPath,
                    visualPosition: kingdomCellData.cellVisualData.position,
                    visualRotation: kingdomCellData.cellVisualData.rotation
                );
            }
            else
            {
                kingdomCell.UpdateBaseCellMaterial();
            }

            return kingdomCell;
        }

        /// <summary>
        /// Extract the data from a KingdomCell.
        /// </summary>
        public static KingdomCellData ExtractInterestPointData(KingdomCell kingdomCell)
        {
            return new KingdomCellData(
                cellX: kingdomCell.Coordinates.x,
                cellY: kingdomCell.Coordinates.y,
                terrainType: kingdomCell.TerrainType,
                biomeType: kingdomCell.BiomeType,
                height: kingdomCell.Height,
                isAccessible: kingdomCell.IsAccessible,
                cellVisualData: kingdomCell.HasVisual ? new(
                    prefabPath: kingdomCell.VisualPrefabPath,
                    position: kingdomCell.VisualTransform.position,
                    rotation: kingdomCell.VisualTransform.rotation
                ) : null
            );
        }
    }
}