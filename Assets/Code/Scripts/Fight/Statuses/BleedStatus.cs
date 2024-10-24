using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "BleedStatus", menuName = "ScriptableObjects/Fight/Statuses/Bleed")]
    public class BleedStatus : AStatus
    {
        [field: SerializeField] public int BleedingDamage { get; private set; }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.ReceiveRawDamages(BleedingDamage);
            Debug.Log($"{fighter.name} is bleeding and loses ${BleedingDamage} HP! ==> Health : ${fighter.GetHealth()}");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            Debug.Log($"{fighter.name} stopped bleeding.");
        }
    }
}
