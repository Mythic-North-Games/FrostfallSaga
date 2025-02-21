using FrostfallSaga.Core;
using UnityEngine;

namespace FrostfallSaga.Kingdom.InterestPoints
{
    /// <summary>
    /// Represents an interest point placed on a cell in the kingdom grid.
    /// </summary>
    public class InterestPoint : KingdomCellOccupier
    {
        [field: SerializeField] public GameObject NamePanelAnchor { get; private set; }
        [field: SerializeField] public AInterestPointConfigurationSO InterestPointConfiguration { get; private set; }

        private void Start()
        {
            if (cell != null)
            {
                transform.position = cell.GetCenter();
                if (!cell.HasOccupier()) cell.SetOccupier(this);
            }
        }
    }
}