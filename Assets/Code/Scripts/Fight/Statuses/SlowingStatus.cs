using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "SlowingStatus", menuName = "ScriptableObjects/Fight/Statuses/Slowing")]
    public class SlowingStatus : Status
    {
        [field: SerializeField] public int InitiativeReduction { get; private set; }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, InitiativeReduction);
            Debug.Log($"{fighter.name}'s initiative is reduced by {InitiativeReduction}.");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, -InitiativeReduction, false);
            Debug.Log($"{fighter.name}'s initiative is back to normal !");
        }
    }
}
