using System;
using FrostfallSaga.Core.Entities;
using UnityEngine;

namespace FrostfallSaga.Core.Dungeons
{
    /// <summary>
    ///     Configuration for a dungeon fight.
    /// </summary>
    [Serializable]
    public class DungeonFightConfiguration
    {
        [field: SerializeField] public EntityConfigurationSO[] MandatoryEnemies { get; private set; }
        [field: SerializeField] public EntityConfigurationSO[] OptionalEnemies { get; private set; }
        [field: SerializeField] public int MinOptionalEnemies { get; private set; } = 1;
        [field: SerializeField] public int MaxOptionalEnemies { get; private set; } = 3;
    }
}