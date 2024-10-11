using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "BleedingStatus", menuName = "ScriptableObjects/Fight/Statuses/Bleeding")]
    public class BleedingStatus : Status
    {
        [field: SerializeField] public int BleedingReduction { get; private set; }

        public override void ApplyStatus(Fighter fighter)
        {
            fighter.ReceiveRawDamages(BleedingReduction);
            Debug.Log($"{fighter.name} is bleeding and loses ${BleedingReduction} HP! ==> Health : ${fighter.GetHealth()}");
        }

        public override void RemoveStatus(Fighter fighter)
        {
            Debug.Log($"{fighter.name} stopped bleeding.");
        }
    }
}
