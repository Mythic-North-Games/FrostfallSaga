using System;
using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class TimelineCharacterUIController
    {
        private readonly VisualElement _characterIconContainer;

        private readonly VisualTreeAsset _statusIconContainerTemplate;

        public Action<TimelineCharacterUIController> onFighterHovered;
        public Action<TimelineCharacterUIController> onFighterUnhovered;
        public Action<TimelineCharacterUIController, AStatus, int> onStatusIconHovered;
        public Action<TimelineCharacterUIController, AStatus, int> onStatusIconUnhovered;

        public TimelineCharacterUIController(
            VisualElement root,
            Fighter fighter,
            VisualTreeAsset statusIconContainerTemplate
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
                statusIconContainerRoot.RegisterCallback<MouseEnterEvent>(
                    _ => onStatusIconHovered?.Invoke(this, status.Key, status.Value.duration)
                );
                statusIconContainerRoot.RegisterCallback<MouseLeaveEvent>(
                    _ => onStatusIconUnhovered?.Invoke(this, status.Key, status.Value.duration)
                );
                statusesContainer.Add(statusIconContainerRoot);
            }
        }

        #region UXML UI Names & Classes

        private static readonly string CHARACTER_ICON_CONTAINER_UI_NAME = "TimelineCharacterContainer";
        private static readonly string CHARACTER_ICON_UI_NAME = "TimelineCharacterIcon";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "TimelineCharacterStatusesContainer";

        private static readonly string STATUS_ICON_CONTAINER_ROOT_CLASSNAME = "statusIconContainerRoot";

        #endregion
    }
}