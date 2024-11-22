using System;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    /// Check if one of the fighter allies is dead.
    /// </summary>
    [Serializable]
    public class OneAllyDeadCondition : AFighterCondition
    {
        public override bool CheckCondition(Fighter fighter, HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            bool fighterTeam = fightersTeams[fighter];
            return fightersTeams.Any(f => f.Key.GetHealth() <= 0 && f.Value == fighterTeam);
        }

        public override string GetName()
        {
            return "One ally dead";
        }

        public override string GetDescription()
        {
            return "Check if one of the fighter allies is dead.";
        }
    }
}