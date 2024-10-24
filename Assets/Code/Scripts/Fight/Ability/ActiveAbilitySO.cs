using UnityEngine;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Statuses;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Active ability fields.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilitySO : BaseAbilitySO
    {
        [field: SerializeField] public TargeterSO Targeter { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int ActionPointsCost { get; private set; }
    }
}