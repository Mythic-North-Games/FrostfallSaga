using System;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Fight.Effects;

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
        public float tenacity;
        public float dodge;
        public int physicalResistance;
        public Dictionary<EMagicalElement, int> magicalResistances;
        public Dictionary<EMagicalElement, int> magicalStrengths;
        public float masterstroke;
        public int initiative;

        public void AddMagicalResistances(MagicalElementToValue[] additionnalMagicalResistances) {
            Dictionary<EMagicalElement, int> result = new Dictionary<EMagicalElement, int>();

            Dictionary<EMagicalElement, int> additionnalMagicalResistancesDictionnary = MagicalElementToValue.GetDictionaryFromArray(additionnalMagicalResistances);
            foreach (var additionnalMagicalResistance in additionnalMagicalResistancesDictionnary) {
                EMagicalElement magicalElement = additionnalMagicalResistance.Key;

                if (magicalResistances.ContainsKey(magicalElement)) {
                    int newResistance = magicalResistances[magicalElement] + additionnalMagicalResistance.Value;
                    result.Add(magicalElement, newResistance);
                }
            }
             magicalResistances = result;  
        }

        public void AddMagicalStrengths(MagicalElementToValue[] additionnalMagicalStrengths) {
            Dictionary<EMagicalElement, int> result = new Dictionary<EMagicalElement, int>();

            Dictionary<EMagicalElement, int> additionnalMagicalStrengthsDictionnary = MagicalElementToValue.GetDictionaryFromArray(additionnalMagicalStrengths);

            foreach (var additionnalStrength in additionnalMagicalStrengthsDictionnary) {
                EMagicalElement magicalElement = additionnalStrength.Key;

                if (magicalStrengths.ContainsKey(magicalElement)) {
                    int newStrength = magicalStrengths[magicalElement] + additionnalStrength.Value;
                    result.Add(magicalElement, newStrength);
                }
            }
            magicalStrengths = result;
        }
    }
}