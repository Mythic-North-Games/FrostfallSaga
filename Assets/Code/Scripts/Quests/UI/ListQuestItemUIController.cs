using UnityEngine.UIElements;
using FrostfallSaga.Core.Quests;
using System;

namespace FrostfallSaga.Quests.UI
{
    public class ListQuestItemUIController
    {
        #region UI Elements Names & Classes
        private static readonly string QUEST_ITEM_CONTAINER_ROOT_UI_NAME = "ListQuestItemContainer";
        private static readonly string QUEST_TYPE_ICON_UI_NAME = "QuestTypeIcon";
        private static readonly string QUEST_TITLE_UI_NAME = "Title";
        private static readonly string QUEST_ORIGIN_LOCATION_UI_NAME = "OriginLocation";
        private static readonly string QUEST_COMPLETED_ICON_UI_NAME = "QuestCompletedIcon";

        private static readonly string QUEST_ITEM_CONTAINER_COMPLETED_CLASSNAME = "listQuestItemContainerCompleted";
        private static readonly string LIST_QUEST_ITEM_DEFAULT_CLASSNAME = "listQuestItemDefault";
        private static readonly string LIST_QUEST_ITEM_ACTIVE_CLASSNAME = "listQuestItemActive";
        private static readonly string QUEST_TYPE_ICON_MISSION_CLASSNAME = "questTypeIconMission";
        private static readonly string QUEST_TYPE_ICON_SECONDARY_CLASSNAME = "questTypeIconSecondary";
        private static readonly string QUEST_TYPE_ICON_HISTORY_CLASSNAME = "questTypeIconHistory";
        #endregion

        public Action<AQuestSO> onQuestSelected;
        public AQuestSO ControlledQuest { get; private set; }
        public bool IsSelected => _listQuestItemRoot.ClassListContains(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);

        private VisualElement _listQuestItemRoot;

        /// <summary>
        /// Add a new quest item to the given container then prepare the control of the quest item.
        /// </summary>
        /// <param name="listQuestItemTemplate">The template of a quest item in the list.</param>
        /// <param name="listQuestItemContainer">The quest item container</param>
        /// <param name="quest">The quest the controller will control.</param>
        public ListQuestItemUIController(
            VisualTreeAsset listQuestItemTemplate,
            VisualElement listQuestItemContainer,
            AQuestSO quest
        )
        {
            ControlledQuest = quest;

            _listQuestItemRoot = BuildListQuestItem(listQuestItemTemplate, quest);
            _listQuestItemRoot.AddToClassList(LIST_QUEST_ITEM_DEFAULT_CLASSNAME);
            _listQuestItemRoot.RegisterCallback<ClickEvent>(OnQuestClicked);
            listQuestItemContainer.Add(_listQuestItemRoot);
        }

        /// <summary>
        /// Unselect the quest item.
        /// </summary>
        public void Unselect()
        {
            _listQuestItemRoot.RemoveFromClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);
        }

        private void OnQuestClicked(ClickEvent clickEvent)
        {
            clickEvent.StopPropagation();
            _listQuestItemRoot.AddToClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);
            onQuestSelected?.Invoke(ControlledQuest);
        }

        private VisualElement BuildListQuestItem(
            VisualTreeAsset listQuestItemTemplate,
            AQuestSO quest
        )
        {
            VisualElement listQuestItemRoot = listQuestItemTemplate.Instantiate();

            listQuestItemRoot.Q<VisualElement>(QUEST_ITEM_CONTAINER_ROOT_UI_NAME).AddToClassList(
                quest.IsCompleted ? QUEST_ITEM_CONTAINER_COMPLETED_CLASSNAME : string.Empty
            );
            listQuestItemRoot.Q<VisualElement>(QUEST_TYPE_ICON_UI_NAME).ClearClassList();
            listQuestItemRoot.Q<VisualElement>(QUEST_TYPE_ICON_UI_NAME).AddToClassList(
                quest.Type switch
                {
                    EQuestType.MISSION => QUEST_TYPE_ICON_MISSION_CLASSNAME,
                    EQuestType.SECONDARY => QUEST_TYPE_ICON_SECONDARY_CLASSNAME,
                    EQuestType.MAIN => QUEST_TYPE_ICON_HISTORY_CLASSNAME,
                    _ => string.Empty
                }
            );
            listQuestItemRoot.Q<Label>(QUEST_TITLE_UI_NAME).text = quest.Name;
            listQuestItemRoot.Q<Label>(QUEST_ORIGIN_LOCATION_UI_NAME).text = quest.OriginLocation;
            listQuestItemRoot.Q<VisualElement>(QUEST_COMPLETED_ICON_UI_NAME).visible = quest.IsCompleted;

            return listQuestItemRoot;
        }
    }
}