using UnityEngine;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/Fight/Effects/HealEffect", order = 0)]
    public class HealEffect : AEffect
    {
        [field: SerializeField, Range(0, 9999)] public int HealAmount { get; private set; }

        public override void ApplyEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
