using UnityEngine;

namespace FrostfallSaga.City
{
    [CreateAssetMenu(fileName = "CityConfigurationSO", menuName = "ScriptableObjects/City/CityConfigurationSO", order = 0)]
    public class CityConfigurationSO : ScriptableObject
    {
        [field: SerializeField, Header("City Configuration")] public string Name { get; private set; }
        [field: SerializeField] public Sprite CityBackground { get; private set; }
        [field: SerializeField] public TavernConfiguration TavernConfiguration { get; private set; }
    }
}