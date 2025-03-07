using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    ///     Check if one of the fighter allies is dead.
    /// </summary>
    [Serializable]
    public class OneAllyDeadCondition : AFighterCondition
    {
        public override bool CheckCondition(Fighter fighter, AHexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams)
        {
            var fighterTeam = fightersTeams[fighter];
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