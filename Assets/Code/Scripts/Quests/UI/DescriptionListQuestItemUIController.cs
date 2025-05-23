using FrostfallSaga.Core.Quests;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class DescriptionListQuestItemUIController : ListQuestItemUIController
    {
        #region UI Elements Names & Classes
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "TeaserDescription";
        #endregion

        private readonly Label _locationLabel;

        public DescriptionListQuestItemUIController(VisualElement root) : base(root)
        {
            _locationLabel = root.Q<Label>(DESCRIPTION_LABEL_UI_NAME);
        }

        public override void SetQuest(AQuestSO quest)
        {
            base.SetQuest(quest);
            _locationLabel.text = quest.TeaserDescription;
        }
    }
}