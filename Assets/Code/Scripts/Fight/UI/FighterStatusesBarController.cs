using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    /// <summary>
    ///     Controller for the fighter statuses bars in the UI.
    /// </summary>
    public class FighterStatusesBarController
    {
        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesBarContainer";
        private static readonly string STATUS_CONTAINER_UI_NAME = "StatusContainer";

        private readonly VisualElement _root;

        public FighterStatusesBarController(VisualElement root)
        {
            _root = root;
        }

        public void UpdateStatuses(Fighter fighter)
        {
            int maxStatusesContainers = _root.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME).childCount;
            Dictionary<AStatus, (bool isActive, int duration)> currentFighterStatuses = fighter.GetStatuses();
            for (int i = 0; i < currentFighterStatuses.Count; i++)
            {
                if (i > maxStatusesContainers) break;
                VisualElement statusContainer = _root.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage =
                    new StyleBackground(currentFighterStatuses.ElementAt(i).Key.Icon);
            }

            for (int i = currentFighterStatuses.Count; i < maxStatusesContainers; i++)
            {
                VisualElement statusContainer = _root.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = null;
            }
        }
    }
}