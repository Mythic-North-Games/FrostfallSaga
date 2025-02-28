using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    /// Check if the fighter is inside one of the terrains.
    /// </summary>
    [Serializable]
    public class InTerrainsCondition : AFighterCondition
    {
        [SerializeField] public TerrainTypeSO[] AvailableTerrains;

        public override bool CheckCondition(Fighter fighter, AHexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return fighter.cell != null && AvailableTerrains.Contains(fighter.cell.TerrainType);
        }

        public override string GetName()
        {
            return "In terrains";
        }

        public override string GetDescription()
        {
            return $"Check if the fighter is inside one of the terrains: {AvailableTerrains}.";
        }
    }
}