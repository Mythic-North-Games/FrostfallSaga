using System;
using FrostfallSaga.City.UI;
using FrostfallSaga.Core.Cities.CitySituations;
using FrostfallSaga.Core.Dialogues;
using FrostfallSaga.Core.Quests;
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

            _cityMenuController.onCitySituationClicked += OnCitySituationClicked;
        }

        private void OnCitySituationClicked(ACitySituationSO citySituation)
        {
            _currentCitySituation = citySituation;
            if (citySituation.OnStartQuest != null)
            {
                // Add quest to hero team's quest list
                HeroTeamQuests.Instance.AddQuest(citySituation.OnStartQuest);
            }

            if (citySituation is CityDialogueSituation cityDialogueSituation)
            {
                _dialogueUIProcessor.onDialogueEnded += OnDialogueEnded;
                _dialogueUIProcessor.ProcessDialogue(cityDialogueSituation.Dialogue);
            }
        }

        private void EndCitySituation()
        {
            _currentCitySituation.ResolveSituation();
            onSituationResolved?.Invoke(_currentCitySituation);
        }

        private void OnDialogueEnded(DialogueSO _endedDialogue)
        {
            _dialogueUIProcessor.onDialogueEnded -= OnDialogueEnded;
            EndCitySituation();
        }
    }
}