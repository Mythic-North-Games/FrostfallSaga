using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.Fight.GameItems
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Items/Consumable", order = 0)]
    public class ConsumableSO : ItemSO
    {
        [SerializeReference] public List<AEffect> Effects;

        public ConsumableSO()
        {
            SlotTag = EItemSlotTag.BAG;
        }
    }
}
