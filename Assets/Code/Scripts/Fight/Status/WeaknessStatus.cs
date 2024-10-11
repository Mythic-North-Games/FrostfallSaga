using UnityEngine;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Statuses
{
    [CreateAssetMenu(fileName = "WeaknessStatus", menuName = "ScriptableObjects/Fight/Status/Weakness")]
    public class WeaknessStatus : Status
    {
        [field: SerializeField] public int StrengthReduction { get; private set; }

        public override void ApplyStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, StrengthReduction);
        }

        public override void RemoveStatus(Fighter fighter)
        {
            fighter.UpdateMutableStat(EFighterMutableStat.Strength, -StrengthReduction, false);
            Debug.Log($"{fighter.name}'s strength is back to normal !");
        }
    }
}
