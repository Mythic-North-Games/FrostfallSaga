using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class TimelineCharacterUIController
    {
        #region UXML UI Names & Classes
        private static readonly string CHARACTER_ICON_UI_NAME = "TimelineCharacterIcon";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "TimelineCharacterStatusesContainer";
        private static readonly string RESISTANCES_PANEL_CONTAINER_UI_NAME = "FighterResistancesPanelRoot";

        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "statusIconContainerRoot";
        #endregion

        private readonly VisualElement _root;
        private readonly Fighter _fighter;
        private readonly VisualTreeAsset _statusIconContainerTemplate;
        private readonly VisualElement _resistancesPanelRoot;
        private readonly FighterResistancesPanelController _resistancesPanelController;

        public TimelineCharacterUIController(
            VisualElement root,
            Fighter fighter,
            VisualTreeAsset statusIconContainerTemplate,
            VisualTreeAsset statContainerTemplate
        )
        {
            _root = root;
            _fighter = fighter;
            _statusIconContainerTemplate = statusIconContainerTemplate;

            //////////////
            // Setup UI //
            //////////////
            // Setup character icon
            VisualElement characterIcon = _root.Q<VisualElement>(CHARACTER_ICON_UI_NAME);
            characterIcon.style.backgroundImage = new(fighter.Icon);

            // Setup resistances panel
            _resistancesPanelRoot = _root.Q<VisualElement>(RESISTANCES_PANEL_CONTAINER_UI_NAME);
            _resistancesPanelController = new FighterResistancesPanelController(
                _resistancesPanelRoot, fighter, statContainerTemplate
            );
            _resistancesPanelController.UpdateStats();
            _resistancesPanelController.Hide();
            fighter.onMagicalStatMutated += OnFighterMagicalStatMutated;

            // Setup statuses
            UpdateStatuses();
            fighter.onStatusApplied += (_fighter, _status) => UpdateStatuses();
            fighter.onStatusRemoved += (_fighter, _status) => UpdateStatuses();

            // Setup hover behaviors
            characterIcon.RegisterCallback<MouseEnterEvent>(_ => _resistancesPanelController.Display());
            characterIcon.RegisterCallback<MouseOutEvent>(_ => _resistancesPanelController.Hide());
        }

        private void UpdateStatuses()
        {
            Dictionary<AStatus, (bool isActive, int duration)> statuses = _fighter.GetStatuses();

            UpdateResistancesPanelLeft(statuses.Count);

            VisualElement statusesContainer = _root.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME);
            statusesContainer.Clear();

            // Hide the container if no statuses
            if (statuses.Count == 0)
            {
                statusesContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _root.style.marginRight = new StyleLength(new Length(-32.5f, LengthUnit.Percent));
                return;
            }

            // Otherwise, display the statuses
            statusesContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            _root.style.marginRight = new StyleLength(new Length(0, LengthUnit.Percent));
            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in _fighter.GetStatuses())
            {
                VisualElement statusIconContainerRoot = _statusIconContainerTemplate.Instantiate();
                statusIconContainerRoot.AddToClassList(STATUS_ICON_CONTAINER_ROOT_CLASSNAME);
                StatusContainerUIController.SetupStatusContainer(statusIconContainerRoot, status.Key);
                statusesContainer.Add(statusIconContainerRoot);
            }
        }

        private void OnFighterMagicalStatMutated(
            Fighter _updatedFighter,
            EMagicalElement magicalElementMutated,
            int updatedStatValue,
            bool isResistance
        )
        {
            // Only display resistances
            if (!isResistance) return;

            _resistancesPanelController.UpdateStats();
        }

        private void UpdateResistancesPanelLeft(int statusesCount)
        {
            float left;
            if (statusesCount > 2 || statusesCount == 0)
            {
                left = -55f;
            }
            else if (statusesCount == 2)
            {
                left = -40f;
            }
            else
            {
                left = -25f;
            }
            _resistancesPanelRoot.style.left = new StyleLength(new Length(left, LengthUnit.Percent));
        }
    }
}