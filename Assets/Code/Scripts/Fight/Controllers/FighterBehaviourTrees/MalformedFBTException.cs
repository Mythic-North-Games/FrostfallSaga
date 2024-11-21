using System;

namespace FrostfallSaga.Fight.Controllers.FighterBehaviourTrees
{
    public class MalformedFBTException : Exception
    {
        public MalformedFBTException()
        {
        }

        public MalformedFBTException(string message) : base(message)
        {
        }

        public MalformedFBTException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}