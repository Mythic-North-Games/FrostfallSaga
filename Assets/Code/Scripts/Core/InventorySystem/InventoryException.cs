using System;

namespace FrostfallSaga.Core.InventorySystem
{
    public class InventoryException : Exception
    {
        public InventoryException(string message) : base(message)
        {
        }
    }
}