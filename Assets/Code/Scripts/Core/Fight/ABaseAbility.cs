using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    /// <summary>
    /// Abilities common fields.
    /// </summary>
    public abstract class ABaseAbility : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite IconSprite { get; protected set; }
    }
}