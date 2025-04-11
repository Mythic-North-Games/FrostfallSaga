using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils;  // Pour GraphNode<T>
using FrostfallSaga.Core;

namespace FrostfallSaga {
    public class AbilityTreeUIController {

        private readonly VisualElement _root;
        private PersistedFighterConfigurationSO _fighter;
        private GraphNode<ABaseAbility> _abilitiesGraphModel;
        private Dictionary<ABaseAbility, AbilityContainerUIController> _abilityControllers;

        public AbilityTreeUIController(VisualElement root) {
            _root = root;
        }

        /// <summary>
        /// Configure le controller avec la configuration du fighter et affiche le graphe d'abilities dans l'UI.
        /// </summary>
        /// <param name="fighter">La configuration persistée incluant le graphe d'abilities.</param>
        public void SetFighter(PersistedFighterConfigurationSO fighter) {
            _fighter = fighter;
            // Récupère le graphe d'abilities depuis FighterClassSO.
            _abilitiesGraphModel = fighter.FighterClass.AbilitiesGraphModel;
            if (_abilitiesGraphModel == null) {
                Debug.LogError("GraphModel for fighterClassSO not defined");
                return;
            }
            _root.Clear();
            AssociateAbilitiesWithContainersBFS();
        }

        /// <summary>
        /// Parcours le graphe d'abilities en largeur (BFS) et associe chaque node à son container UI,
        /// en s'assurant que chaque node est traité une seule fois.
        /// La correspondance se fait par un index : "abilityContainer_" + index.
        /// </summary>
        private void AssociateAbilitiesWithContainersBFS() {
            Queue<GraphNode<ABaseAbility>> queue = new Queue<GraphNode<ABaseAbility>>();
            HashSet<GraphNode<ABaseAbility>> visited = new HashSet<GraphNode<ABaseAbility>>();
            int currentIndex = 0;

            queue.Enqueue(_abilitiesGraphModel);
            visited.Add(_abilitiesGraphModel);

            while (queue.Count > 0) {
                GraphNode<ABaseAbility> currentNode = queue.Dequeue();

                // Recherche du container UI en utilisant l'index courant.
                VisualElement abilityElement = FindAbilityContainer(currentIndex);
                currentIndex++;

                if (abilityElement != null) {
                    var abilityUIController = new AbilityContainerUIController(abilityElement);
                    abilityUIController.SetAbilityVisual(currentNode.Data);
                    _abilityControllers.Add(currentNode.Data, abilityUIController);
                    // Attache l'événement pour la sélection.
                    abilityUIController.OnAbilitySelected += OnAbilitySelected;
                } else {
                    Debug.LogWarning($"Aucun container trouvé pour l'index {currentIndex - 1}");
                }

                // Enfile tous les enfants non encore visités.
                foreach (var child in currentNode.Children) {
                    if (!visited.Contains(child)) {
                        visited.Add(child);
                        queue.Enqueue(child);
                    }
                }
            }
        }

        /// <summary>
        /// Recherche dans l'UI le VisualElement dont le nom correspond au container recherché.
        /// Le nom est construit avec la convention "abilityContainer_" + index.
        /// </summary>
        /// <param name="index">L'index du container à rechercher.</param>
        /// <returns>Le VisualElement correspondant ou null s'il n'est pas trouvé.</returns>
        private VisualElement FindAbilityContainer(int index) {
            return _root.Q<VisualElement>("abilityContainer_" + index);
        }

        /// <summary>
        /// Gestion de l'événement lorsqu'une ability est sélectionnée dans l'UI.
        /// Si l'ability peut être débloquée, on la marque comme déverrouillée et on met à jour l'UI.
        /// </summary>
        /// <param name="ability">L'ability sélectionnée.</param>
        private void OnAbilitySelected(ABaseAbility ability) {
                //TODO : IsUnlockable(), IsLocked()
                //UpdateUIAfterUnlock(ability);
        }

        /// <summary>
        /// Met à jour l'UI pour refléter que l'ability vient d'être débloquée.
        /// Parcourt les containers pour trouver celui qui correspond à l'ability et change son style.
        /// </summary>
        /// <param name="unlockedAbility">L'ability qui vient d'être débloquée.</param>
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
