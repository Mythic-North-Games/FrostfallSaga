using System;
using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils.UI;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class TimelineCharacterUIController
    {
        #region UXML UI Names & Classes
        private static readonly string CHARACTER_ICON_CONTAINER_UI_NAME = "TimelineCharacterIconContainer";
        private static readonly string CHARACTER_ICON_UI_NAME = "TimelineCharacterIcon";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "TimelineCharacterStatusesContainer";

        private static readonly string CHARACTER_ICON_CONTAINER_TOP_CLASSNAME = "timelineCharacterContainerTop";
        private static readonly string CHARACTER_ICON_CONTAINER_MIDDLE_CLASSNAME = "timelineCharacterContainerMiddle";
        private static readonly string CHARACTER_ICON_CONTAINER_BOTTOM_CLASSNAME = "timelineCharacterContainerBottom";
        private static readonly string CHARACTER_ICON_CONTAINER_SOLO_CLASSNAME = "timelineCharacterContainerSolo";
        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "statusIconContainerRoot";
        #endregion

        public Action<TimelineCharacterUIController> onFighterHovered;
        public Action<TimelineCharacterUIController> onFighterUnhovered;
        public Action<TimelineCharacterUIController, AStatus, int> onStatusIconHovered;
        public Action<TimelineCharacterUIController, AStatus, int> onStatusIconUnhovered;

        private readonly VisualElement _characterIconContainer;
        private readonly VisualTreeAsset _statusIconContainerTemplate;

        public TimelineCharacterUIController(
            VisualElement root,
            Fighter fighter,
            VisualTreeAsset statusIconContainerTemplate,
            ETimelinePosition positionInTimeline
        )
        {
            Root = root;
            Fighter = fighter;
            _statusIconContainerTemplate = statusIconContainerTemplate;

            //////////////
            // Setup UI //
            //////////////
            // Setup character icon life progress
            _characterIconContainer = Root.Q<VisualElement>(CHARACTER_ICON_CONTAINER_UI_NAME);
            _characterIconContainer.AddToClassList(GetTimelinePositionClassName(positionInTimeline));
            UpdateHealth();
            Fighter.onDamageReceived += (_, _, _, _) => UpdateHealth();
            Fighter.onHealReceived += (_, _, _) => UpdateHealth();

            // Setup character icon
            VisualElement characterIcon = Root.Q<VisualElement>(CHARACTER_ICON_UI_NAME);
            characterIcon.style.backgroundImage = new(fighter.Icon);

            // Setup statuses
            UpdateStatuses();
            fighter.onStatusApplied += (_fighter, _status) => UpdateStatuses();
            fighter.onStatusRemoved += (_fighter, _status) => UpdateStatuses();

            // Setup hover events
            characterIcon.RegisterCallback<MouseEnterEvent>(_ => onFighterHovered?.Invoke(this));
            characterIcon.RegisterCallback<MouseLeaveEvent>(_ => onFighterUnhovered?.Invoke(this));
        }

        public VisualElement Root { get; private set; }
        public Fighter Fighter { get; private set; }

        private void UpdateHealth()
        {
            ProgressBarUIController.SetupProgressBar(
                _characterIconContainer,
                Fighter.GetHealth(),
                Fighter.GetMaxHealth(),
                adjustWidth: false,
                adjustHeight: true,
                invertProgress: true,
                displayValueLabel: false
            );
        }

        private void UpdateStatuses()
        {
            Dictionary<AStatus, (bool isActive, int duration)> statuses = Fighter.GetStatuses();

            VisualElement statusesContainer = Root.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME);
            statusesContainer.Clear();

            // Hide the container if no statuses
            if (statuses.Count == 0)
            {
                statusesContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                Root.style.marginRight = new StyleLength(new Length(-32.5f, LengthUnit.Percent));
                return;
            }

            // Otherwise, display the statuses
            statusesContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            Root.style.marginRight = new StyleLength(new Length(0, LengthUnit.Percent));
            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in Fighter.GetStatuses())
            {
                VisualElement statusIconContainerRoot = _statusIconContainerTemplate.Instantiate();
                statusIconContainerRoot.AddToClassList(STATUS_ICON_CONTAINER_ROOT_CLASSNAME);
                StatusContainerUIController.SetupStatusContainer(statusIconContainerRoot, status.Key);

                // Setup long hover events
                LongHoverEventController<VisualElement> longHoverEventController = new(statusIconContainerRoot);
                longHoverEventController.onElementLongHovered += _ =>
                    onStatusIconHovered?.Invoke(this, status.Key, status.Value.duration);
                longHoverEventController.onElementLongUnhovered += _ =>
                    onStatusIconUnhovered?.Invoke(this, status.Key, status.Value.duration);

                statusesContainer.Add(statusIconContainerRoot);
            }
        }

        private static string GetTimelinePositionClassName(ETimelinePosition position)
        {
            return position switch
            {
                ETimelinePosition.TOP => CHARACTER_ICON_CONTAINER_TOP_CLASSNAME,
                ETimelinePosition.MIDDLE => CHARACTER_ICON_CONTAINER_MIDDLE_CLASSNAME,
                ETimelinePosition.BOTTOM => CHARACTER_ICON_CONTAINER_BOTTOM_CLASSNAME,
                ETimelinePosition.SOLO => CHARACTER_ICON_CONTAINER_SOLO_CLASSNAME,
                _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
            };
        }

        public enum ETimelinePosition
        {
            TOP,
            MIDDLE,
            BOTTOM,
            SOLO
        }
    }
}