using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class RoundsCountLabelController : BaseUIController
    {
        private static readonly string ROUNDS_COUNT_LABEL_UI_NAME = "RoundsCountLabel";

        [SerializeField] private FightManager _fightManager;
        private int _roundsCount;
        private Label _roundsCountLabel;

        private void OnFighterTurnBegan(Fighter currentFighter, bool isAlly)
        {
            IncreaseRoundCount();
        }

        private void IncreaseRoundCount()
        {
            _roundsCount++;
            _roundsCountLabel.text = _roundsCount.ToString();
        }

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

            _roundsCount = 0;
            _roundsCountLabel = _uiDoc.rootVisualElement.Q<Label>(ROUNDS_COUNT_LABEL_UI_NAME);

            _fightManager.onFighterTurnBegan += OnFighterTurnBegan;
        }

        private void OnDisable()
        {
            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
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