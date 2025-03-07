using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FightersOrderTimelineController : BaseUIController
    {
        private static readonly string TIMELINE_UI_NAME = "TimelinePanel";
        private static readonly string CHARACTER_CONTAINER_UI_NAME = "TimelineCharacterContainer";
        private static readonly string CHARACTER_BACKGROUND_UI_NAME = "TimelineCharacterBackground";

        [SerializeField] private FightManager _fightManager;
        [SerializeField] private FighterDetailsPanelController _fighterDetailsPanelController;
        [SerializeField] private Vector2Int _fighterDetailsOffset = new(-20, -50);

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

            _fightManager.onFightersTurnOrderUpdated += OnFightersTurnOrderUpdated;
        }

        #endregion

        private void OnFightersTurnOrderUpdated(Fighter[] fighters)
        {
            VisualElement[] characterContainers = GetCharacterContainers();

            var containerIndex = 0;
            while (containerIndex < characterContainers.Length)
            {
                VisualElement characterContainer = characterContainers[containerIndex];
                RemoveAllVisualElementChildren(characterContainer); // Clean before updating
                if (containerIndex < fighters.Length)
                    SetupCharacterContainerForFighter(characterContainer, fighters[containerIndex]);
                else
                    characterContainer.style.display = DisplayStyle.None;
                containerIndex++;
            }
        }

        private void RemoveAllVisualElementChildren(VisualElement visualElement)
        {
            visualElement.Children().ToList().ForEach(child => visualElement.Remove(child));
        }

        private void SetupCharacterContainerForFighter(VisualElement characterContainer, Fighter fighter)
        {
            VisualElement characterBackground = new()
            {
                name = $"{CHARACTER_BACKGROUND_UI_NAME}{characterContainer.name[^1]}"
            };
            characterBackground.style.backgroundImage = new StyleBackground(fighter.Icon);
            characterBackground.style.width = new StyleLength(new Length(90, LengthUnit.Percent));
            characterBackground.style.height = new StyleLength(new Length(90, LengthUnit.Percent));
            characterBackground.style.marginLeft = new StyleLength(new Length(5, LengthUnit.Percent));
            characterBackground.style.marginTop = new StyleLength(new Length(5, LengthUnit.Percent));
            characterBackground.RegisterCallback<MouseOverEvent>(evt =>
                {
                    Vector2Int timelinePosition = new(
                        (int)_uiDoc.rootVisualElement.Q(TIMELINE_UI_NAME).worldBound.x,
                        (int)_uiDoc.rootVisualElement.Q(TIMELINE_UI_NAME).worldBound.y
                    );
                    Vector2Int panelSize = _fighterDetailsPanelController.GetPanelSize();
                    Vector2Int displayPosition = new(
                        timelinePosition.x - panelSize.x + _fighterDetailsOffset.x,
                        timelinePosition.y + _fighterDetailsOffset.y
                    );
                    _fighterDetailsPanelController.Display(fighter, displayPosition);
                }
            );
            characterBackground.RegisterCallback<MouseOutEvent>(
                evt => _fighterDetailsPanelController.Hide()
            );
            characterContainer.Add(characterBackground);
        }

        private VisualElement[] GetCharacterContainers()
        {
            List<VisualElement> characterContainers = new();

            var availableCharacterContainersCount = _uiDoc.rootVisualElement.Q(TIMELINE_UI_NAME).childCount - 1;
            for (var containerIndex = 0; containerIndex < availableCharacterContainersCount; containerIndex++)
                characterContainers.Add(_uiDoc.rootVisualElement.Q($"{CHARACTER_CONTAINER_UI_NAME}{containerIndex}"));

            return characterContainers.ToArray();
        }
    }
}