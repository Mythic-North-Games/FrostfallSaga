using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "ParalysisStatus", menuName = "ScriptableObjects/Fight/Statuses/Paralysis")]
    public class ParalysisStatus : AStatus
    {
        [field: SerializeField] public new EStatusType StatusType { get; private set; } = EStatusType.PARALYSIS;

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(true);
            Debug.Log($"{fighter.name} is paralyzed!");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(false);
            Debug.Log($"{fighter.name} is no longer paralyzed!");
        }
    }
}
