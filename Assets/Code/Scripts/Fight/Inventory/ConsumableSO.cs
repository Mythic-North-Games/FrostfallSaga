using System.Collections.Generic;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.InventorySystem;
using UnityEngine;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Items/Consumable", order = 0)]
    public class ConsumableSO : AConsumable
    {
        [SerializeReference] public List<AEffect> Effects;
    }
}