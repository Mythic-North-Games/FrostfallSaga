using UnityEngine;

namespace FrostfallSaga.Core.Cities
{
    [CreateAssetMenu(fileName = "CityBuildingConfigurationSO", menuName = "ScriptableObjects/Cities/CityBuildingConfigurationSO", order = 0)]
    public class CityBuildingConfigurationSO : AInterestPointConfigurationSO
    {
        [field: SerializeField] public Sprite CityPreview { get; private set; }
        [field: SerializeField] public InCityConfigurationSO InCityConfiguration { get; private set; }
    }
}