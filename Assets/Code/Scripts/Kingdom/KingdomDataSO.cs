using FrostfallSaga.Kingdom.EntitiesGroups;
using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    [CreateAssetMenu(fileName = "KingdomConfiguration", menuName = "ScriptableObjects/Kingdom/KingdomConfiguration", order = 0)]
    public class KingdomDataSO : ScriptableObject
    {
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
    }
}
