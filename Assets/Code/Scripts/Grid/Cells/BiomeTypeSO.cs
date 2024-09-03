using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    [CreateAssetMenu(fileName = "BiomeType", menuName = "ScriptableObjects/Grid/Biome", order = 0)]
    public class BiomeTypeSO : ScriptableObject
    {
        [field: SerializeField, Header("Info"), Tooltip("Name of the biome's type")] public string TypeName { get; private set; }
        [field: SerializeField, Tooltip("Description of the biome's type"), TextArea] public string TypeDescription { get; private set; }
        [field: SerializeField, Header("Terrains"), Tooltip("Type of Terrain that can be contain in this biome")] public TerrainTypeSO[] TerrainTypeSO { get; private set; }

    }
}
