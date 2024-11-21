using System;
using UnityEngine;
using FrostfallSaga.Fight.FightCells.Impediments;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [Serializable]
    /// <summary>
    /// Alteration that adds an impediment to a cell.
    /// </summary>
    public class AddImpedimentAlteration : AFightCellAlteration
    {
        [field: SerializeField, Header("Impediment alteration definition")]
        public AImpedimentSO Impediment { get; private set; }

        private FightCell _currentlyModifiedCell;

        public AddImpedimentAlteration()
        {
            CanBeReplaced = false;
            CanApplyWithFighter = false;
        }

        public AddImpedimentAlteration(
            bool isPermanent,
            int duration,
            string name,
            string description,
            Sprite icon
        ) : base(isPermanent, duration, false, false, name, description, icon)
        {
        }

        #region Application

        public override void Apply(FightCell fightCell)
        {
            Impediment.SpawnController.onSpawnEnded += OnImpedimentGameObjectSpawned;

            _currentlyModifiedCell = fightCell;
            Impediment.SpawnController.SpawnGameObject(fightCell.transform, Impediment.Prefab);
        }

        private void OnImpedimentGameObjectSpawned(GameObject impedimentGameObject)
        {
            _currentlyModifiedCell.SetImpediment(Impediment, impedimentGameObject);
            _currentlyModifiedCell = null;
            Impediment.SpawnController.onSpawnEnded -= OnImpedimentGameObjectSpawned;
            onAlterationApplied?.Invoke(_currentlyModifiedCell, this);
        }

        #endregion

        #region Removal

        public override void Remove(FightCell fightCell)
        {
            Impediment.DestroyController.onDestroyEnded += OnImpedimentGameObjectDestroyed;

            _currentlyModifiedCell = fightCell;
            Impediment.DestroyController.DestroyGameObject(fightCell.GetImpedimentGameObject());
        }

        private void OnImpedimentGameObjectDestroyed()
        {
            _currentlyModifiedCell.SetImpediment(null, null);
            _currentlyModifiedCell = null;
            Impediment.DestroyController.onDestroyEnded -= OnImpedimentGameObjectDestroyed;
            onAlterationRemoved?.Invoke(_currentlyModifiedCell, this);
        }

        #endregion
    }
}