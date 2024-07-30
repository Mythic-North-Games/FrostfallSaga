using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.UI;

namespace FrostfallSaga.Fight.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private FighterActionPanel _actionPanel;
        [SerializeField] private Material _cellHighlightMaterial;
        [SerializeField] private Material _cellActionableHighlightMaterial;
        [SerializeField] private Material _cellInaccessibleHighlightMaterial;

        private PlayerEndTurnController _endTurnController;
        private PlayerDirectAttackController _directAttackController;
        private PlayerActiveAbilityController _activeAbilityController;

        private HexGrid _currentFightGrid;
        private Fighter _possessedFighter;

        public void Initialize(Fighter possessedFighter, HexGrid fightGrid)
        {
            _currentFightGrid = fightGrid;
            _possessedFighter = possessedFighter;

            _directAttackController = new PlayerDirectAttackController(_possessedFighter, _currentFightGrid, _cellHighlightMaterial, _cellActionableHighlightMaterial, _cellInaccessibleHighlightMaterial);
            _activeAbilityController = new PlayerActiveAbilityController(_possessedFighter, _currentFightGrid, _cellHighlightMaterial, _cellActionableHighlightMaterial, _cellInaccessibleHighlightMaterial);
            _endTurnController = new PlayerEndTurnController(this, _directAttackController, _activeAbilityController);

            BindUIEvents();
        }

        private void BindUIEvents()
        {
            if (_actionPanel == null)
            {
                Debug.LogError("Player controller has no action panel to work with.");
                return;
            }

            _actionPanel.onDirectAttackClicked += _directAttackController.StartTargeting;
            _actionPanel.onActiveAbilityClicked += OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked += _endTurnController.EndTurn;
        }

        private void OnActiveAbilityClicked(ActiveAbilityToAnimation activeAbility)
        {
            _activeAbilityController.StartTargeting(activeAbility);
        }

        public void PlayTurn(Fighter fighterToPlay, Dictionary<Fighter, bool> fighterTeams)
        {
            _endTurnController.PlayTurn(fighterToPlay, fighterTeams, _currentFightGrid);
        }

        private void OnDestroy()
        {
            UnbindUIEvents();
        }

        private void UnbindUIEvents()
        {
            if (_actionPanel == null)
            {
                return;
            }

            _actionPanel.onDirectAttackClicked -= _directAttackController.StartTargeting;
            _actionPanel.onActiveAbilityClicked -= OnActiveAbilityClicked;
            _actionPanel.onEndTurnClicked -= _endTurnController.EndTurn;
        }
    }
}
