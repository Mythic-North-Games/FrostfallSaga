using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FightersOrderTimelineController : BaseUIController
    {
        #region UXML UI Names & Classes
        private static readonly string TIMELINE_CONTENT_CONTAINER_UI_NAME = "TimelineContentContainer";
        private static readonly string TIMELINE_UI_NAME = "TimelinePanel";

        private static readonly string TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME = "timelineCharacterContainerRoot";
        #endregion

        [SerializeField] private VisualTreeAsset _characterContainerTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private VisualTreeAsset _statusIconContainerTemplate;
        [SerializeField] private FightManager _fightManager;

        private VisualElement _timelineContentContainer;

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

            _fightManager.onFightersTurnOrderUpdated += OnFightersTurnOrderUpdated;
            _uiDoc.rootVisualElement.Q<ScrollView>(TIMELINE_UI_NAME).contentContainer.StretchToParentSize();
            _uiDoc.rootVisualElement.Q<ScrollView>(TIMELINE_UI_NAME).contentContainer.style.minHeight = new StyleLength(new Length(100, LengthUnit.Percent));
            _timelineContentContainer = _uiDoc.rootVisualElement.Q<VisualElement>(TIMELINE_CONTENT_CONTAINER_UI_NAME);
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
                VisualElement characterContainerRoot = _characterContainerTemplate.Instantiate();
                characterContainerRoot.AddToClassList(TIMELINE_CHARACTER_CONTAINER_ROOT_CLASSNAME);
                new TimelineCharacterUIController(characterContainerRoot, fighter, _statusIconContainerTemplate, _statContainerTemplate);

                // Remove the character container from the timeline content container when the fighter dies
                fighter.onFighterDied += _ =>
                {
                    characterContainerRoot.RemoveFromHierarchy();
                };

                // Add the character container to the timeline content container
                _timelineContentContainer.Add(characterContainerRoot);
            }
        }

        private float GetTimelineCharacterContainerHeight(int characterContainerCount)
        {
            if (characterContainerCount == 0) return 0; // Avoid division by zero
            return (314.31f / characterContainerCount) - 2.08f; // INFO: Formula to compute the height of the character containers
        }

    }
}