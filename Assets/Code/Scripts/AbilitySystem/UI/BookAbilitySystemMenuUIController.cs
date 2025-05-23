using System.Collections.Generic;
using FrostfallSaga.Core.BookMenu;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class BookAbilitySystemMenuUIController  : ABookMenuUIController
    {
        private const string EFFECT_LINE_CLASSNAME = "effectLine";

        [SerializeField] private VisualTreeAsset _abilityTreePanelTemplate;
        [SerializeField] private VisualTreeAsset _abilityDetailsPanelTemplate;
        [SerializeField] private VisualTreeAsset _statContainerTemplate;
        [SerializeField] private Color _statValueColor = new(9.8f, 9.8f, 9.8f, 1f);
        [SerializeField] private Color _statIconTintColor;

        private AbilityDetailsPanelUIController _abilityDetailsPanelController;
        private AbilityTreeUIController _abilityTreeController;
        private HeroChooserUIController _heroChooserUIController;
        private Hero _currentHero;
        private PersistedFighterConfigurationSO _currentFighterConf;

        private AbilityContainerUIController _currentShownTreeAbilityUIController;
        private EquippedAbilityUIController _currentShownEquippedAbilityUIController;

        private CoroutineRunner _coroutineRunner;

        protected override void Awake()
        {
            base.Awake();
            _coroutineRunner = CoroutineRunner.Instance;
        }

        public override void SetupMenu()
        {
            List<Hero> heroes = HeroTeam.Instance.Heroes;
            // Get current fighter
            _currentHero = heroes[0];
            _currentFighterConf = _currentHero.PersistedFighterConfiguration;

            // Setup ability tree panel
            VisualElement abilityTreePanelRoot = _abilityTreePanelTemplate.Instantiate();
            abilityTreePanelRoot.StretchToParentSize();
            _abilityTreeController = new AbilityTreeUIController(abilityTreePanelRoot);
            _coroutineRunner.StartCoroutine(_abilityTreeController.UpdateAllPanel(_currentHero));
            _abilityTreeController.onShowTreeAbilityDetailsClicked += OnShowTreeAbilityDetailsClicked;
            _abilityTreeController.onShowEquippedAbilityDetailsClicked += OnShowEquippedAbilityDetailsClicked;
            _abilityTreeController.onEquipAbilityClicked += OnEquipAbilityClicked;
            _abilityTreeController.onUnequipAbilityClicked += OnUnequipAbilityClicked;

            // Setup ability details panel
            VisualElement abilityDetailsPanelRoot = _abilityDetailsPanelTemplate.Instantiate();
            abilityDetailsPanelRoot.StretchToParentSize();
            _abilityDetailsPanelController = new AbilityDetailsPanelUIController(
                abilityDetailsPanelRoot,
                _statContainerTemplate,
                EFFECT_LINE_CLASSNAME,
                _statValueColor,
                _statIconTintColor
            );
            _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
            _abilityDetailsPanelController.onUnlockButtonClicked += OnUnlockAbilityButtonClicked;

            // Setup hero chooser 
            _heroChooserUIController = new HeroChooserUIController(abilityTreePanelRoot);
            _heroChooserUIController.SetHeroes(heroes);
            _heroChooserUIController.ActivateHero(heroes[0]);
            _heroChooserUIController.onHeroChosen += OnHeroChosen;

            _leftPageContainer.Add(abilityTreePanelRoot);
            _rightPageContainer.Add(abilityDetailsPanelRoot);
        }

        private void OnShowTreeAbilityDetailsClicked(AbilityContainerUIController abilityContainerUIController)
        {
            _currentShownTreeAbilityUIController?.SetActive(false);
            _currentShownEquippedAbilityUIController?.SetActive(false);

            UpdateAbilityDetailsPanel(abilityContainerUIController.CurrentAbility);

            _currentShownTreeAbilityUIController = abilityContainerUIController;
            _currentShownTreeAbilityUIController.SetActive(true);
        }

        private void OnShowEquippedAbilityDetailsClicked(EquippedAbilityUIController equippedAbilityUIController)
        {
            _currentShownTreeAbilityUIController?.SetActive(false);
            _currentShownEquippedAbilityUIController?.SetActive(false);

            UpdateAbilityDetailsPanel(equippedAbilityUIController.CurrentAbility);
            
            _currentShownEquippedAbilityUIController = equippedAbilityUIController;
            _currentShownEquippedAbilityUIController.SetActive(true);
        }

        private void UpdateAbilityDetailsPanel(ABaseAbility ability)
        {
            if (ability == null)
            {
                _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);
                return;
            }

            _abilityDetailsPanelController.UpdatePanel(
                ability,
                _currentFighterConf.GetAbilityState(ability),
                _currentFighterConf.AbilityPoints
            );
        }

        private void OnEquipAbilityClicked(AbilityContainerUIController abilityContainer)
        {
            bool abilityNotUnlocked = _currentFighterConf.GetAbilityState(abilityContainer.CurrentAbility) != EAbilityState.Unlocked;

            if (abilityNotUnlocked)
            {
                _coroutineRunner.StartCoroutine(CommonUIAnimations.PlayShakeAnimation(abilityContainer.Root));
                return;
            }

            if (!_currentFighterConf.IsAbilityEquipped(abilityContainer.CurrentAbility))
            {
                _currentFighterConf.EquipAbility(abilityContainer.CurrentAbility);
                _abilityTreeController.UpdateEquippedAbilities(
                    _currentHero.PersistedFighterConfiguration.EquippedActiveAbilities.ToArray()
                );
            }
            else
            {
                _coroutineRunner.StartCoroutine(CommonUIAnimations.PlayShakeAnimation(abilityContainer.Root));
            }

            _abilityTreeController.PlayEquippedAbilityJumpAnimation(
                abilityContainer.CurrentAbility
            );
        }

        private void OnUnequipAbilityClicked(EquippedAbilityUIController equippedAbilityUIController)
        {
            if (_currentFighterConf.UnequipAbility(equippedAbilityUIController.CurrentAbility))
            {
                _abilityTreeController.UpdateEquippedAbilities(
                    _currentHero.PersistedFighterConfiguration.EquippedActiveAbilities.ToArray()
                );
            }
            else
            {
                Debug.LogError($"Failed to unequip ability {equippedAbilityUIController.CurrentAbility.Name}");
            }
        }

        private void OnUnlockAbilityButtonClicked(ABaseAbility ability)
        {
            _currentFighterConf.UnlockAbility(ability);
            _coroutineRunner.StartCoroutine(_abilityTreeController.UpdateAbilityPointsLabel(_currentFighterConf.AbilityPoints));
            _abilityTreeController.UpdateAbilityTree(_currentFighterConf);
            _abilityTreeController.PlayUnlockAnimation(ability);
            _abilityDetailsPanelController.UpdatePanel(
                ability,
                _currentFighterConf.GetAbilityState(ability),
                _currentFighterConf.AbilityPoints
            );
        }

        private void OnHeroChosen(Hero hero)
        {
            _currentHero = hero;
            _currentFighterConf = hero.PersistedFighterConfiguration;

            _coroutineRunner.StartCoroutine(_abilityTreeController.UpdateAllPanel(_currentHero));
            _abilityDetailsPanelController.UpdatePanel(null, EAbilityState.Locked, -1);

            _currentShownTreeAbilityUIController?.SetActive(false);
            _currentShownEquippedAbilityUIController?.SetActive(false);
        }
    }
}
