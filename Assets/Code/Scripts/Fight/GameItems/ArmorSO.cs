using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight.GameItems
{
    [CreateAssetMenu(fileName = "Armor", menuName = "ScriptableObjects/Items/Armor", order = 0)]
    public class ArmorSO : ItemSO
    {
        [field: SerializeField] public int PhysicalResistance { get; private set; }
        [field: SerializeField] public SElementToValue<EMagicalElement, int>[] MagicalResistances { get; private set; }
        [field: SerializeField] public SElementToValue<EEntityID, float>[] FightersStrengths { get; private set; }
    }
}
