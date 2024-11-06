using UnityEngine;
using FrostfallSaga.Fight.FightCells.Impediments;

namespace FrostfallSaga.Fight.FightCells.FightCellAlterations
{
    [CreateAssetMenu(
        fileName = "AddImpedimentAlteration",
        menuName = "ScriptableObjects/Fight/CellAlterations/AddImpedimentAlteration",
        order = 0
    )]
    /// <summary>
    /// Alteration that adds an impediment to a cell.
    /// </summary>
    public class AddImpedimentAlterationSO : AFightCellAlterationSO
    {
        [field: SerializeField, Header("Impediment alteration definition")]
        public AImpedimentSO Impediment { get; private set; }

        private FightCell _currentlyModifiedCell;

        #region Application

        public override void Apply(FightCell fightCell)
        {
            _currentlyModifiedCell = fightCell;
            Impediment.SpawnController.SpawnGameObject(fightCell.transform, Impediment.Prefab);
        }

        private void OnImpedimentGameObjectSpawned(GameObject impedimentGameObject)
        {
            _currentlyModifiedCell.SetImpediment(Impediment, impedimentGameObject);
            onAlterationApplied?.Invoke(_currentlyModifiedCell);
            _currentlyModifiedCell = null;
        }

        #endregion

        #region Destruction

        public override void Remove(FightCell fightCell)
        {
            _currentlyModifiedCell = fightCell;
            Impediment.DestroyController.DestroyGameObject(fightCell.GetImpedimentGameObject());
        }

        private void OnImpedimentGameObjectDestroyed()
        {
            _currentlyModifiedCell.SetImpediment(null, null);
            onAlterationRemoved?.Invoke(_currentlyModifiedCell);
            _currentlyModifiedCell = null;
        }

        #endregion

        #region Setup & teardown

        private void Awake()
        {
            if (Impediment == null)
            {
                Debug.LogWarning($"{name} has no impediment configured.");
                return;
            }

            if (Impediment.SpawnController == null)
            {
                Debug.LogWarning($"Impediment {Impediment.name} has no spawn controller.");
            }
            else
            {
                Impediment.SpawnController.onSpawnEnded += OnImpedimentGameObjectSpawned;
            }

            if (Impediment.DestroyController == null)
            {
                Debug.LogWarning($"Impediment {Impediment.name} has no destroy controller.");
            }
            else
            {
                Impediment.DestroyController.onDestroyEnded += OnImpedimentGameObjectDestroyed;
            }
        }

        private void OnDisable()
        {
            if (Impediment == null)
            {
                return;
            }

            if (Impediment.SpawnController != null)
            {
                Impediment.SpawnController.onSpawnEnded -= OnImpedimentGameObjectSpawned;
            }

            if (Impediment.DestroyController != null)
            {
                Impediment.DestroyController.onDestroyEnded -= OnImpedimentGameObjectDestroyed;
            }
        }

        #endregion
    }
}