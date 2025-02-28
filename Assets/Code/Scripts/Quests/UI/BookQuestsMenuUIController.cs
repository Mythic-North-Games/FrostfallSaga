using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Quests;

namespace FrostfallSaga.Quests.UI
{
    public class BookQuestsMenuUIController : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset _questsListTemplate;
        [SerializeField] private VisualTreeAsset _listQuestItemTemplate;
        [SerializeField] private VisualTreeAsset _questDetailsTemplate;
        [SerializeField] private VisualTreeAsset _questStepTemplate;
        [SerializeField] private VisualTreeAsset _actionInstructionTemplate;
        [SerializeField] private AQuestSO[] _devQuests;

        private QuestsListMenuUIController _questsListMenuUIController;
        private VisualElement _questDetailsPanelRoot;

        public override void SetupMenu()
        {
            VisualElement questsListMenuRoot = _questsListTemplate.Instantiate();
            questsListMenuRoot.StretchToParentSize();
            _questsListMenuUIController = new QuestsListMenuUIController(
                questsListMenuRoot,
                _listQuestItemTemplate,
                _devQuests
            );
            _questsListMenuUIController.onQuestSelected += OnQuestSelected;
            _questsListMenuUIController.onQuestsFilterChanged += OnQuestsFilterChanged;

            _questDetailsPanelRoot = _questDetailsTemplate.Instantiate();
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
                _questStepTemplate,
                _actionInstructionTemplate,
                selectedQuest
            );
        }

        private void OnQuestsFilterChanged()
        {
            QuestDetailsPanelUIController.ResetQuestDetailsPanel(_questDetailsPanelRoot);
        }

        #region Setup
        protected override void Awake()
        {
            base.Awake();

            if (_questsListTemplate == null)
            {
                Debug.LogError("QuestsListTemplate is not set in the inspector.");
                return;
            }

            if (_listQuestItemTemplate == null)
            {
                Debug.LogError("ListQuestItemTemplate is not set in the inspector.");
                return;
            }

            if (_questDetailsTemplate == null)
            {
                Debug.LogError("QuestDetailsTemplate is not set in the inspector.");
                return;
            }

            if (_questStepTemplate == null)
            {
                Debug.LogError("QuestStepTemplate is not set in the inspector.");
                return;
            }

            if (_actionInstructionTemplate == null)
            {
                Debug.LogError("ActionInstructionTemplate is not set in the inspector.");
                return;
            }
        }
        #endregion
    }
}