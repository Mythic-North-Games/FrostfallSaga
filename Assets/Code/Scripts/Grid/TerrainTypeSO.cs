using UnityEngine;

namespace FrostfallSaga.Grid
{
    [CreateAssetMenu(fileName = "TerrainType", menuName = "ScriptableObjects/Grid/Terrain", order = 0)]
    public class TerrainTypeSO : ScriptableObject
    {
        [field: SerializeField, Header("Info"), Tooltip("Name of the terrain's type")] public string TypeName { get; private set; }
        [field: SerializeField, Tooltip("Description of the terrain's type"), TextArea] public string TypeDescription { get; private set; }
        [field: SerializeField, Range(-10, 10), Header("Entities Stats"), Tooltip("Add or remove Health point to the entity")] public int HealthPoint { get; private set; }
        [field: SerializeField, Range(-5, 5), Tooltip("Add or remove Move point to the entity")] public int MovePoint { get; private set; }
        [field: SerializeField, Range(-5, 5), Tooltip("Add or remove Range point to the entity")] public int RangePoint { get; private set; }
        [field: SerializeField, Range(-5, 5), Tooltip("Add or remove Initiative point to the entity")] public int InitiativePoint { get; private set; }
        [field: SerializeField, Header("Cells Stats"), Tooltip("Define if the cell is accessible or not")] public bool IsAccessible { get; private set; }
        [field: SerializeField, Header("Cell visual"), Tooltip("Define the terrain's color")] public Material CellMaterial { get; private set; }
        [field: SerializeField, Tooltip("List of different prefab for visual over the cell")] public GameObject[] VisualsTerrain { get; private set; }
    }
}
