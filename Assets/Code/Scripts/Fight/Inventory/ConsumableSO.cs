using System.Collections.Generic;
using FrostfallSaga.Core.InventorySystem;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.FightItems
{
    [CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/Items/Consumable", order = 0)]
    public class ConsumableSO : AConsumable
    {
        [SerializeReference] public List<AEffect> Effects;

        public void Use(Fighter receiver)
        {
            foreach (AEffect effect in Effects)
            {
                effect.ApplyEffect(receiver, false);
            }
        }

        public override List<string> GetEffectsUIData()
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