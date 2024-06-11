using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    public class CellFightBehavior : MonoBehaviour
    {
        [field: SerializeField] public Fighter fighter { get; set; } = null;

        public bool IsFighterOnIt(Fighter fighter)
        {
            return fighter != null;
        }
    }
}
