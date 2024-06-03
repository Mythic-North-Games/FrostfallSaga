using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies damages to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "MagicalDamageEffect", menuName = "ScriptableObjects/Fight/Effects/MagicalDamageEffect", order = 0)]
    public class MagicalDamageEffect : AEffect
    {
        [field: SerializeField, Range(0, 9999)] public int MagicalDamageAmount { get; private set; }
        [field: SerializeField] public EMagicalElement MagicalElement { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
