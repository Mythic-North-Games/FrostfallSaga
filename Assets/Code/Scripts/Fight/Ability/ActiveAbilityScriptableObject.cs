using UnityEngine;
using FrostfallSaga.Fight.Targeters;


namespace FrostfallSaga.Fight.Abilities
{
    /// <summary>
    /// Active ability fields.
    /// </summary>
    [CreateAssetMenu(fileName = "ActiveAbility", menuName = "ScriptableObjects/Fight/Abilities/ActiveAbility", order = 0)]
    public class ActiveAbilityScriptableObject : BaseAbilityScriptableObject
    {
        [field: SerializeField] public TargeterScriptableObject Targeter { get; private set; }
        [field: SerializeField, Range(0, 9999)] public int ActionPointsCost { get; private set; }
    }
}