using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;

namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Manages the passive abilities of a fighter.
    /// Checks the conditions of the passive abilities and applies them if the conditions are met.
    /// Removes the passive abilities that does not last for the fight if the conditions are not met anymore.
    /// </summary>
    public class PassiveAbilitiesManager
    {
        private readonly Fighter _fighter;
        private readonly List<PassiveAbilitySO> _activePassiveAbilities = new();

        public PassiveAbilitiesManager(Fighter fighter)
        {
            _fighter = fighter;
        }

        /// <summary>
        /// Updates the passive abilities of the fighter.
        /// Applies the passive abilities if the conditions are met.
        /// Removes the passive abilities that does not last for the fight if the conditions are not met anymore.
        /// </summary>
        /// <param name="fightGrid">The grid where the fight is happening.</param>
        /// <param name="fighterTeams">The fighters and their team.</param>
        public void UpdatePassiveAbilities(AHexGrid fightGrid, Dictionary<Fighter, bool> fighterTeams)
        {
            foreach (PassiveAbilitySO passiveAbility in _fighter.PassiveAbilities)
            {
                if (passiveAbility.CheckConditions(_fighter, fightGrid, fighterTeams))
                {
                    if (!_activePassiveAbilities.Contains(passiveAbility))
                    {
                        _activePassiveAbilities.Add(passiveAbility);
                        passiveAbility.Apply(_fighter, fighterTeams);
                    }
                    else
                    {
                        if (passiveAbility.IsRecurring)
                        {
                            passiveAbility.Apply(_fighter, fighterTeams);
                        }
                    }
                }
                else
                {
                    if (_activePassiveAbilities.Contains(passiveAbility))
                    {
                        if (!passiveAbility.LastsForFight)
                        {
                            passiveAbility.Remove(_fighter, fighterTeams);
                        }
                        _activePassiveAbilities.Remove(passiveAbility);
                    }
                }
            }
        }
    }
}