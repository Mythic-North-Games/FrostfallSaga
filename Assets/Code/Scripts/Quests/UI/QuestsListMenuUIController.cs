using System;
using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core.Quests;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class QuestsListMenuUIController
    {
        #region UI Elements Names & Classes
        private static readonly string FILTER_PRIMARY_QUESTS_BUTTON_UI_NAME = "PrimaryFilterButton";
        private static readonly string FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME = "SecondaryFilterButton";
        private static readonly string FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME = "MissionsFilterButton";
        private static readonly string QUEST_ITEMS_LIST_UI_NAME = "QuestItemsList";

        private static readonly string FILTER_BUTTON_ACTVE_CLASSNAME = "questsFilterButtonActive";
        private static readonly string LOCATION_LIST_QUEST_ITEM_DEFAULT_CLASSNAME = "locationListQuestItemDefault";
        private static readonly string DESCRIPTION_LIST_QUEST_ITEM_DEFAULT_CLASSNAME = "descriptionListQuestItemDefault";
        private static readonly string LIST_QUEST_ITEM_ACTIVE_CLASSNAME = "listQuestItemActive";
        #endregion

        public Action<AQuestSO> onQuestSelected;
        public Action onQuestUnselected;
        public Action onQuestsFilterChanged;

        private readonly VisualTreeAsset _locationListQuestItemTemplate;
        private readonly VisualTreeAsset _descriptionListQuestItemTemplate;
        private readonly VisualElement _root;
        private readonly Button _primaryQuestsFilterButton;
        private readonly Button _secondaryQuestsFilterButton;
        private readonly Button _missionsQuestsFilterButton;
        private readonly VisualElement _questItemsList;
        private readonly Dictionary<VisualElement, ListQuestItemUIController> _questItemsUIControllers = new();
        private readonly AQuestSO[] _quests;
        private EQuestType _currentQuestFilter;
        private VisualElement _previousSelectedQuestItemRoot;

        /// <summary>
        /// Create a new instance of the QuestsListMenuUIController.
        /// </summary>
        /// <param name="root">The root visual element of the quests list menu.</param>
        /// <param name="locationListQuestItemTemplate">The template of a quest item with location info in the list.</param>
        /// <param name="descriptionListQuestItemTemplate">The template of a quest item with description in the list.</param>
        /// <param name="quests">The quests to display in the list.</param>
        public QuestsListMenuUIController(
            VisualElement root,
            AQuestSO[] quests,
            VisualTreeAsset locationListQuestItemTemplate,
            VisualTreeAsset descriptionListQuestItemTemplate
        )
        {
            _root = root;
            _quests = quests;
            _locationListQuestItemTemplate = locationListQuestItemTemplate;
            _descriptionListQuestItemTemplate = descriptionListQuestItemTemplate;

            _questItemsList = _root.Q<VisualElement>(QUEST_ITEMS_LIST_UI_NAME);
            _primaryQuestsFilterButton = _root.Q<Button>(FILTER_PRIMARY_QUESTS_BUTTON_UI_NAME);
            _secondaryQuestsFilterButton = _root.Q<Button>(FILTER_SECONDARY_QUESTS_BUTTON_UI_NAME);
            _missionsQuestsFilterButton = _root.Q<Button>(FILTER_MISSIONS_QUESTS_BUTTON_UI_NAME);

            _primaryQuestsFilterButton.RegisterCallback<ClickEvent>(evt => OnQuestsFilterButtonClicked(evt, EQuestType.PRIMARY));
            _secondaryQuestsFilterButton.RegisterCallback<ClickEvent>(evt => OnQuestsFilterButtonClicked(evt, EQuestType.SECONDARY));
            _missionsQuestsFilterButton.RegisterCallback<ClickEvent>(evt => OnQuestsFilterButtonClicked(evt, EQuestType.MISSION));

            // Filter primary quests by default
            _currentQuestFilter = EQuestType.SECONDARY;
            FilterQuests(EQuestType.PRIMARY);
        }

        private void OnQuestsFilterButtonClicked(ClickEvent evt, EQuestType questType)
        {
            evt.StopPropagation();
            FilterQuests(questType);
        }

        private void OnQuestSelected(VisualElement questItemRoot)
        {
            // Unselect the quest if the same is clicked twice
            if (questItemRoot == _previousSelectedQuestItemRoot)
            {
                questItemRoot.RemoveFromClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);
                onQuestUnselected?.Invoke();
                _previousSelectedQuestItemRoot = null;
                return;
            }

            // Otherwise, set the selected quest item to active
            questItemRoot.AddToClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);

            // Set the previous selected quest item to default
            _previousSelectedQuestItemRoot?.RemoveFromClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);

            // Trigger the selected event to display the quest details
            onQuestSelected?.Invoke(_questItemsUIControllers[questItemRoot].ControlledQuest);

            _previousSelectedQuestItemRoot = questItemRoot;
        }

        private void OnQuestTrackingToggled(VisualElement questListItemToTrack)
        {
            AQuestSO questToTrack = _questItemsUIControllers[questListItemToTrack].ControlledQuest;

            // Update the traked state of the quest
            if (questToTrack.IsTracked)
            {
                HeroTeamQuests.Instance.UntrackQuest(questToTrack);
            }
            else
            {
                HeroTeamQuests.Instance.UpdateTrackedQuest(questToTrack);

                // Display the quest details if the quest is not already selected
                if (questListItemToTrack != _previousSelectedQuestItemRoot)
                {
                    questListItemToTrack.AddToClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);

                    // Set the previous selected quest item to default
                    _previousSelectedQuestItemRoot?.RemoveFromClassList(LIST_QUEST_ITEM_ACTIVE_CLASSNAME);

                    // Trigger the selected event to display the quest details
                    onQuestSelected?.Invoke(questToTrack);

                    _previousSelectedQuestItemRoot = questListItemToTrack;
                }
            }

            // Update the quest list items UI
            RefreshCurrentQuestsUI();
        }

        private void FilterQuests(EQuestType questType)
        {
            if (_currentQuestFilter == questType) return;
            _currentQuestFilter = questType;

            // Update the filter buttons UI
            _primaryQuestsFilterButton.EnableInClassList(FILTER_BUTTON_ACTVE_CLASSNAME, questType == EQuestType.PRIMARY);
            _secondaryQuestsFilterButton.EnableInClassList(FILTER_BUTTON_ACTVE_CLASSNAME, questType == EQuestType.SECONDARY);
            _missionsQuestsFilterButton.EnableInClassList(FILTER_BUTTON_ACTVE_CLASSNAME, questType == EQuestType.MISSION);

            // Filter the quests based on the selected quest type
            AQuestSO[] filteredQuests = _quests.Where(quest => quest.Type == questType).ToArray();
            UpdateQuestList(filteredQuests);

            onQuestsFilterChanged?.Invoke();
        }

        private void UpdateQuestList(AQuestSO[] quests)
        {
            // Clear the current quest items
            _questItemsList.Clear();
            _questItemsUIControllers.Clear();

            // Filter the quests and create the UI controllers
            foreach (AQuestSO quest in quests)
            {
                VisualElement questItemRoot;
                ListQuestItemUIController questItemUIController;

                if (_currentQuestFilter == EQuestType.PRIMARY)
                {
                    questItemRoot = _descriptionListQuestItemTemplate.Instantiate();
                    questItemRoot.AddToClassList(DESCRIPTION_LIST_QUEST_ITEM_DEFAULT_CLASSNAME);
                    questItemUIController = new DescriptionListQuestItemUIController(questItemRoot);
                }
                else
                {
                    questItemRoot = _locationListQuestItemTemplate.Instantiate();
                    questItemRoot.AddToClassList(LOCATION_LIST_QUEST_ITEM_DEFAULT_CLASSNAME);
                    questItemUIController = new LocationListQuestItemUIController(questItemRoot);
                }

                questItemUIController.SetQuest(quest);
                questItemUIController.onQuestSelected += OnQuestSelected;
                questItemUIController.onQuestTrackingToggled += OnQuestTrackingToggled;
                _questItemsUIControllers.Add(questItemRoot, questItemUIController);
                _questItemsList.Add(questItemRoot);
            }
        }

        private void RefreshCurrentQuestsUI()
        {
            _questItemsUIControllers.Values.ToList().ForEach(questListItem => questListItem.SetQuest(questListItem.ControlledQuest));
        }
    }
}