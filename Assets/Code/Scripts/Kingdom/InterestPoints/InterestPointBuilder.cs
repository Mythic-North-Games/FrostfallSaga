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
        ///     Construit un InterestPoint à partir des données sauvegardées
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
        ///     Génère les InterestPoints initiaux
        /// </summary>
        public static void FirstBuildInterestPoints(KingdomHexGrid kingdomHexGrid,
            List<AInterestPointConfigurationSO> interestPoints)
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
                InstantiateInterestPoint(interestPointConfig, cell);
                freeCells.Remove(cell);
            }
        }

        /// <summary>
        ///     Crée une instance d'un InterestPoint sur une cellule
        /// </summary>
        private static InterestPoint InstantiateInterestPoint(AInterestPointConfigurationSO conf, KingdomCell cell)
        {
            GameObject instantiateGameObject = Instantiate(conf.InterestPointPrefab);
            InterestPoint instantiateInterestPoint = instantiateGameObject.GetComponent<InterestPoint>();
            instantiateInterestPoint.cell = cell;
            cell.SetOccupier(instantiateInterestPoint);
            Vector3 position = cell.GetCenter();
            position.y += 0.5f;
            instantiateInterestPoint.transform.SetPositionAndRotation(position, Quaternion.identity);
            instantiateInterestPoint.transform.SetParent(cell.transform);
            return instantiateInterestPoint;
        }

        /// <summary>
        ///     Extrait les données d'un InterestPoint pour sauvegarde
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