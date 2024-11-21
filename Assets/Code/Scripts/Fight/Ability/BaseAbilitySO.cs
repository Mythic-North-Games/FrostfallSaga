using FrostfallSaga.Fight.Effects;
using UnityEngine;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Abilities common fields.
    /// </summary>
    public abstract class BaseAbilitySO : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite IconSprite { get; private set; }
        [field: SerializeField] public AEffectSO[] Effects { get; private set; }
    }
}