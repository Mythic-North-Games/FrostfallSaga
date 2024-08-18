using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight
{
    [Serializable]
    public class PostFightFighterState
    {
        public int lastingHealth;

        public PostFightFighterState(Fighter fighter)
        {
            lastingHealth = fighter.GetHealth();
        }
    }
}