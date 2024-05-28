using UnityEngine;

namespace Code.Scripts.Targeter
{
    public class Targeter
    {
        [SerializeField] private int _originCellRange;
        [SerializeField] private Vector3[] _cellsSequence;
        [SerializeField] private bool _fighterMandatory;

        public int OriginCellRange
        {
            get => _originCellRange;
        }

        public Vector3[] CellsSequence
        {
            get => _cellsSequence;
        }

        public bool FighterMandatory
        {
            get => _fighterMandatory;
        }
    }
}