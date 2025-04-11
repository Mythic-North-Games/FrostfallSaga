using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils;  // Pour GraphNode<T>
using FrostfallSaga.Core;

namespace FrostfallSaga.AbilitySystem.UI
{
    public class AbilityTreeUIController
    {
        private readonly VisualElement _root;
        private GraphNode<ABaseAbility> _abilitiesGraphModel;
        private Dictionary<ABaseAbility, AbilityContainerUIController> _abilityControllers;

        public AbilityTreeUIController(VisualElement root)
        {
            _root = root;
        }

        /// <summary>
        /// Update the ability tree panel to set the abilities slots.
        /// </summary>
        /// <param name="fighter">The current visualized fighter.</param>
        public void UpdatePanel(PersistedFighterConfigurationSO fighter)
        {
            // R�cup�re le graphe d'abilities depuis FighterClassSO.
            _abilitiesGraphModel = fighter.FighterClass.AbilitiesGraphModel;
            if (_abilitiesGraphModel == null)
            {
                Debug.LogError("GraphModel for fighterClassSO not defined");
                return;
            }
            _root.Clear();
            AssociateAbilitiesWithContainersBFS();
        }

        /// <summary>
        /// Parcours le graphe d'abilities en largeur (BFS) et associe chaque node � son container UI,
        /// en s'assurant que chaque node est trait� une seule fois.
        /// La correspondance se fait par un index : "abilityContainer_" + index.
        /// </summary>
        private void AssociateAbilitiesWithContainersBFS()
        {
            Queue<GraphNode<ABaseAbility>> queue = new Queue<GraphNode<ABaseAbility>>();
            HashSet<GraphNode<ABaseAbility>> visited = new HashSet<GraphNode<ABaseAbility>>();
            int currentIndex = 0;

            queue.Enqueue(_abilitiesGraphModel);
            visited.Add(_abilitiesGraphModel);

            while (queue.Count > 0)
            {
                GraphNode<ABaseAbility> currentNode = queue.Dequeue();

                // Recherche du container UI en utilisant l'index courant.
                VisualElement abilityElement = FindAbilityContainer(currentIndex);
                currentIndex++;

                if (abilityElement != null)
                {
                    var abilityUIController = new AbilityContainerUIController(abilityElement);
                    abilityUIController.SetAbilityVisual(currentNode.Data);
                    _abilityControllers.Add(currentNode.Data, abilityUIController);
                    // Attache l'�v�nement pour la s�lection.
                    abilityUIController.OnAbilitySelected += OnAbilitySelected;
                }
                else
                {
                    Debug.LogWarning($"Aucun container trouv� pour l'index {currentIndex - 1}");
                }

                // Enfile tous les enfants non encore visit�s.
                foreach (var child in currentNode.Children)
                {
                    if (!visited.Contains(child))
                    {
                        visited.Add(child);
                        queue.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Recherche dans l'UI le VisualElement dont le nom correspond au container recherch�.
        /// Le nom est construit avec la convention "abilityContainer_" + index.
        /// </summary>
        /// <param name="index">L'index du container � rechercher.</param>
        /// <returns>Le VisualElement correspondant ou null s'il n'est pas trouv�.</returns>
        private VisualElement FindAbilityContainer(int index)
        {
            return _root.Q<VisualElement>("abilityContainer_" + index);
        }

        /// <summary>
        /// Gestion de l'�v�nement lorsqu'une ability est s�lectionn�e dans l'UI.
        /// Si l'ability peut �tre d�bloqu�e, on la marque comme d�verrouill�e et on met � jour l'UI.
        /// </summary>
        /// <param name="ability">L'ability s�lectionn�e.</param>
        private void OnAbilitySelected(ABaseAbility ability)
        {
            //TODO : IsUnlockable(), IsLocked()
            //UpdateUIAfterUnlock(ability);
        }

        /// <summary>
        /// Met � jour l'UI pour refl�ter que l'ability vient d'�tre d�bloqu�e.
        /// Parcourt les containers pour trouver celui qui correspond � l'ability et change son style.
        /// </summary>
        /// <param name="unlockedAbility">L'ability qui vient d'�tre d�bloqu�e.</param>
        /*private void UpdateUIAfterUnlock(ABaseAbility unlockedAbility) {
            foreach (VisualElement childElement in _root.Children()) {
                var abilityUI = childElement.Q<AbilityContainerUIController>();
                if (abilityUI != null && abilityUI.CurrentAbility == unlockedAbility) {
                    abilityUI.SetAbilityStatusStyle("unlock");
                    break;
                }
            }
        }*/
    }
}
