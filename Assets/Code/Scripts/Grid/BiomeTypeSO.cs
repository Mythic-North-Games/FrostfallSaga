using UnityEngine;

namespace FrostfallSaga.Grid
{
    [CreateAssetMenu(fileName = "BiomeType", menuName = "ScriptableObjects/Grid/Biome", order = 0)]
    public class BiomeTypeSO : ScriptableObject
    {
        [field: SerializeField]
        [field: Header("Info")]
        [field: Tooltip("Name of the biome's type")]
        public string TypeName { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Description of the biome's type")]
        [field: TextArea]
        public string TypeDescription { get; private set; }

        [field: SerializeField]
        [field: Header("Terrains")]
        [field: Tooltip("Type of Terrain that can be contain in this biome")]
        public TerrainTypeSO[] TerrainTypeSO { get; private set; }
    }
}