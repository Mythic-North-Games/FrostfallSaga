using UnityEngine;

namespace FrostfallSaga.Core.Cities
{
    [CreateAssetMenu(fileName = "InCityConfiguration", menuName = "ScriptableObjects/City/InCityConfigurationSO", order = 0)]
    public class InCityConfigurationSO : ScriptableObject
    {
        [field: SerializeField, Header("City Configuration")] public string Name { get; private set; }
        [field: SerializeField] public Sprite CityBackground { get; private set; }
        [field: SerializeField] public TavernConfiguration TavernConfiguration { get; private set; }
    }
}