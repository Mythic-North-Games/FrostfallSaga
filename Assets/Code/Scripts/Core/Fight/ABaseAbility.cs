using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    /// <summary>
    ///     Abilities common fields.
    /// </summary>
    public abstract class ABaseAbility : ScriptableObject
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }
        [field: SerializeField, Range(0, 99)] public int UnlockPoints { get; protected set; }


        public abstract Dictionary<Sprite, string> GetStatsUIData();
        public abstract List<string> GetEffectsUIData();
        public abstract List<string> GetMasterstrokeEffectsUIData();
    }
}