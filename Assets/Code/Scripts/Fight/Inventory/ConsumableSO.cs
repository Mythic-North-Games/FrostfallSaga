using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Items/Consumable", order = 0)]
    public class ConsumableSO : AConsumable
    {
        [SerializeReference] public List<AEffect> Effects;
    }
}
