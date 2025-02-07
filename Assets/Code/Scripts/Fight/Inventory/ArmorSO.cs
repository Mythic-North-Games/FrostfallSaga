using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Items/Armor", order = 0)]
    public class ArmorSO : AArmor
    {
        [field: SerializeField] public int PhysicalResistance { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EEntityRace, float>[] FightersStrengths { get; private set; }
    }
}
