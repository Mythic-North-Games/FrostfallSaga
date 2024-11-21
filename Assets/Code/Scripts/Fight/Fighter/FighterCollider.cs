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
            onFighterEnter?.Invoke(other.gameObject.GetComponent<Fighter>());
        }

        private void OnTriggerExit(Collider other)
        {
            onFighterExit?.Invoke(other.gameObject.GetComponent<Fighter>());
        }
    }
}