using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    /// Check if the fighter is dead.
    /// </summary>
    [Serializable]
    public class IsDeadCondition : AFighterCondition
    {
        public override bool CheckCondition(Fighter fighter, FightHexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return fighter.GetHealth() <= 0;
        }

        public override string GetName()
        {
            return "Is dead";
        }

        public override string GetDescription()
        {
            return "Check if the fighter is dead.";
        }
    }
}