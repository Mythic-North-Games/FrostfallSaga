using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Base class for all effects that abilities will have.
    /// </summary>
    public abstract class AEffectSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        public abstract void ApplyEffect(Fighter fighter);
    }
}
