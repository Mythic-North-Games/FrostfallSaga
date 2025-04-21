using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.UI;
using System.Collections.Generic;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class BookAbilitySystemMenuUIController  : ABookMenuUIController
    {
        private const string EFFECT_LINE_CLASSNAME = "effectLine";

        [SerializeField] private VisualTreeAsset _abilityTreePanelTemplate;
        [SerializeField] private VisualTreeAsset _abilityDetailsPanelTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private Color _statValueColor = new(9.8f, 9.8f, 9.8f, 1f);

        private AbilityDetailsPanelUIController _abilityDetailsPanelController;
        private AbilityTreeUIController _abilityTreeController;
        private HeroChooserUIController _heroChooserUIController;
        private PersistedFighterConfigurationSO _currentFighter;

        public override void SetupMenu()
        {
            List<Hero> heroes = HeroTeam.Instance.Heroes;
            // Get current fighter
            _currentFighter = heroes[0].PersistedFighterConfiguration;

            // Setup ability tree panel
            VisualElement abilityTreePanelRoot = _abilityTreePanelTemplate.Instantiate();
            abilityTreePanelRoot.StretchToParentSize();
            _abilityTreeController = new AbilityTreeUIController(abilityTreePanelRoot);
            _abilityTreeController.UpdatePanel(_currentFighter);
            _abilityTreeController.onShowAbilityDetailsClicked += OnShowAbilityDetailsClicked;
            _abilityTreeController.onEquipAbilityClicked += OnEquipAbilityClicked;
            _abilityTreeController.onUnequipAbilityClicked += OnUnequipAbilityClicked;

            // Setup ability details panel
            VisualElement abilityDetailsPanelRoot = _abilityDetailsPanelTemplate.Instantiate();
            abilityDetailsPanelRoot.StretchToParentSize();
            _abilityDetailsPanelController = new AbilityDetailsPanelUIController(
                abilityDetailsPanelRoot,
                _statContainerTemplate,
                EFFECT_LINE_CLASSNAME,
                _statValueColor
            );
            _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
            _abilityDetailsPanelController.onUnlockButtonClicked += OnUnlockAbilityButtonClicked;

            // Setup hero chooser 
            _heroChooserUIController = new HeroChooserUIController(abilityTreePanelRoot);
            _heroChooserUIController.SetHeroes(heroes);
            _heroChooserUIController.onHeroChosen += OnHeroChosen;

            _leftPageContainer.Add(abilityTreePanelRoot);
            _rightPageContainer.Add(abilityDetailsPanelRoot);
        }

        private void OnShowAbilityDetailsClicked(ABaseAbility ability)
        {
            if (ability == null)
            {
                _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
                return;
            }

            _abilityDetailsPanelController.UpdatePanel(
                ability,
                _currentFighter.GetAbilityState(ability),
                _currentFighter.AbilityPoints
            );
        }

        private void OnEquipAbilityClicked(ABaseAbility ability)
        {
            if (_currentFighter.GetAbilityState(ability) != EAbilityState.Unlocked)
            {
                return;
            }
            _currentFighter.EquipAbility(ability);
            _abilityTreeController.UpdatePanel(_currentFighter);
        }

        private void OnUnequipAbilityClicked(ABaseAbility ability)
        {
            if (_currentFighter.UnequipAbility(ability))
            {
                _abilityTreeController.UpdatePanel(_currentFighter);
                _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
            }
            else
            {
                Debug.LogError($"Failed to unequip ability {ability.Name}");
            }
        }

        private void OnUnlockAbilityButtonClicked(ABaseAbility ability)
        {
            _currentFighter.UnlockAbility(ability);
            _abilityTreeController.UpdatePanel(_currentFighter);
            _abilityDetailsPanelController.UpdatePanel(
                ability,
                _currentFighter.GetAbilityState(ability),
                _currentFighter.AbilityPoints
            );
        }

        private void OnHeroChosen(Hero hero)
        {
            _currentFighter = hero.PersistedFighterConfiguration;
            _abilityTreeController.UpdatePanel(_currentFighter);
            _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
        }
    }
}
