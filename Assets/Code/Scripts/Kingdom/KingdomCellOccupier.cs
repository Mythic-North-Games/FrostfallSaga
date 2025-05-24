using UnityEngine;

namespace FrostfallSaga.Kingdom
{
    /// <summary>
    /// Base class for objects that can occupy a KingdomCell.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class KingdomCellOccupier : MonoBehaviour
    {
        public KingdomCell cell;
    }
}