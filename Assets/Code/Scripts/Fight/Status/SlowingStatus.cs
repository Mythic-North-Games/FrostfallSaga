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
        }

        public override void RemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Initiative, -InitiativeReduction, false);
            Debug.Log($"{fighter.name}'s initiative is back to normal !");
        }
    }
}
