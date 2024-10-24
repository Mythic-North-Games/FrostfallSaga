using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "WeaknessStatus", menuName = "ScriptableObjects/Fight/Statuses/Weakness")]
    public class WeaknessStatus : AStatus
    {
        public new EStatusType StatusType { get; private set; } = EStatusType.WEAKNESS;
        [field: SerializeField] public int StrengthReduction { get; private set; }

        protected override void DoApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, StrengthReduction);
            Debug.Log($"{fighter.name}'s strength is reduced by {StrengthReduction} == > Strength : {fighter.GetStrength()}.");
        }

        protected override void DoRemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, -StrengthReduction, false);
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }
    }
}
