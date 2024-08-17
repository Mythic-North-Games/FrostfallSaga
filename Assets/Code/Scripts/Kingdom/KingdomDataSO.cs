using UnityEngine;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
    [CreateAssetMenu(fileName = "KingdomConfiguration", menuName = "ScriptableObjects/Kingdom/KingdomConfiguration", order = 0)]
    public class KingdomDataSO : ScriptableObject
    {
        //public HexGrid hexGrid;
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
    }
}
