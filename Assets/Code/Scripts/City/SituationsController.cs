using System;
using UnityEngine;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.City.UI;

namespace FrostfallSaga.City
{
    public class SituationsController : MonoBehaviour
    {
        public Action<ACitySituationSO> onSituationResolved;

        [SerializeField] private DialogueUIProcessor _dialogueUIProcessor;
        [SerializeField] private CityMenuController _cityMenuController;

        private ACitySituationSO _currentCitySituation;

        private void Awake()
        {
            if (_cityMenuController == null)
            {
                Debug.LogError("CityMenuController is not assigned in SituationsController.");
                return;
            }
            
            _cityMenuController.onCitySituationClicked += OnCitySituationClicked;
        }

        private void OnCitySituationClicked(ACitySituationSO citySituation)
        {
            _currentCitySituation = citySituation;
            if (citySituation is CityDialogueSituation cityDialogueSituation)
            {
                _dialogueUIProcessor.onDialogueEnded += OnDialogueEnded;
                _dialogueUIProcessor.ProcessDialogue(cityDialogueSituation.Dialogue);
            }
        }

        private void OnDialogueEnded(DialogueSO _endedDialogue)
        {
            _dialogueUIProcessor.onDialogueEnded -= OnDialogueEnded;
            onSituationResolved?.Invoke(_currentCitySituation);
        }
    }
}