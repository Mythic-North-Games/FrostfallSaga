using System;
using System.Collections.Generic;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using JetBrains.Annotations;

namespace FrostfallSaga.Core.GameState.Fight
{
    [Serializable]
    public class PreFightData
    {
        public EntityConfigurationSO[] alliesEntityConf;
        public EFightOrigin fightOrigin;
        public KeyValuePair<string, EntityConfigurationSO>[] enemiesEntityConf;
        [CanBeNull] public Dictionary<HexDirection, Cell> HexDirectionCells = new ();

    }
}