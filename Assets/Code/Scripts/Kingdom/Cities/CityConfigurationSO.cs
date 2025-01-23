using UnityEngine;

namespace FrostfallSaga.Kingdom.Cities
{
    [CreateAssetMenu(fileName = "CityConfigurationSO", menuName = "ScriptableObjects/Kingdom/CityConfigurationSO", order = 0)]
    public class CityConfigurationSO : ScriptableObject
    {
        [field: SerializeField, Header("City Configuration")] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public GameObject CityPrefab { get; private set; }
        [field: SerializeField] public Sprite CityBackground { get; private set; }
        [field: SerializeField] public TavernConfiguration TavernConfiguration { get; private set; }
    }
}