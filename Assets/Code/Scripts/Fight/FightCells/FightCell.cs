using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells.Impediments;

namespace FrostfallSaga.Fight.FightCells
{
    public class FightCell : Cell
    {
        [field: SerializeField, Header("Fight related")] public Fighter Fighter { get; private set; }
        [field: SerializeField] public AImpedimentSO Impediment { get; private set; }
        private GameObject _currentImpedimentGameObject;

        public void SetFighter(Fighter fighter)
        {
            Fighter = fighter;
        }

        public void SetImpediment(AImpedimentSO impediment, GameObject impediementGameObject)
        {
            Impediment = impediment;
            _currentImpedimentGameObject = impediementGameObject;
        }

        public bool HasFighter()
        {
            return Fighter != null;
        }

        public bool HasObstacle()
        {
            return Impediment != null && Impediment is ObstacleSO;
        }

        public bool HasTrap()
        {
            return Impediment != null && Impediment is TrapSO;
        }

        public override bool IsTerrainAccessible()
        {
            return base.IsTerrainAccessible() && !HasObstacle();
        }

        public override bool IsFree()
        {
            return IsTerrainAccessible() && !HasFighter();
        }

        public GameObject GetImpedimentGameObject()
        {
            return _currentImpedimentGameObject;
        }
    }
}
