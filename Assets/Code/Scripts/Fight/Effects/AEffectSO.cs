using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Base class for all effects that abilities will have.
    /// </summary>
    public abstract class AEffectSO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        // Apply effect involving both attacker and defender (e.g., damage with crit / dodge)
        public abstract void ApplyEffect(Fighter attacker, Fighter defender);
    }
}
