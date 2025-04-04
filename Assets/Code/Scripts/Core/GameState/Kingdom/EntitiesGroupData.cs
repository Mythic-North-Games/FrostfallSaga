using System;

namespace FrostfallSaga.Core.GameState.Kingdom
{
    [Serializable]
    public class EntitiesGroupData
    {
        public int movePoints;
        public int cellX;
        public int cellY;
        public EntityData[] entitiesData;
        public string displayedEntitySessionId;
    }
}