using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using UnityEngine;

namespace FrostfallSaga.Core.InventorySystem
{
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Items/Item", order = 0)]
    public class ItemSO : ScriptableObject, IUIObjectDescribable
    {
        private static readonly string ITEMS_RESOURCES_PATH = "ScriptableObjects/Items/";

        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite IconSprite { get; protected set; }
        [field: SerializeField] public EItemSlotTag SlotTag { get; protected set; }
        [field: SerializeField] public float LootChance { get; protected set; }

        public bool IsEquippable()
        {
            return this is AConsumable || this is AArmor || this is AWeapon;
        }

        #region IUIObjectDescribable interface implementation
        
        public Sprite GetIcon()
        {
            return IconSprite;
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Description;
        }

        public virtual Dictionary<Sprite, string> GetStatsUIData()
        {
            return new();
        }

        public virtual string GetPrimaryEffectsTitle()
        {
            return null;
        }

        public virtual List<string> GetPrimaryEffectsUIData()
        {
            return new();
        }

        public virtual string GetSecondaryEffectsTitle()
        {
            return null;
        }

        public virtual List<string> GetSecondaryEffectsUIData()
        {
            return new();
        }

        #endregion

        public static ItemSO LoadFromResources(string itemResourceFilename)
        {
            return Resources.Load<ItemSO>($"{ITEMS_RESOURCES_PATH}{itemResourceFilename}");
        }
    }
}