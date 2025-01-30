using System;

namespace FrostfallSaga.Core.GameState.Fight
{
    [Serializable]
    public class PostFightFighterState
    {
        public int lastingHealth;

        public PostFightFighterState(int lastingHealth)
        {
            this.lastingHealth = lastingHealth;
        }
    }
}