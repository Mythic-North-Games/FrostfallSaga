using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core.HeroTeam;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.DataStructures.GraphNode;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityTreeUIController
    {
        private const float DISPLAY_TRANSITION_DURATION = 0.25f;

        #region UXML Element names & classes
        private static readonly string ABILITY_TREE_CONTAINER_UI_NAME = "AbilityTreeContainer";
        private static readonly string BASE_ABILITY_CONTAINER_UI_NAME = "AbilityContainer";
        private static readonly string TREE_ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string EQUIPPED_ABILIIES_CONTAINER_UI_NAME = "EquippedAbilitiesContainer";
        private static readonly string ABILITY_POINTS_CONTAINER_UI_NAME = "AbilityPointsContainer";
        private static readonly string ABILITY_POINTS_LABEL_UI_NAME = "AbilityPointsLabel";
        private static readonly string FIGHTER_NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string FIGHTER_BACKGROUND_ILLUSTRATION_CLASSNAME = "FighterBackgroundIllustration";

        private static readonly string FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME = "fighterNameLabelHidden";
        private static readonly string FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME = "fighterBackgroundIllustrationHidden";
        private static readonly string ABILITY_POINTS_LABEL_HIDDEN_CLASSNAME = "abilityPointsLabelHidden";
        #endregion

        public Action<AbilityContainerUIController> onShowTreeAbilityDetailsClicked;
        public Action<AbilityContainerUIController> onEquipAbilityClicked;

        public Action<EquippedAbilityUIController> onShowEquippedAbilityDetailsClicked;
        public Action<EquippedAbilityUIController> onUnequipAbilityClicked;

        private readonly VisualElement _fighterBackgroundIllustration;
        private readonly VisualElement _abilityPointsContainer;
        private readonly Label _abilityPointsLabel;
        private readonly Label _fighterNameLabel;
        private readonly Dictionary<VisualElement, AbilityContainerUIController> _abilityContainerControllers = new();
        private readonly Dictionary<VisualElement, EquippedAbilityUIController> _equippedAbilityControllers = new();

        private readonly CoroutineRunner _coroutineRunner;

        public AbilityTreeUIController(VisualElement root)
        {
            _abilityPointsContainer = root.Q(ABILITY_POINTS_CONTAINER_UI_NAME);
            _abilityPointsLabel = root.Q<Label>(ABILITY_POINTS_LABEL_UI_NAME);
            _fighterNameLabel = root.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME);
            _fighterBackgroundIllustration = root.Q(FIGHTER_BACKGROUND_ILLUSTRATION_CLASSNAME);
            InitializeAbilityContainerControllers(root.Q(ABILITY_TREE_CONTAINER_UI_NAME));
            InitializeEquippedAbilityControllers(root.Q(EQUIPPED_ABILIIES_CONTAINER_UI_NAME));

            _coroutineRunner = CoroutineRunner.Instance;
        }

        /// <summary>
        /// Update the ability tree panel to set the abilities slots.
        /// </summary>
        /// <param name="hero">The current visualized hero.</param>
        public IEnumerator UpdateAllPanel(Hero hero)
        {
            PersistedFighterConfigurationSO fighterConf = hero.PersistedFighterConfiguration;
            if (fighterConf.FighterClass.AbilitiesGraphRoot == null)
            {
                Debug.LogError("GraphModel for fighterClassSO not defined");
                yield break;
            }

            // Update all panel components
            _coroutineRunner.StartCoroutine(UpdateFighterBackgroundIllustration(hero.EntityConfiguration));
            _coroutineRunner.StartCoroutine(UpdateAbilityPointsLabel(fighterConf.AbilityPoints));
            _coroutineRunner.StartCoroutine(UpdateFighterName(hero.EntityConfiguration.Name));

            HideAbilitiesIcons();
            yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            UpdateAbilityTree(fighterConf);
            UpdateEquippedAbilities(fighterConf.EquippedActiveAbilities.ToArray());   // TODO: What to do for passive abilities?
        }


        public IEnumerator UpdateFighterBackgroundIllustration(EntityConfigurationSO fighterEntityConf)
        {
            if (!_fighterBackgroundIllustration.ClassListContains(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME))
            {
                _fighterBackgroundIllustration.AddToClassList(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            }

            _fighterBackgroundIllustration.style.backgroundImage = new(fighterEntityConf.InventoryBackgroundIllustration);
            yield return new WaitForSeconds(0.1f);
            _fighterBackgroundIllustration.RemoveFromClassList(FIGHTER_BACKGROUND_ILLUSTRATION_HIDDEN_CLASSNAME);
        }

        public void UpdateAbilityTree(PersistedFighterConfigurationSO fighter)
        {
            GraphNode<ABaseAbility> abilitiesGraphRoot = fighter.FighterClass.AbilitiesGraphRoot;
            int currentIndex = 0;
            Queue<GraphNode<ABaseAbility>> queue = new();
            queue.Enqueue(abilitiesGraphRoot);
            VisualElement rootAbilityContainer = FindAbilityContainerByIndex(currentIndex);
            _abilityContainerControllers[rootAbilityContainer].SetAbility(
                abilitiesGraphRoot.Data,
                fighter.GetAbilityState(abilitiesGraphRoot.Data)
            );

            while (queue.Count > 0)
            {
                GraphNode<ABaseAbility> current = queue.Dequeue();
                foreach (GraphNode<ABaseAbility> child in current.Children)
                {
                    currentIndex++;
                    VisualElement childAbilityContainer = FindAbilityContainerByIndex(currentIndex);
                    if (childAbilityContainer == null)
                    {
                        Debug.LogError($"No ability container found for index {currentIndex}");
                        continue;
                    }
                    _abilityContainerControllers[childAbilityContainer].SetAbility(
                        child.Data,
                        fighter.GetAbilityState(child.Data)
                    );
                    queue.Enqueue(child);
                }
            }
        }

        public void UpdateEquippedAbilities(AActiveAbility[] equippedAbilities)
        {
            if (equippedAbilities.Length > _equippedAbilityControllers.Count)
            {
                Debug.LogError($"Equipped abilities can't be more than ability controllers count {equippedAbilities.Length}");
                return;
            }

            for (int i = 0; i < _equippedAbilityControllers.Count; i++)
            {
                if (i >= equippedAbilities.Length)
                {
                    _equippedAbilityControllers.ElementAt(i).Value.SetAbility(null);
                }
                else
                {
                    _equippedAbilityControllers.ElementAt(i).Value.SetAbility(equippedAbilities[i]);
                }
            }
        }

        public IEnumerator UpdateAbilityPointsLabel(int abilityPoints)
        {
            _abilityPointsLabel.AddToClassList(ABILITY_POINTS_LABEL_HIDDEN_CLASSNAME);
            yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            _abilityPointsLabel.text = abilityPoints.ToString();

            yield return new WaitForSeconds(0.1f);
            _abilityPointsLabel.RemoveFromClassList(ABILITY_POINTS_LABEL_HIDDEN_CLASSNAME);
        }

        public IEnumerator UpdateFighterName(string fighterName)
        {
            if (!_fighterNameLabel.ClassListContains(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME))
            {
                _fighterNameLabel.AddToClassList(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME);
                yield return new WaitForSeconds(DISPLAY_TRANSITION_DURATION);
            }

            _fighterNameLabel.text = fighterName;
            yield return new WaitForSeconds(0.1f);
            _fighterNameLabel.RemoveFromClassList(FIGHTER_NAME_LABEL_HIDDEN_CLASSNAME);
        }

        public void PlayUnlockAnimation(ABaseAbility ability)
        {
            foreach (AbilityContainerUIController abilityContainerController in _abilityContainerControllers.Values)
            {
                if (abilityContainerController.CurrentAbility == ability)
                {
                    _coroutineRunner.StartCoroutine(abilityContainerController.PlayUnlockAnimation());
                }
            }
            _coroutineRunner.StartCoroutine(CommonUIAnimations.PlayScaleAnimation(_abilityPointsContainer, new(1.25f, 1.25f), 1f));
        }

        public void PlayEquippedAbilityJumpAnimation(ABaseAbility ability)
        {
            foreach (EquippedAbilityUIController equippedAbilityController in _equippedAbilityControllers.Values)
            {
                if (equippedAbilityController.CurrentAbility == ability)
                {
                    _coroutineRunner.StartCoroutine(equippedAbilityController.PlayJumpAimation());
                }
            }
        }

        private void HideAbilitiesIcons()
        {
            foreach (AbilityContainerUIController abilityContainerController in _abilityContainerControllers.Values)
            {
                abilityContainerController.HideIcon();
            }

            foreach (EquippedAbilityUIController equippedAbilityController in _equippedAbilityControllers.Values)
            {
                equippedAbilityController.HideIcon();
            }
        }

        private VisualElement FindAbilityContainerByIndex(int index)
        {
            string abilityContainerName = $"{BASE_ABILITY_CONTAINER_UI_NAME}{index}";
            return _abilityContainerControllers.Keys.FirstOrDefault(abilityContainer => abilityContainer.name == abilityContainerName);
        }

        private void OnAbilityOnTreeClicked(MouseUpEvent evt)
        {
            VisualElement clickedAbilityContainer = (evt.currentTarget as VisualElement).parent;
            AbilityContainerUIController controller = _abilityContainerControllers[clickedAbilityContainer];

            if (controller.CurrentAbility == null) return;

            if (evt.button == (int)MouseButton.LeftMouse) onShowTreeAbilityDetailsClicked?.Invoke(controller);
            else if (evt.button == (int)MouseButton.RightMouse) onEquipAbilityClicked?.Invoke(controller);
        }

        private void OnEquippedAbilityClicked(MouseUpEvent evt)
        {
            VisualElement clickedAbilityContainer = evt.currentTarget as VisualElement;
            EquippedAbilityUIController controller = _equippedAbilityControllers[clickedAbilityContainer];

            if (controller.CurrentAbility == null) return;

            if (evt.button == (int)MouseButton.LeftMouse) onShowEquippedAbilityDetailsClicked?.Invoke(controller);
            else if (evt.button == (int)MouseButton.RightMouse) onUnequipAbilityClicked?.Invoke(controller);
        }

        private void InitializeAbilityContainerControllers(VisualElement abilityTreeContainer)
        
        {
            foreach (VisualElement abilityContainerRow in abilityTreeContainer.Children())
            {
                foreach (VisualElement abilityContainer in abilityContainerRow.Children())
                {
                    Button abilityButton = abilityContainer.Q<Button>(TREE_ABILITY_BUTTON_UI_NAME);
                    AbilityContainerUIController controller = new(abilityContainer);
                    controller.SetDefautState();
                    abilityButton.RegisterCallback<MouseUpEvent>(OnAbilityOnTreeClicked);
                    _abilityContainerControllers.Add(abilityContainer, controller);
                }
            }
        }

        private void InitializeEquippedAbilityControllers(VisualElement equippedAbilitiesContainer)
        {
            foreach (VisualElement abilityContainer in equippedAbilitiesContainer.Children())
            {
                VisualElement abilityButton = abilityContainer.Children().First();
                EquippedAbilityUIController controller = new(abilityButton);
                controller.SetAbility(null);
                abilityButton.RegisterCallback<MouseUpEvent>(OnEquippedAbilityClicked);
                _equippedAbilityControllers.Add(abilityButton, controller);
            }
        }
    }
}
