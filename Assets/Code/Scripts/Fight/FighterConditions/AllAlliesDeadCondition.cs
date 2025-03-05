using System;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    /// Check if all of the fighter allies are dead.
    /// </summary>
    [Serializable]
    public class AllAlliesDeadCondition : AFighterCondition
    {
        public override bool CheckCondition(Fighter fighter, AHexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            bool fighterTeam = fightersTeams[fighter];
            return fightersTeams.All(f => f.Key.GetHealth() <= 0 || f.Value != fighterTeam);
        }

        public override string GetName()
        {
            return "All allies are dead";
        }

        public override string GetDescription()
        {
            return "Check if all the fighter allies are dead.";
        }
    }
}