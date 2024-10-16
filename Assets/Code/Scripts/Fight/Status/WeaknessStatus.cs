using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "WeaknessStatus", menuName = "ScriptableObjects/Fight/Statuses/Weakness")]
    public class WeaknessStatus : Status
    {
        [field: SerializeField] public int StrengthReduction { get; private set; }

        public override void ApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, StrengthReduction);
            fighter.onStatusApplied?.Invoke(fighter, this);
            Debug.Log($"{fighter.name}'s strength is reduced by {StrengthReduction} == > Strength : ${fighter.GetStrength()}.");
        }

        public override void RemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, -StrengthReduction, false);
            fighter.onStatusRemoved?.Invoke(fighter, this);
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }
    }
}
