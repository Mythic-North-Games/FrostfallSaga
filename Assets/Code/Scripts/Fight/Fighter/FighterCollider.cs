using System;
using UnityEngine;

namespace FrostfallSaga.Fight.Fighters
{
    /// <summary>
    /// Specific collider used to collides only with Fighters.
    /// Meant to be included in weapons and projectiles.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FighterCollider : MonoBehaviour
    {
        public Action<Fighter> onFighterEnter;
        public Action<Fighter> onFighterExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Fighter fighter))
            {
                onFighterEnter?.Invoke(fighter);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Fighter fighter))
            {
                onFighterExit?.Invoke(fighter);
            }
        }
    }
}