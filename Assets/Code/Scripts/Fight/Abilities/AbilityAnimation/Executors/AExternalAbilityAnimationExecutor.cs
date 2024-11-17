using System;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Grid.Cells;

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
        public Action<FightCell> onCellTouched;
        public Action<Fighter> onAnimationEnded;

        /// <summary>
        /// Instanciates and move the projectilePrefab as defined by the executor.
        /// The projectilePrefab should have a FighterCollider.
        /// </summary>
        /// <param name="fighterThatWillExecute">The fighter that will execute the ability animation.</param>
        /// <param name="abilityCells">The cells the ability targets.</param>
        /// <param name="projectilePrefab">The projectilePrefab to instanciate and move.</param>
        public abstract void Execute(Fighter fighterThatWillExecute, FightCell[] abilityCells, GameObject projectilePrefab);

        protected void SetupProjectileColliderEventsIfAny(GameObject projectile)
        {
            if (projectile.TryGetComponent<FighterCollider>(out var projectileCollider))
            {
                projectileCollider.onFighterEnter += OnFighterTouched;
            }
            if (projectile.TryGetComponent<CellCollider>(out var cellCollider))
            {
                cellCollider.onCellEnter += OnCellTouched;
            }
        }

        protected void OnFighterTouched(Fighter touchedFighter)
        {
            onFighterTouched?.Invoke(touchedFighter);
        }

        protected void OnCellTouched(Cell touchedCell)
        {
            onCellTouched?.Invoke((FightCell)touchedCell);
        }
    }
}