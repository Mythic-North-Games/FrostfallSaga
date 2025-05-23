using FrostfallSaga.Core.Quests;
using UnityEngine.UIElements;

namespace FrostfallSaga.Quests.UI
{
    public class LocationListQuestItemUIController : ListQuestItemUIController
    {
        #region UI Elements Names & Classes
        private static readonly string LOCATION_LABEL_UI_NAME = "OriginLocation";
        #endregion

        private readonly Label _locationLabel;

        public LocationListQuestItemUIController(VisualElement root) : base(root)
        {
            _locationLabel = root.Q<Label>(LOCATION_LABEL_UI_NAME);
        }

        public override void SetQuest(AQuestSO quest)
        {
            base.SetQuest(quest);
            _locationLabel.text = quest.OriginLocation;
        }
    }
}