using System;

namespace FrostfallSaga.InventorySystem
{
    public class InventoryException : Exception
    {
        public InventoryException(string message) : base(message)
        {
        }
    }
}