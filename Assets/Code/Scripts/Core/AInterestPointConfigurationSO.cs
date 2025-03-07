using UnityEngine;

namespace FrostfallSaga.Core
{
    /// <summary>
    ///     Base configuration for all interest points on the kingdom grid.
    /// </summary>
    public abstract class AInterestPointConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public GameObject InterestPointPrefab { get; private set; }
    }
}