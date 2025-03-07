using System;
using FrostfallSaga.City.UI;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.Dialogues;
using UnityEngine;

namespace FrostfallSaga.City
{
    public class SituationsController : MonoBehaviour
    {
        [SerializeField] private DialogueUIProcessor _dialogueUIProcessor;
        [SerializeField] private CityMenuController _cityMenuController;

        private ACitySituationSO _currentCitySituation;
        public Action<ACitySituationSO> onSituationResolved;

        private void Awake()
        {
            if (_cityMenuController == null)
            {
                Debug.LogError("CityMenuController is not assigned in SituationsController.");
                return;
            }

            _cityMenuController.OnCitySituationClicked += OnCitySituationClicked;
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