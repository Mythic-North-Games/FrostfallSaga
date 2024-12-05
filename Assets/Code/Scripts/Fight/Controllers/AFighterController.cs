using System;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.Controllers
{
    public abstract class AFighterController : MonoBehaviour
    {
        public Action<Fighter> onFighterActionStarted;
        public Action<Fighter> onFighterActionEnded;
        public Action<Fighter> onFighterTurnEnded;

        /// <summary>
        /// Make the given fighter play its turn and send an event when it's done.
        /// </summary>
        public abstract void PlayTurn(Fighter fighterToPlay);
    }
}