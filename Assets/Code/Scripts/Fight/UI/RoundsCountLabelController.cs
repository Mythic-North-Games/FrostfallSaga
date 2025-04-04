using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class RoundsCountLabelController : BaseUIController
    {
        private static readonly string ROUNDS_COUNT_LABEL_UI_NAME = "RoundsCountLabel";

        [SerializeField] private FightManager fightManager;
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
            _uiDoc ??= GetComponent<UIDocument>();
            if (!_uiDoc)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            fightManager ??= FindObjectOfType<FightManager>();
            if (!fightManager)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            _roundsCount = 0;
            _roundsCountLabel = _uiDoc.rootVisualElement.Q<Label>(ROUNDS_COUNT_LABEL_UI_NAME);

            fightManager.onFighterTurnBegan += OnFighterTurnBegan;
        }

        private void OnDisable()
        {
            fightManager ??= FindObjectOfType<FightManager>();
            if (!fightManager)
            {
                Debug.LogWarning("No FightManager found. Can't tear down properly.");
                return;
            }

            fightManager.onFighterTurnBegan -= OnFighterTurnBegan;
        }

        #endregion
    }
}