using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "ParalysisStatus", menuName = "ScriptableObjects/Fight/Status/Paralysis")]
    public class ParalysisStatus : Status
    {
        public override void ApplyStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(true);
            Debug.Log($"{fighter.name} is paralyzed!");
        }

        public override void RemoveStatus(Fighter fighter)
        {
            fighter.SetIsParalyzed(false);
            Debug.Log($"{fighter.name} is no longer paralyzed!");
        }
    }
}
