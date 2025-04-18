using System;
using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.Trees;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityTreeUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_TREE_CONTAINER_UI_NAME = "AbilityTreeContainer";
        private static readonly string BASE_ABILITY_CONTAINER_UI_NAME = "AbilityContainer";
        private static readonly string TREE_ABILITY_BUTTON_UI_NAME = "AbilityButton";
        private static readonly string EQUIPPED_ABILIIES_CONTAINER_UI_NAME = "EquippedAbilitiesContainer";
        private static readonly string ABILITY_POINTS_LABEL_UI_NAME = "AbilityPointsLabel";
        #endregion

        public Action<ABaseAbility> onShowAbilityDetailsClicked;
        public Action<ABaseAbility> onEquipAbilityClicked;
        public Action<ABaseAbility> onUnequipAbilityClicked;

        private readonly Label _abilityPointsLabel;

        private readonly Dictionary<VisualElement, AbilityContainerUIController> _abilityContainerControllers = new();
        private readonly Dictionary<VisualElement, EquippedAbilityUIController> _equippedAbilityControllers = new();

        public AbilityTreeUIController(VisualElement root)
        {
            InitializeAbilityContainerControllers(root.Q(ABILITY_TREE_CONTAINER_UI_NAME));
            InitializeEquippedAbilityControllers(root.Q(EQUIPPED_ABILIIES_CONTAINER_UI_NAME));
            _abilityPointsLabel = root.Q<Label>(ABILITY_POINTS_LABEL_UI_NAME);
        }

        /// <summary>
        /// Update the ability tree panel to set the abilities slots.
        /// </summary>
        /// <param name="fighter">The current visualized fighter.</param>
        public void UpdatePanel(PersistedFighterConfigurationSO fighter)
        {
            if (fighter.FighterClass.AbilitiesGraphModel == null)
            {
                Debug.LogError("GraphModel for fighterClassSO not defined");
                return;
            }

            UpdateAbilityContainers(fighter);
            UpdateEquippedAbilities(fighter.EquippedActiveAbilities.ToArray());   // TODO: What to do for passive abilities?
            _abilityPointsLabel.text = fighter.AbilityPoints.ToString();
        }

        private void UpdateAbilityContainers(PersistedFighterConfigurationSO fighter)
        {
            GraphNode<ABaseAbility> abilitiesGraphRoot = fighter.FighterClass.AbilitiesGraphModel;
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

        private void UpdateEquippedAbilities(AActiveAbility[] equippedAbilities)
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

            if (evt.button == (int)MouseButton.LeftMouse) onShowAbilityDetailsClicked?.Invoke(controller.CurrentAbility);
            else if (evt.button == (int)MouseButton.RightMouse) onEquipAbilityClicked?.Invoke(controller.CurrentAbility);
        }

        private void OnEquippedAbilityClicked(MouseUpEvent evt)
        {
            VisualElement clickedAbilityContainer = evt.currentTarget as VisualElement;
            EquippedAbilityUIController controller = _equippedAbilityControllers[clickedAbilityContainer];

            if (controller.CurrentAbility == null) return;

            if (evt.button == (int)MouseButton.LeftMouse) onShowAbilityDetailsClicked?.Invoke(controller.CurrentAbility);
            else if (evt.button == (int)MouseButton.RightMouse) onUnequipAbilityClicked?.Invoke(controller.CurrentAbility);
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
                EquippedAbilityUIController controller = new(abilityContainer);
                controller.SetAbility(null);
                abilityContainer.RegisterCallback<MouseUpEvent>(OnEquippedAbilityClicked);
                _equippedAbilityControllers.Add(abilityContainer, controller);
            }
        }
    }
}
