using System;

namespace FrostfallSaga.Kingdom.EnemiesGroupsSpawner
{
    public class ImpossibleSpawnException : Exception
    {
        public ImpossibleSpawnException()
        {
        }

        public ImpossibleSpawnException(string message) : base(message)
        {
        }

        public ImpossibleSpawnException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}