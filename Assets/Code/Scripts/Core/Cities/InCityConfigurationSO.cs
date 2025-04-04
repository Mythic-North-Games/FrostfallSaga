using FrostfallSaga.Core.Cities.CitySituations;
using UnityEngine;

namespace FrostfallSaga.Core.Cities
{
    [CreateAssetMenu(fileName = "InCityConfiguration", menuName = "ScriptableObjects/Cities/InCityConfigurationSO",
        order = 0)]
    public class InCityConfigurationSO : ScriptableObject
    {
        [field: SerializeField]
        [field: Header("Inside the city Configuration")]
        public string Name { get; private set; }

        [field: SerializeField] public Sprite CityBackground { get; private set; }
        [field: SerializeField] public Sprite ExitCityIllustration { get; private set; }
        [field: SerializeField] public TavernConfiguration TavernConfiguration { get; private set; }
        [field: SerializeField] public ACitySituationSO[] CitySituations { get; private set; }
    }
}