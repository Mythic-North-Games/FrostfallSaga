using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
    [CreateAssetMenu(fileName = "KingdomConfiguration", menuName = "ScriptableObjects/Kingdom/KingdomConfiguration", order = 0)]
    public class KingdomDataSO : ScriptableObject
    {
        public HexGrid hexGrid;
        public EntitiesGroup heroGroup;
        public EnemiesGroup[] enemiesGroups;
    }
}
