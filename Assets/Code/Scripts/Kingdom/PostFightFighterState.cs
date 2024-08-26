using System;

namespace FrostfallSaga.Kingdom
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