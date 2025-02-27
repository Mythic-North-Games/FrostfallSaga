using UnityEngine;

namespace FrostfallSaga.Core.Cities.CitySituations
{
    public abstract class ACitySituationSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Illustration { get; private set; }
    }
}