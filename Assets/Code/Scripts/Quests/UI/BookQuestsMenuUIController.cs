using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Quests;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class BookQuestsMenuUIController : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset questsListTemplate;
        [SerializeField] private VisualTreeAsset locationListQuestItemTemplate;
        [SerializeField] private VisualTreeAsset descriptionListQuestItemTemplate;
        [SerializeField] private VisualTreeAsset questDetailsTemplate;
        [SerializeField] private VisualTreeAsset questStepTemplate;
        [SerializeField] private VisualTreeAsset actionInstructionTemplate;
        [SerializeField] private VisualTreeAsset actionInstructionSeparatorTemplate;
        [SerializeField] private VisualTreeAsset itemSlotTemplate;
        [SerializeField] private VisualTreeAsset rewardItemDetailsOverlay;
        [SerializeField] private VisualTreeAsset statContainerTemplate;
        [SerializeField] private AQuestSO[] devQuests;
        private VisualElement _questDetailsPanelRoot;

        private QuestsListMenuUIController _questsListMenuUIController;
        private QuestDetailsPanelUIController _questDetailsPanelUIController;
        private AQuestSO[] _questsToShow;

        #region Setup

        protected override void Awake()
        {
            base.Awake();

            if (questsListTemplate == null)
            {
                Debug.LogError("QuestsListTemplate is not set in the inspector.");
                return;
            }

            if (locationListQuestItemTemplate == null)
            {
                Debug.LogError("ListQuestItemTemplate is not set in the inspector.");
                return;
            }

            if (questDetailsTemplate == null)
            {
                Debug.LogError("QuestDetailsTemplate is not set in the inspector.");
                return;
            }

            if (questStepTemplate == null)
            {
                Debug.LogError("QuestStepTemplate is not set in the inspector.");
                return;
            }

            if (actionInstructionTemplate == null)
            {
                Debug.LogError("ActionInstructionTemplate is not set in the inspector.");
                return;
            }

            if (actionInstructionSeparatorTemplate == null)
            {
                Debug.LogError("ActionInstructionSeparatorTemplate is not set in the inspector.");
            }

            if (itemSlotTemplate == null)
            {
                Debug.LogError("ItemSlotTemplate is not set in the inspector.");
                return;
            }

            if (rewardItemDetailsOverlay == null)
            {
                Debug.LogError("RewardItemDetailsOverlay is not set in the inspector.");
                return;
            }

            if (statContainerTemplate == null)
            {
                Debug.LogError("StatContainerTemplate is not set in the inspector.");
                return;
            }
        }

        #endregion

        public override void SetupMenu()
        {
            SetQuestsToShow();

            VisualElement questsListMenuRoot = questsListTemplate.Instantiate();
            questsListMenuRoot.StretchToParentSize();
            _questsListMenuUIController = new QuestsListMenuUIController(
                questsListMenuRoot,
                _questsToShow,
                locationListQuestItemTemplate,
                descriptionListQuestItemTemplate
            );
            _questsListMenuUIController.onQuestSelected += OnQuestSelected;
            _questsListMenuUIController.onQuestUnselected += HideQuestDetailsPanel;
            _questsListMenuUIController.onQuestsFilterChanged += HideQuestDetailsPanel;

            _questDetailsPanelRoot = questDetailsTemplate.Instantiate();
            _questDetailsPanelRoot.StretchToParentSize();
            _questDetailsPanelUIController = new QuestDetailsPanelUIController(
                _questDetailsPanelRoot,
                questStepTemplate,
                actionInstructionTemplate,
                actionInstructionSeparatorTemplate,
                itemSlotTemplate,
                rewardItemDetailsOverlay,
                statContainerTemplate
            );

            _leftPageContainer.Add(questsListMenuRoot);
            _rightPageContainer.Add(_questDetailsPanelRoot);
        }

        public override void ClearMenu()
        {
            base.ClearMenu();
            _questsListMenuUIController = null;
            _questDetailsPanelRoot = null;
        }

        private void OnQuestSelected(AQuestSO selectedQuest)
        {
            if (selectedQuest == null)
            {
                Debug.LogError("Selected quest is null.");
                return;
            }

            _uiDoc.StartCoroutine(_questDetailsPanelUIController.DisplayQuestDetails(selectedQuest));
        }

        private void HideQuestDetailsPanel()
        {
            if (_questDetailsPanelUIController.IsDisplayed())
                _uiDoc.StartCoroutine(_questDetailsPanelUIController.ResetAndHidePanel());
        }

        private void SetQuestsToShow()
        {
            // Add dev quests to the hero team quests singleton if defined
            if (devQuests != null && devQuests.Length > 0)
            {
                foreach (AQuestSO devQuest in devQuests)
                {
                    if (!HeroTeamQuests.Instance.Quests.Contains(devQuest))
                    {
                        HeroTeamQuests.Instance.AddQuest(devQuest);
                    }
                }

                _questsToShow = HeroTeamQuests.Instance.Quests.ToArray();
            }

            _questsToShow = HeroTeamQuests.Instance.Quests.ToArray();
        }
    }
}