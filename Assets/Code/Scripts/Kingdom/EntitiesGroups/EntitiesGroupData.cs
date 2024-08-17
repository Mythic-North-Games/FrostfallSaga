using System;
using FrostfallSaga.Kingdom.Entities;

namespace FrostfallSaga.Kingdom.EntitiesGroups
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
