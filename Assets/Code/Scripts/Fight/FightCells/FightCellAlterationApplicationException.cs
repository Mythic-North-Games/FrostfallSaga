using System;

namespace FrostfallSaga.Fight.FightCells
{
    public class FightCellAlterationApplicationException : Exception
    {
        public FightCellAlterationApplicationException()
        {
        }

        public FightCellAlterationApplicationException(string message) : base(message)
        {
        }

        public FightCellAlterationApplicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}