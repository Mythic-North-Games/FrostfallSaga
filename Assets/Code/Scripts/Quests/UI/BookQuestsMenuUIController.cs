using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Quests;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class BookQuestsMenuUIController : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset questsListTemplate;
        [SerializeField] private VisualTreeAsset listQuestItemTemplate;
        [SerializeField] private VisualTreeAsset questDetailsTemplate;
        [SerializeField] private VisualTreeAsset questStepTemplate;
        [SerializeField] private VisualTreeAsset actionInstructionTemplate;
        [SerializeField] private AQuestSO[] devQuests;
        private VisualElement _questDetailsPanelRoot;

        private QuestsListMenuUIController _questsListMenuUIController;
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

            if (listQuestItemTemplate == null)
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
                listQuestItemTemplate,
                _questsToShow
            );
            _questsListMenuUIController.onQuestSelected += OnQuestSelected;
            _questsListMenuUIController.onQuestsFilterChanged += OnQuestsFilterChanged;

            _questDetailsPanelRoot = questDetailsTemplate.Instantiate();
            _questDetailsPanelRoot.StretchToParentSize();
            QuestDetailsPanelUIController.ResetQuestDetailsPanel(_questDetailsPanelRoot);

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
            QuestDetailsPanelUIController.DisplayQuestDetails(
                _questDetailsPanelRoot,
                questStepTemplate,
                actionInstructionTemplate,
                selectedQuest
            );
        }

        private void OnQuestsFilterChanged()
        {
            QuestDetailsPanelUIController.ResetQuestDetailsPanel(_questDetailsPanelRoot);
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