using UnityEngine;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Reperesents an active ability that can be used during a fight.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : BaseAbilitySO
    {
        [field: SerializeField] public Targeter Targeter { get; private set; }
        [field: SerializeField, Range(0, 99)] public int ActionPointsCost { get; private set; }
        [field: SerializeField, Range(0, 99)] public int GodFavorsPointsCost { get; private set; }
        [SerializeReference] public AFightCellAlteration[] CellAlterations = { };
    }
}