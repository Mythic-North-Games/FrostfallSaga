using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    ///     Base class for all fighter conditions.
    ///     A fighter condition implements a condition that must be met for a fighter to be able to perform an action that
    ///     depends on it.
    /// </summary>
    [Serializable]
    public abstract class AFighterCondition
    {
        public abstract bool CheckCondition(Fighter fighter, FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams);

        public abstract string GetName();
        public abstract string GetDescription();
    }
}