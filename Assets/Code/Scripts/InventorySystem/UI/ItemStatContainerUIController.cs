using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class ItemStatContainerUIController
    {
        #region UI Elements Names & Classes
        private static readonly string STAT_ICON_UI_NAME = "StatIcon";
        private static readonly string STAT_VALUE_LABEL_UI_NAME = "StatValueLabel";

        private static readonly string STAT_CONTAINER_ROOT_DEFAULT_CLASSNAME = "statContainerRootDefault";
        #endregion

        public ItemStatContainerUIController(
            VisualElement parent,
            VisualTreeAsset statContainerTemplate,
            Sprite statIcon,
            string statValue
        )
        {
            VisualElement statContainerRoot = statContainerTemplate.Instantiate();
            statContainerRoot.AddToClassList(STAT_CONTAINER_ROOT_DEFAULT_CLASSNAME);

            statContainerRoot.Q<VisualElement>(STAT_ICON_UI_NAME).style.backgroundImage = new(statIcon);
            statContainerRoot.Q<Label>(STAT_VALUE_LABEL_UI_NAME).text = statValue;

            parent.Add(statContainerRoot);
        }
    }
}