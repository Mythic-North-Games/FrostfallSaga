using UnityEngine;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{

    /// <summary>
    ///	List of enemiy entity prefab to make them available for spawning in a kingdom.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyEntitiesList", menuName = "ScriptableObjects/Entities/EnemyEntitiesList", order = 0)]
    public class EnemyEntitiesListSO : ScriptableObject
    {
        [field: SerializeField] public GameObject[] AvailableEnemyEntities { get; private set; }
    }
}