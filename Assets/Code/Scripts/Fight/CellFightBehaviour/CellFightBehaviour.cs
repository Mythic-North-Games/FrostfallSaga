using UnityEngine;
using FrostfallSaga.Fight.Fighters;

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
