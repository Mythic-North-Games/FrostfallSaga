using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Core.GameState.Kingdom;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Kingdom.InterestPoints
{
    public class InterestPointBuilder : MonoBehaviourPersistingSingleton<InterestPointBuilder>
    {
        static InterestPointBuilder()
        {
            PersistAcrossScenes = false;
        }

        /// <summary>
        /// Builds an interest point from saved data.
        /// </summary>
        public static InterestPoint BuildInterestPoint(InterestPointData interestPointData, KingdomHexGrid grid)
        {
            Vector2Int coordinates = new(interestPointData.cellX, interestPointData.cellY);
            Cell cell = grid.GetCellAtCoordinates(coordinates);
            if (cell == null) return null;

            InterestPoint interestPoint = InstantiateInterestPoint(
                interestPointData.interestPointConfiguration,
                cell.GetComponent<KingdomCell>()
            );
            return interestPoint;
        }

        /// <summary>
        /// Place the interest points on the existing grid.
        /// </summary>
        public static void FirstBuildInterestPoints(    // TODO: Refactor grid generation to include this
            KingdomHexGrid kingdomHexGrid,
            List<AInterestPointConfigurationSO> interestPoints
        )
        {
            Debug.Log("Generating Interest Points...");

            List<KingdomCell> freeCells = kingdomHexGrid.GetFreeCells();
            if (freeCells.Count == 0 && freeCells.Count > interestPoints.Count)
            {
                Debug.LogWarning("No free cells available for InterestPoints!");
                return;
            }

            foreach (AInterestPointConfigurationSO interestPointConfig in interestPoints)
            {
                KingdomCell cell = Randomizer.GetRandomElementFromArray(freeCells.ToArray());
                cell.DestroyVisual();
                InstantiateInterestPoint(interestPointConfig, cell);
                freeCells.Remove(cell);
            }
        }

        /// <summary>
        /// Instantiate an interest point in the given cell.
        /// </summary>
        private static InterestPoint InstantiateInterestPoint(AInterestPointConfigurationSO conf, KingdomCell cell)
        {
            GameObject instantiateGameObject = Instantiate(conf.InterestPointPrefab);
            InterestPoint instantiateInterestPoint = instantiateGameObject.GetComponent<InterestPoint>();
            instantiateInterestPoint.cell = cell;
            cell.SetOccupier(instantiateInterestPoint);
            Vector3 position = cell.GetCenter();
            position.y += 0.05f;
            instantiateInterestPoint.transform.SetPositionAndRotation(position, Quaternion.identity);
            instantiateInterestPoint.transform.SetParent(cell.transform);
            return instantiateInterestPoint;
        }

        /// <summary>
        /// Extract interest points data to a serializable format.
        /// </summary>
        public static InterestPointData ExtractInterestPointData(InterestPoint interestPoint)
        {
            return new InterestPointData(
                interestPoint.InterestPointConfiguration,
                interestPoint.cell.Coordinates.x,
                interestPoint.cell.Coordinates.y
            );
        }
    }
}