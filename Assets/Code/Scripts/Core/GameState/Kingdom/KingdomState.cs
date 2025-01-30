using System;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class KingdomState
    {
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
    }
}
