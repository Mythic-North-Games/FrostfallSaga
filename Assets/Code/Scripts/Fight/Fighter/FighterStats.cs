using System;
using System.Collections.Generic;

namespace FrostfallSaga.Fight.Fighters
{
    [Serializable]
    public class FighterStats
    {
        public int health;
        public int maxHealth;
        public int actionPoints;
        public int maxActionPoints;
        public int movePoints;
        public int maxMovePoints;
        public int strength;
        public int dexterity;
        public int tenacity;
        public int physicalResistance;
        public Dictionary<EMagicalElement, int> magicalResistances = new();
        public Dictionary<EMagicalElement, int> magicalStrengths = new();
        public float dodgeChance;
        public float masterstrokeChance;
        public int initiative;

        public void AddMagicalResistances(Dictionary<EMagicalElement, int> magicalResistancesToAdd) {
            foreach (KeyValuePair<EMagicalElement, int> additionnalMagicalResistance in magicalResistancesToAdd) {
                EMagicalElement magicalElement = additionnalMagicalResistance.Key;
                if (magicalResistances.ContainsKey(magicalElement)) {
                    magicalResistances[magicalElement] += additionnalMagicalResistance.Value;
                }
            }
        }

        public void AddMagicalStrengths(Dictionary<EMagicalElement, int> magicalStrengthsToAdd) {
            foreach (KeyValuePair<EMagicalElement, int> additionnalMagicalStrength in magicalStrengthsToAdd) {
                EMagicalElement magicalElement = additionnalMagicalStrength.Key;
                if (magicalStrengths.ContainsKey(magicalElement)) {
                    magicalStrengths[magicalElement] += additionnalMagicalStrength.Value;
                }
            }
        }
    }
}