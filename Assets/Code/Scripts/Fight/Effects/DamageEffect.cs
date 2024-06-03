using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies damages to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "DamageEffect", menuName = "ScriptableObjects/Fight/Effects/DamageEffect", order = 0)]
    public class DamageEffect : AEffect
    {
        [field: SerializeField, Range(0, 9999)] public int DamageAmount { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
