using System;
using FrostfallSaga.Core.Quests;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class ListQuestItemUIController
    {
        #region UI Elements Names & Classes
        private static readonly string QUEST_TITLE_LABEL_UI_NAME = "Title";
        private static readonly string QUEST_TRACKING_ICON_UI_NAME = "QuestTrackingIcon";
        private static readonly string QUEST_COMPLETED_ICON_UI_NAME = "QuestCompletedIcon";

        private static readonly string QUEST_TRACKING_ICON_TRACKED_CLASSNAME = "questTrackingIconTracked";
        #endregion

        public Action<VisualElement> onQuestSelected;
        public Action<VisualElement> onQuestTrackingToggled;

        public AQuestSO ControlledQuest { get; private set; }

        private readonly VisualElement _root;
        private readonly VisualElement _titleLabel;
        private readonly VisualElement _trackingIcon;
        private readonly VisualElement _completedIcon;

        /// <summary>
        /// Add a new quest item to the given container then prepare the control of the quest item.
        /// </summary>
        /// <param name="root">The root visual element of thhe quest item in the list.</param>
        protected ListQuestItemUIController(VisualElement root)
        {
            _root = root;
            _titleLabel = _root.Q(QUEST_TITLE_LABEL_UI_NAME);
            _trackingIcon = _root.Q(QUEST_TRACKING_ICON_UI_NAME);
            _completedIcon = _root.Q(QUEST_COMPLETED_ICON_UI_NAME);

            _root.RegisterCallback<MouseUpEvent>(OnQuestClicked);
        }

        public virtual void SetQuest(AQuestSO quest)
        {
            ControlledQuest = quest;

            _titleLabel.Q<Label>(QUEST_TITLE_LABEL_UI_NAME).text = quest.Name;

            _trackingIcon.visible = !quest.IsCompleted;
            _trackingIcon.EnableInClassList(QUEST_TRACKING_ICON_TRACKED_CLASSNAME, quest.IsTracked);

            _completedIcon.visible = quest.IsCompleted;
        }

        private void OnQuestClicked(MouseUpEvent mouseUpEvent)
        {
            mouseUpEvent.StopPropagation();
            if (mouseUpEvent.button == (int)MouseButton.LeftMouse) onQuestSelected?.Invoke(_root);
            else if (mouseUpEvent.button == (int)MouseButton.RightMouse)
            {
                if (ControlledQuest.IsCompleted)
                {
                    CoroutineRunner.Instance.StartCoroutine(CommonUIAnimations.PlayShakeAnimation(_completedIcon));
                }
                else
                {
                    CoroutineRunner.Instance.StartCoroutine(
                        CommonUIAnimations.PlayScaleAnimation(_trackingIcon, new(1.2f, 1.2f), 0.2f)
                    );
                    onQuestTrackingToggled?.Invoke(_root);
                }
            }
        }
    }
}