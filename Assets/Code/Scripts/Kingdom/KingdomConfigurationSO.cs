using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Kingdom.EntitiesGroups;

namespace FrostfallSaga.Kingdom
{
    [CreateAssetMenu(fileName = "KingdomConfiguration", menuName = "ScriptableObjects/Kingdom/KingdomConfiguration", order = 0)]
    public class KingdomConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public HexGrid HexGrid { get; set; }
        [field: SerializeField] public EntitiesGroup HeroGroup { get; set; }
        [field: SerializeField] public EnemiesGroup[] EnemiesGroups { get; set; }
    }
}
