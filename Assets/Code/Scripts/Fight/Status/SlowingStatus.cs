using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "SlowingStatus", menuName = "ScriptableObjects/Fight/Statuses/Slowing")]
    public class SlowingStatus : Status
    {
        [field: SerializeField] public int InitiativeReduction { get; private set; }

        public override void ApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, InitiativeReduction);
            fighter.onStatusApplied?.Invoke(fighter, this);
            Debug.Log($"{fighter.name}'s initiative is reduced by {InitiativeReduction}.");
        }

        public override void RemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, -InitiativeReduction, false);
            fighter.onStatusRemoved?.Invoke(fighter, this);
            Debug.Log($"{fighter.name}'s initiative is back to normal !");
        }
    }
}
