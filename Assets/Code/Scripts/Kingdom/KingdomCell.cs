using UnityEngine;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// A cell that can contain a KingdomCellOccupier.
    /// </summary>
    public class KingdomCell : Cell
    {
        [field: SerializeField, Header("Kingdom related")] public KingdomCellOccupier Occupier { get; private set; }

        public void SetOccupier(KingdomCellOccupier newOccupier)
        {
            Occupier = newOccupier;
        }

        public bool HasOccupier()
        {
            return Occupier != null;
        }

        public override bool IsFree()
        {
            return IsTerrainAccessible() && !HasOccupier();
        }
    }
}
