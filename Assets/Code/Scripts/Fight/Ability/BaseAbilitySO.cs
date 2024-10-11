using UnityEngine;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Statuses;


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
        [field: SerializeField] public AEffectSO[] Effects { get; private set; } = {};
        [field: SerializeField] public Status[] Statuses { get; private set; } = {};
    }
}