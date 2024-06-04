using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies damages to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "PhysicalDamageEffect", menuName = "ScriptableObjects/Fight/Effects/PhysicalDamageEffect", order = 0)]
    public class PhysicalDamageEffectScriptableObject : AEffectScriptableObject
    {
        [field: SerializeField, Range(0, 9999)] public int PhysicalDamageAmount { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
