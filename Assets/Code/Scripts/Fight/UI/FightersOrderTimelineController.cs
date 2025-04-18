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
        [SerializeField] private VisualTreeAsset characterContainerTemplate;
        [SerializeField] private VisualTreeAsset statContainerTemplate;
        [SerializeField] private VisualTreeAsset statusIconContainerTemplate;
        [SerializeField] private FightManager fightManager;
        private FighterResistancesPanelController _resistancesPanelController;
        private StatusDetailsPanelUIController _statusesDetailsPanelController;

        private VisualElement _timelineContentContainer;

        public Action<Fighter> onFighterHovered;
        public Action<Fighter> onFighterUnhovered;

        #region Setup & tear down

        private void Awake()
        {
            if (_uiDoc == null) _uiDoc = GetComponent<UIDocument>();
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (fightManager == null) fightManager = FindObjectOfType<FightManager>();
            if (fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            if (characterContainerTemplate == null)
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
                statContainerTemplate
            );
            _resistancesPanelController.Hide();

            _statusesDetailsPanelController = new(timelineRoot.Q<VisualElement>(STATUS_DETAILS_PANEL_UI_NAME));
            _statusesDetailsPanelController.Hide();

            fightManager.onFightersTurnOrderUpdated += OnFightersTurnOrderUpdated;
            fightManager.onFightEnded += (_, _) => _timelineContentContainer.RemoveFromHierarchy();
        }

        #endregion

        private void OnFightersTurnOrderUpdated(Fighter[] fighters)
        {
            // Clear the previous content
            _timelineContentContainer.Clear();

            // Compute the new height for the character containers
            float timelineCharacterContainerHeight = GetTimelineCharacterContainerHeight(fighters.Length);

            // Instantiate the new character containers for each fighter
            foreach (Fighter fighter in fighters)
            {
                // Configure the character container
                VisualElement characterContainerRoot = characterContainerTemplate.Instantiate();
                characterContainerRoot.AddToClassList(TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME);

                TimelineCharacterUIController characterUIController = new(
                    characterContainerRoot, fighter, statusIconContainerTemplate
                );
                characterUIController.onFighterHovered += OnFighterHovered;
                characterUIController.onFighterUnhovered += OnFighterUnhovered;
                characterUIController.onStatusIconHovered += OnStatusIconHovered;
                characterUIController.onStatusIconUnhovered += OnStatusIconUnhovered;

                // Add the character container to the timeline content container
                _timelineContentContainer.Add(characterContainerRoot);
            }
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
            _statusesDetailsPanelController.Root.style.top =
                character.Root.worldBound.y - character.Root.worldBound.height + 20;
            _statusesDetailsPanelController.Display(status, lastingDuration);
        }

        private void OnStatusIconUnhovered(TimelineCharacterUIController character, AStatus status, int lastingDuration)
        {
            _statusesDetailsPanelController.Hide();
        }

        private static float GetTimelineCharacterContainerHeight(int characterContainerCount)
        {
            if (characterContainerCount == 0) return 0; // Avoid division by zero
            return
                (314.31f / characterContainerCount) -
                2.08f; // INFO: Formula to compute the height of the character containers
        }

        #region UXML UI Names & Classes

        private static readonly string TIMELINE_ROOT_UI_NAME = "TimelinePanelRoot";
        private static readonly string TIMELINE_PANEL_UI_NAME = "TimelinePanel";
        private static readonly string TIMELINE_CONTENT_CONTAINER_UI_NAME = "TimelineContentContainer";
        private static readonly string FIGHTER_RESISTANCE_PANEL_UI_NAME = "FighterResistancesPanel";
        private static readonly string STATUS_DETAILS_PANEL_UI_NAME = "StatusDetailsPanel";

        private static readonly string TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME = "timelineCharacterContainerRoot";

        #endregion
    }
}