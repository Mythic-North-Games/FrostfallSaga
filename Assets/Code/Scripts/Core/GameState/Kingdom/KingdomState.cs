using System;
using FrostfallSaga.Core.GameState.Grid;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class KingdomState
    {
        public bool kingdomGridGenerated;
        public KingdomCellData[] kingdomCellsData;
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
        public InterestPointData[] interestPointsData;
    }
}