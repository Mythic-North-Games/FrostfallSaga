using System.Collections.Generic;
using System.Linq;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FightersOrderTimelineController : BaseUIController
    {
        [SerializeField] private FightManager _fightManager;

        private static readonly string TIMELINE_UI_NAME = "TimelinePanel";
        private static readonly string CHARACTER_CONTAINER_UI_NAME = "TimelineCharacterContainer";
        private static readonly string CHARACTER_BACKGROUND_UI_NAME = "TimelineCharacterBackground";

        private void OnFightersTurnOrderUpdated(Fighter[] fighters)
        {
            VisualElement[] characterContainers = GetCharacterContainers();

            int containerIndex = 0;
            while (containerIndex < characterContainers.Length)
            {
                VisualElement characterContainer = characterContainers[containerIndex];
                RemoveAllVisualElementChildren(characterContainer); // Clean before updating
                if (containerIndex < fighters.Length)
                {
                    SetupCharacterContainerForFighter(characterContainer, fighters[containerIndex]);
                }
                else
                {
                    characterContainer.style.display = DisplayStyle.None;
                }
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
            characterBackground.style.backgroundImage = new(fighter.FighterIcon);
            characterBackground.style.width = new(new Length(90, LengthUnit.Percent));
            characterBackground.style.height = new(new Length(90, LengthUnit.Percent));
            characterBackground.style.marginLeft = new(new Length(5, LengthUnit.Percent));
            characterBackground.style.marginTop = new(new Length(5, LengthUnit.Percent));
            characterContainer.Add(characterBackground);
        }

        private VisualElement[] GetCharacterContainers()
        {
            List<VisualElement> characterContainers = new();

            int availableCharacterContainersCount = _uiDoc.rootVisualElement.Q(TIMELINE_UI_NAME).childCount;
            for (int containerIndex = 0; containerIndex < availableCharacterContainersCount; containerIndex++)
            {
                characterContainers.Add(_uiDoc.rootVisualElement.Q($"{CHARACTER_CONTAINER_UI_NAME}{containerIndex}"));
            }

            return characterContainers.ToArray();
        }

        #region Setup & tear down

        private void Awake()
        {
            if (_uiDoc == null)
            {
                _uiDoc = GetComponent<UIDocument>();
            }
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (_fightManager == null)
            {
                _fightManager = FindObjectOfType<FightManager>();
            }
            if (_fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            _fightManager.onFightersTurnOrderUpdated += OnFightersTurnOrderUpdated;
        }

        private void OnDisable()
        {
            if (_fightManager == null)
            {
                _fightManager = FindObjectOfType<FightManager>();
            }
            if (_fightManager == null)
            {
                Debug.LogWarning("No FightManager found. Can't tear down properly.");
                return;
            }

            _fightManager.onFightersTurnOrderUpdated -= OnFightersTurnOrderUpdated;
        }

        #endregion
    }
}