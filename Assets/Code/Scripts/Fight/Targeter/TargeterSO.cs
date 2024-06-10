using UnityEngine;

namespace FrostfallSaga.Fight.Targeters
{
    /// <summary>
    /// Defines a targeter. Will be used to select cells for abilities and weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeter", menuName = "ScriptableObjects/Fight/Targeter", order = 0)]
    public class TargeterSO : ScriptableObject
    {
        [field: SerializeField] public int OriginCellRange { get; private set; }
        [field: SerializeField] public Vector3[] CellsSequence { get; private set; }
        [field: SerializeField] public bool FighterMandatory { get; private set; }
    }
}