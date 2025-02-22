using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.Quests.UI
{
    public class QuestsListMenuUIController
    {
        #region UI Elements Names & Classes
        private static readonly string FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME = "SecondaryFilterButton";
        private static readonly string FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME = "MissionsFilterButton";
        private static readonly string QUEST_ITEMS_LIST_UI_NAME = "QuestItemsList";

        private static readonly string FILTER_BUTTON_ACTVE_CLASSNAME = "questsFilterButtonActive";
        #endregion

        public Action<AQuestSO> onQuestSelected;
        public Action onQuestsFilterChanged;

        private readonly VisualElement _root;
        private readonly VisualElement _questItemsList;
        private readonly VisualTreeAsset _listQuestItemTemplate;

        private readonly AQuestSO[] _quests;
        private readonly List<ListQuestItemUIController> _questItemsUIControllers = new();
        private EQuestType _currentQuestFilter = EQuestType.SECONDARY;

        /// <summary>
        /// Create a new instance of the QuestsListMenuUIController.
        /// </summary>
        /// <param name="root">The root visual element of the quests list menu.</param>
        /// <param name="listQuestItemTemplate">The template of a quest item in the list.</param>
        /// <param name="quests">The quests to display in the list.</param>
        public QuestsListMenuUIController(
            VisualElement root,
            VisualTreeAsset listQuestItemTemplate,
            AQuestSO[] quests
        )
        {
            _root = root;
            _quests = quests;
            _questItemsList = _root.Q<VisualElement>(QUEST_ITEMS_LIST_UI_NAME);
            _listQuestItemTemplate = listQuestItemTemplate;

            InitializeQuestFilters();
            FilterQuests(EQuestType.SECONDARY);
        }

        private void InitializeQuestFilters()
        {
            Button secondaryFilterButton = _root.Q<Button>(FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME);
            secondaryFilterButton.RegisterCallback<ClickEvent>(
                (clickEvent) => 
                {
                    clickEvent.StopPropagation();
                    if (_currentQuestFilter == EQuestType.SECONDARY)
                    {
                        return;
                    }
                    _currentQuestFilter = EQuestType.SECONDARY;
                    FilterQuests(EQuestType.SECONDARY);
                    onQuestsFilterChanged?.Invoke();
                }
            );

            Button missionsFilterButton = _root.Q<Button>(FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME);
            missionsFilterButton.RegisterCallback<ClickEvent>(
                (clickEvent) => 
                {
                    clickEvent.StopPropagation();
                    if (_currentQuestFilter == EQuestType.MISSION)
                    {
                        return;
                    }
                    _currentQuestFilter = EQuestType.MISSION;
                    FilterQuests(EQuestType.MISSION);
                    onQuestsFilterChanged?.Invoke();
                }
            );
        }

        private void FilterQuests(EQuestType questType)
        {
            // Update the filter buttons UI
            if (questType == EQuestType.SECONDARY)
            {
                _root.Q<Button>(FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME).AddToClassList(FILTER_BUTTON_ACTVE_CLASSNAME);
                _root.Q<Button>(FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME).RemoveFromClassList(FILTER_BUTTON_ACTVE_CLASSNAME);
            }
            else
            {
                _root.Q<Button>(FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME).RemoveFromClassList(FILTER_BUTTON_ACTVE_CLASSNAME);
                _root.Q<Button>(FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME).AddToClassList(FILTER_BUTTON_ACTVE_CLASSNAME);
            }

            // Clear the current quest items
            _questItemsList.Clear();
            _questItemsUIControllers.Clear();

            // Filter the quests and create the UI controllers
            AQuestSO[] filteredQuests = _quests.Where(quest => quest.Type == questType).ToArray();
            foreach (AQuestSO quest in filteredQuests)
            {
                ListQuestItemUIController questItemUIController = new(
                    _listQuestItemTemplate,
                    _questItemsList,
                    quest
                );
                questItemUIController.onQuestSelected += OnQuestSelected;
                _questItemsUIControllers.Add(questItemUIController);
            }
        }

        private void OnQuestSelected(AQuestSO quest)
        {
            // Deselect the possible other selected quests
            _questItemsUIControllers
                .Where(questItemUIController => questItemUIController.ControlledQuest != quest && questItemUIController.IsSelected)
                .ToList()
                .ForEach(questItem => questItem.Unselect());
            
            // Trigger the selected event to display the quest details
            onQuestSelected?.Invoke(quest);
        }
    }
}