using UnityEngine;

namespace FrostfallSaga.Core.Cities
{
    [CreateAssetMenu(fileName = "CityBuildingConfigurationSO", menuName = "ScriptableObjects/Kingdom/CityBuildingConfigurationSO", order = 0)]
    public class CityBuildingConfigurationSO : ScriptableObject
    {
        [field: SerializeField, Header("City Configuration")] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public GameObject CityPrefab { get; private set; }
        [field: SerializeField] public Sprite CityPreview { get; private set; }
        [field: SerializeField] public InCityConfigurationSO InCityConfiguration { get; private set; }
    }
}