using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Effects;
using UnityEngine;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Items/Consumable", order = 0)]
    public class ConsumableSO : AConsumable
    {
        [SerializeReference] public List<AEffect> Effects;

        public override List<string> GetSpecialEffectsUIData()
        {
            List<string> specialEffectsUIData = new();
            foreach (AEffect effect in Effects)
            {
                specialEffectsUIData.Add(effect.GetUIEffectDescription());
            }
            return specialEffectsUIData;
        }
    }
}