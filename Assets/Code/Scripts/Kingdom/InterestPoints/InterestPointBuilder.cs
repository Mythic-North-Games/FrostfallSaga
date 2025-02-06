using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using FrostfallSaga.Core.GameState.Kingdom;

namespace FrostfallSaga.Kingdom.InterestPoints
{
    public class InterestPointBuilder : MonoBehaviourPersistingSingleton<InterestPointBuilder>
    {
        public InterestPoint BuildInterestPoint(InterestPointData interestPointData, HexGrid grid)
        {
            GameObject interestPointPrefab = WorldGameObjectInstantiator.Instance.Instantiate(
                interestPointData.interestPointConfiguration.InterestPointPrefab
            );
            InterestPoint interestPoint = interestPointPrefab.GetComponent<InterestPoint>();
            interestPoint.cell = grid.CellsByCoordinates[new(interestPointData.cellX, interestPointData.cellY)] as KingdomCell;
            interestPoint.transform.position = interestPoint.cell.GetCenter();
            return interestPoint;
        }

        public InterestPointData ExtractInterestPointDataFromInterestPoint(InterestPoint interestPoint)
        {
            return new(
                interestPoint.InterestPointConfiguration,
                interestPoint.cell.Coordinates.x,
                interestPoint.cell.Coordinates.y
            );
        }

        static InterestPointBuilder()
        {
            PersistAcrossScenes = false;
        }
    }
}