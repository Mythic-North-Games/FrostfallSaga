using System;
using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    /// <summary>
    /// Base class for all external abilities animation executors.
    /// Exposes a onFighterTouched and onAnimationEnded events that will be triggered after the call of the Execute method.
    /// </summary>
    [Serializable]
    public abstract class AExternalAbilityAnimationExecutor
    {
        public Action<Fighter> onFighterTouched;
        public Action<Fighter> onAnimationEnded;

        /// <summary>
        /// Instanciates and move the projectilePrefab as defined by the executor.
        /// The projectilePrefab should have a FighterCollider.
        /// </summary>
        /// <param name="fighterThatWillExecute">The fighter that will execute the ability animation.</param>
        /// <param name="abilityCells">The cells the ability targets.</param>
        /// <param name="projectilePrefab">The projectilePrefab to instanciate and move.</param>
        public abstract void Execute(Fighter fighterThatWillExecute, Cell[] abilityCells, GameObject projectilePrefab);

        protected void SetupProjectileColliderEventIfAny(GameObject projectile)
        {
            if (projectile.TryGetComponent<FighterCollider>(out var projectileCollider))
            {
                projectileCollider.onFighterEnter += OnFighterTouched;
            }
        }

        protected void OnFighterTouched(Fighter touchedFighter)
        {
            onFighterTouched?.Invoke(touchedFighter);
        }
    }
}