using UnityEngine;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Active ability fields.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : BaseAbilitySO
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int ActionPointsCost { get; private set; }
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
    }
}