using UnityEngine;

namespace FrostfallSaga.Kingdom.EntitiesGroups
{

    /// <summary>
    ///	List of entity prefabs to make them available for spawning in a kingdom.
    /// </summary>
    [CreateAssetMenu(fileName = "EntitiesList", menuName = "ScriptableObjects/Entities/EntitiesList", order = 0)]
    public class EntitiesListSO : ScriptableObject
    {
        [field: SerializeField] public GameObject[] AvailableEntities { get; private set; }
    }
}