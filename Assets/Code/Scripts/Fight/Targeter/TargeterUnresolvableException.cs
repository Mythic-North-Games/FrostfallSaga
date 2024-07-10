using System;

namespace FrostfallSaga.Fight.Targeters
{
    public class TargeterUnresolvableException : Exception
    {
        public TargeterUnresolvableException()
        {
        }

        public TargeterUnresolvableException(string message) : base(message)
        {
        }

        public TargeterUnresolvableException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}