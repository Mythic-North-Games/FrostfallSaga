using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;

namespace FrostfallSaga.UI {
    public class BookAbilitySystemMenuUIController {
        private readonly VisualElement _root;

        private readonly AbilityVisualizerUIController _abilityVisualizerController;
        private readonly AbilityTreeUIController _abilityTreeController;

        private PersistedFighterConfigurationSO _currentFighter;

        public BookAbilitySystemMenuUIController(
            VisualElement root,
            AbilityVisualizerUIController visualizerController,
            AbilityTreeUIController treeController
        ) {
            _root = root;
            _abilityVisualizerController = visualizerController;
            _abilityTreeController = treeController;
        }

        public void SetFighter(PersistedFighterConfigurationSO fighter) {
            _currentFighter = fighter;

            if (_currentFighter == null || _currentFighter.FighterClass == null) {
                Debug.LogWarning("Fighter ou FighterClass est nul.");
                return;
            }

            // Met à jour la visualisation et l’arbre
           // _abilityVisualizerController.SetFighter(_currentFighter);
            _abilityTreeController.SetFighter(_currentFighter);
        }
    }
}
