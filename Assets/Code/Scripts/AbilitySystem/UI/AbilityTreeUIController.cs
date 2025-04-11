using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.Trees;
using System.Linq;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityTreeUIController
    {
        #region UXML Element names & classes
        private static readonly string ABILITY_TREE_CONTAINER_UI_NAME = "AbilityTreeContainer";
        private static readonly string BASE_ABILITY_CONTAINER_UI_NAME = "AbilityContainer";

        #endregion

        private readonly VisualElement _root;
        private readonly VisualElement _abilityTreeContainer;

        private GraphNode<ABaseAbility> _abilitiesGraphModel;
        private Dictionary<VisualElement, AbilityContainerUIController> _abilityContainerControllers = new();

        public AbilityTreeUIController(VisualElement root)
        {
            _root = root;
            _abilityTreeContainer = root.Q(ABILITY_TREE_CONTAINER_UI_NAME);
            InitializeAbilityContainerControllers();
        }

        /// <summary>
        /// Update the ability tree panel to set the abilities slots.
        /// </summary>
        /// <param name="fighter">The current visualized fighter.</param>
        public void UpdatePanel(PersistedFighterConfigurationSO fighter)
        {
            _abilitiesGraphModel = fighter.FighterClass.AbilitiesGraphModel;
            if (_abilitiesGraphModel == null)
            {
                Debug.LogError("GraphModel for fighterClassSO not defined");
                return;
            }
            UpdateAbilityContainers(_abilitiesGraphModel);
        }

        private void UpdateAbilityContainers(GraphNode<ABaseAbility> abilitiesGraphRoot)
        {
            int currentIndex = 0;
            Queue<GraphNode<ABaseAbility>> queue = new();
            queue.Enqueue(abilitiesGraphRoot);
            VisualElement rootAbilityContainer = FindAbilityContainerByIndex(currentIndex);
            _abilityContainerControllers[rootAbilityContainer].SetAbility(abilitiesGraphRoot.Data, EAbilityState.Unlocked);

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
                    _abilityContainerControllers[childAbilityContainer].SetAbility(child.Data, EAbilityState.Unlocked);
                    queue.Enqueue(child);
                }
            }
        }

        private VisualElement FindAbilityContainerByIndex(int index)
        {
            string abilityContainerName = $"{BASE_ABILITY_CONTAINER_UI_NAME}{index}";
            return _abilityContainerControllers.Keys.FirstOrDefault(abilityContainer => abilityContainer.name == abilityContainerName);
        }

        private void InitializeAbilityContainerControllers()
        {
            foreach (VisualElement abilityContainerRow in _abilityTreeContainer.Children())
            {
                foreach (VisualElement abilityContainer in abilityContainerRow.Children())
                {
                    AbilityContainerUIController controller = new(abilityContainer);
                    controller.SetDefautState();
                    _abilityContainerControllers.Add(abilityContainer, controller);
                }
            }
        }
    }
}
