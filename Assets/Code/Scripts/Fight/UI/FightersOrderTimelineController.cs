using System;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FightersOrderTimelineController : BaseUIController
    {
        #region UXML UI Names & Classes
        private static readonly string TIMELINE_ROOT_UI_NAME = "TimelinePanelRoot";
        private static readonly string TIMELINE_PANEL_UI_NAME = "TimelinePanel";
        private static readonly string TIMELINE_CONTENT_CONTAINER_UI_NAME = "TimelineContentContainer";
        private static readonly string FIGHTER_RESISTANCE_PANEL_UI_NAME = "FighterResistancesPanel";

        private static readonly string TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME = "timelineCharacterContainerRoot";
        #endregion

        [SerializeField] private VisualTreeAsset _characterContainerTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private VisualTreeAsset _statusIconContainerTemplate;
        [SerializeField] private VisualTreeAsset _statusDetailsOverlayTemplate;
        [SerializeField] private FightManager _fightManager;

        public Action<Fighter> onFighterHovered;
        public Action<Fighter> onFighterUnhovered;

        private VisualElement _timelineContentContainer;
        private FighterResistancesPanelController _resistancesPanelController;
        private StatusDetailsOverlayUIController _statusesDetailsOverlayController;

        #region Setup & tear down

        private void Awake()
        {
            if (_uiDoc == null) _uiDoc = GetComponent<UIDocument>();
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
            if (_fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            if (_characterContainerTemplate == null)
            {
                Debug.LogError("No character container template to work with. UI can't be updated dynamically.");
                return;
            }

            VisualElement timelineRoot = _uiDoc.rootVisualElement.Q<VisualElement>(TIMELINE_ROOT_UI_NAME);

            ScrollView timelinePanel = timelineRoot.Q<ScrollView>(TIMELINE_PANEL_UI_NAME);
            timelinePanel.contentContainer.StretchToParentSize();
            timelinePanel.contentContainer.style.minHeight = new StyleLength(new Length(100, LengthUnit.Percent));
            _timelineContentContainer = timelineRoot.Q<VisualElement>(TIMELINE_CONTENT_CONTAINER_UI_NAME);

            _resistancesPanelController = new(
                timelineRoot.Q<VisualElement>(FIGHTER_RESISTANCE_PANEL_UI_NAME),
                _statContainerTemplate
            );
            _resistancesPanelController.Hide();

            _statusesDetailsOverlayController = new(_statusDetailsOverlayTemplate);

            _fightManager.onFightersTurnOrderUpdated += OnFightersTurnOrderUpdated;
            _fightManager.onFightEnded += (_, _) => _timelineContentContainer.RemoveFromHierarchy();
        }

        #endregion

        private void OnFightersTurnOrderUpdated(Fighter[] fighters)
        {
            // Clear the previous content
            _timelineContentContainer.Clear();

            // Compute the new height for the character containers
            float timelineCharacterContainerHeight = GetTimelineCharacterContainerHeight(fighters.Length);

            // Instantiate the new character containers for each fighter
            for (int i = 0;  i < fighters.Length; i++)
            {
                Fighter fighter = fighters[i];

                // Configure the character container
                VisualElement characterContainerRoot = _characterContainerTemplate.Instantiate();
                characterContainerRoot.AddToClassList(TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME);

                TimelineCharacterUIController characterUIController = new(
                    root: characterContainerRoot,
                    fighter: fighter,
                    statusIconContainerTemplate: _statusIconContainerTemplate,
                    positionInTimeline: GetPositionInTimeline(i, fighters.Length)
                );
                characterUIController.onFighterHovered += OnFighterHovered;
                characterUIController.onFighterUnhovered += OnFighterUnhovered;
                characterUIController.onStatusIconHovered += OnStatusIconHovered;
                characterUIController.onStatusIconUnhovered += OnStatusIconUnhovered;

                // Add the character container to the timeline content container
                _timelineContentContainer.Add(characterContainerRoot);
            }
        }

        private static TimelineCharacterUIController.ETimelinePosition GetPositionInTimeline(
            int fighterIndex,
            int totalFighters
        )
        {
            if (totalFighters == 1) return TimelineCharacterUIController.ETimelinePosition.SOLO;
            if (fighterIndex == 0) return TimelineCharacterUIController.ETimelinePosition.TOP;
            if (fighterIndex == totalFighters - 1) return TimelineCharacterUIController.ETimelinePosition.BOTTOM;
            return TimelineCharacterUIController.ETimelinePosition.MIDDLE;
        }

        private void OnFighterHovered(TimelineCharacterUIController hoveredCharacter)
        {
            _resistancesPanelController.Display(hoveredCharacter);
            onFighterHovered?.Invoke(hoveredCharacter.Fighter);
        }

        private void OnFighterUnhovered(TimelineCharacterUIController unhoveredCharacter)
        {
            _resistancesPanelController.Hide();
            onFighterUnhovered?.Invoke(unhoveredCharacter.Fighter);
        }

        private void OnStatusIconHovered(TimelineCharacterUIController character, AStatus status, int lastingDuration)
        {
            _statusesDetailsOverlayController.SetStatus(status, lastingDuration);
            _statusesDetailsOverlayController.ShowOverlay(followMouse: true);
        }

        private void OnStatusIconUnhovered(TimelineCharacterUIController character, AStatus status, int lastingDuration)
        {
            _statusesDetailsOverlayController.HideOverlay();
        }

        private static float GetTimelineCharacterContainerHeight(int characterContainerCount)
        {
            if (characterContainerCount == 0) return 0; // Avoid division by zero

            // INFO: Formula to compute the height of the character containers
            return (314.31f / characterContainerCount) - 2.08f;
        }
    }
}