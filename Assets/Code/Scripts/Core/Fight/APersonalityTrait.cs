using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    public abstract class APersonalityTrait : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
    }
}