using System.Collections.Generic;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.Fight.Fighters
{
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
        public int physicalResistance;
        public Dictionary<EMagicalElement, int> magicalResistances;
        public Dictionary<EMagicalElement, int> magicalStrengths;
        public int initiative;
    }
}