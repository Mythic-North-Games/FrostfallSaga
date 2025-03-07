using UnityEngine;

namespace FrostfallSaga.Grid
{
    [CreateAssetMenu(fileName = "TerrainType", menuName = "ScriptableObjects/Grid/Terrain", order = 0)]
    public class TerrainTypeSO : ScriptableObject
    {
        [field: SerializeField]
        [field: Header("Info")]
        [field: Tooltip("Name of the terrain's type")]
        public string TypeName { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Description of the terrain's type")]
        [field: TextArea]
        public string TypeDescription { get; private set; }

        [field: SerializeField]
        [field: Range(-10, 10)]
        [field: Header("Entities Stats")]
        [field: Tooltip("Add or remove Health point to the entity")]
        public int HealthPoint { get; private set; }

        [field: SerializeField]
        [field: Range(-5, 5)]
        [field: Tooltip("Add or remove Move point to the entity")]
        public int MovePoint { get; private set; }

        [field: SerializeField]
        [field: Range(-5, 5)]
        [field: Tooltip("Add or remove Range point to the entity")]
        public int RangePoint { get; private set; }

        [field: SerializeField]
        [field: Range(-5, 5)]
        [field: Tooltip("Add or remove Initiative point to the entity")]
        public int InitiativePoint { get; private set; }

        [field: SerializeField]
        [field: Header("Cells Stats")]
        [field: Tooltip("Define if the cell is accessible or not")]
        public bool IsAccessible { get; private set; }

        [field: SerializeField]
        [field: Header("Cell visual")]
        [field: Tooltip("Define the terrain's color")]
        public Material CellMaterial { get; private set; }

        [field: SerializeField]
        [field: Tooltip("List of different prefab for visual over the cell")]
        public GameObject[] VisualsTerrain { get; private set; }
    }
}