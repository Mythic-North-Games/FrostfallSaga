using UnityEngine;

namespace Code.Scripts.Targeter
{
    public class Targeter
    {
        [SerializeField] public int OriginCellRange { get; private set; }
        [SerializeField] public Vector3[] CellsSequence { get; private set; }
        [SerializeField] public bool FighterMandatory { get; private set; }


    }
}