using FrostfallSaga.Core.InventorySystem;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI.FightEndMenu
{
    /// <summary>
    ///     Responsible for controlling the fight end menu.
    /// </summary>
    public static class ItemRewardContainerUIController
    {
        #region UXML Names and classes
        private static readonly string ITEM_ICON_UI_NAME = "ItemIcon";
        private static readonly string ITEM_COUNT_LABEL_UI_NAME = "ItemCount";
        #endregion

        public static void Setup(VisualElement root, ItemSO item, int count)
        {
            root.Q<VisualElement>(ITEM_ICON_UI_NAME).style.backgroundImage = new(item.IconSprite);
            root.Q<Label>(ITEM_COUNT_LABEL_UI_NAME).text = count.ToString();
        }
    }
}