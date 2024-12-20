using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.Fight.UI
{
    public class TurnLabelController : BaseUIController
    {
        private static readonly string ROUND_COUNTER_UI_NAME = "RoundPanel_TextNumRound";

        [SerializeField] private FightManager _fightManager;
        private Label _roundCounter;
        private int _roundCount;

        private void OnFighterTurnBegan(Fighter currentFighter, bool isAlly)
        {
            IncreaseRoundCount();
        }

        private void IncreaseRoundCount()
        {
            _roundCount++;
            _roundCounter.text = _roundCount.ToString();
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

            _roundCount = 0;
            _roundCounter = _uiDoc.rootVisualElement.Q<Label>(ROUND_COUNTER_UI_NAME);

            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
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

            _fightManager.onFighterTurnBegan -= OnFighterTurnBegan;
        }

        #endregion
    }
}