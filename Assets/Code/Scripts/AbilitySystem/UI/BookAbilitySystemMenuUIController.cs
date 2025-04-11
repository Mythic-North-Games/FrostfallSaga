using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.HeroTeam;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class BookAbilitySystemMenuUIController  : ABookMenuUIController
    {
        [SerializeField] private VisualTreeAsset _abilityTreePanelTemplate;

        private AbilityVisualizerUIController _abilityVisualizerController;
        private AbilityTreeUIController _abilityTreeController;

        private PersistedFighterConfigurationSO _currentFighter;

        public override void SetupMenu()
        {
            // Setup ability tree panel
            VisualElement abilityTreePanelRoot = _abilityTreePanelTemplate.Instantiate();
            abilityTreePanelRoot.StretchToParentSize();
            //_abilityTreeController = new AbilityTreeUIController(abilityTreePanelRoot);

            //SetFighter(HeroTeam.Instance.Heroes[0].PersistedFighterConfiguration);

            _leftPageContainer.Add(abilityTreePanelRoot);
        }

        public void SetFighter(PersistedFighterConfigurationSO fighter)
        {
            _currentFighter = fighter;

            if (_currentFighter == null || _currentFighter.FighterClass == null)
            {
                Debug.LogError("Fighter ou FighterClass est nul.");
                return;
            }

            // Met � jour la visualisation et l�arbre
            // _abilityVisualizerController.SetFighter(_currentFighter);
            //_abilityTreeController.UpdatePanel(_currentFighter);
        }
    }
}
