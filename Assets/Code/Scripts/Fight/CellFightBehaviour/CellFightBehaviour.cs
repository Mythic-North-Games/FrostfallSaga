using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class CellFightBehaviour : MonoBehaviour
    {
        public Fighter Fighter = null;

        public bool IsFighterOnIt()
        {
            return Fighter != null;
        }
    }
}
