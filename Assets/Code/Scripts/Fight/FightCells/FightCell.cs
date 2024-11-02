using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightCells
{
    public class FightCell : Cell
    {
        [field: SerializeField, Header("Fight related")] public Fighter Fighter { get; private set; }

        public void SetFighter(Fighter fighter)
        {
            Fighter = fighter;
        }

        public bool HasFighter()
        {
            return Fighter != null;
        }

        public override bool IsFree()
        {
            return base.IsFree() && !HasFighter();
        }
    }
}
