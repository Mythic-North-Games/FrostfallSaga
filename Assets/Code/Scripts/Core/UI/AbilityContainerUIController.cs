using System;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core {
    public class AbilityContainerUIController {
        #region Constantes et noms d'éléments UI
        private static readonly string ABILITY_ICON_UI_NAME = "abilityIcon";
        private static readonly string ABILITY_NAME_UI_NAME = "abilityName";
        private static readonly string ABILITY_COST_UI_NAME = "abilityCost";
        #endregion

        /// <summary>
        /// Événement déclenché lors de la sélection de l'ability.
        /// </summary>
        public event Action<ABaseAbility> OnAbilitySelected;

        /// <summary>
        /// L'élément racine du container (le VisualElement qui contient l'UI de l'ability).
        /// </summary>
        public VisualElement Root {
            get; private set;
        }

        private readonly VisualElement _abilityIcon;
        private readonly Label _abilityName;
        private readonly Label _abilityCost;

        /// <summary>
        /// Propriété pour accéder à l'ability actuellement associée.
        /// </summary>
        public ABaseAbility CurrentAbility => _currentAbility;
        private ABaseAbility _currentAbility;

        /// <summary>
        /// Constructeur qui initialise le container et récupère les sous-éléments par leur nom.
        /// </summary>
        /// <param name="root">L'élément racine correspondant au container de l'ability.</param>
        public AbilityContainerUIController(VisualElement root) {
            Root = root;
            // Enregistre le callback pour détecter le clic
            Root.RegisterCallback<MouseUpEvent>(OnAbilityClicked);
            // Récupère les sous-éléments de l'UI par leurs noms
            _abilityIcon = Root.Q<VisualElement>(ABILITY_ICON_UI_NAME);
            _abilityName = Root.Q<Label>(ABILITY_NAME_UI_NAME);
            _abilityCost = Root.Q<Label>(ABILITY_COST_UI_NAME);
        }

        /// <summary>
        /// Affecte l'affichage visuel de l'ability.
        /// </summary>
        /// <param name="ability">L'ability à afficher.</param>
        public void SetAbilityVisual(ABaseAbility ability) {
            _currentAbility = ability;
            bool abilityIsNull = ability == null;
            // Met à jour l'icône (on utilise null si l'ability est absente)
            _abilityIcon.style.backgroundImage = abilityIsNull ? null : new StyleBackground(ability.IconSprite);
            // Met à jour le nom et le coût (points de déblocage)
            _abilityName.text = abilityIsNull ? string.Empty : ability.Name;
            _abilityCost.text = abilityIsNull ? string.Empty : ability.UnlockPoints.ToString();
        }

        /// <summary>
        /// Gère l'événement de clic sur le container.
        /// Si le clic est effectué avec le bouton gauche et que l'ability est définie, déclenche l'événement OnAbilitySelected.
        /// </summary>
        /// <param name="mouseEvent">L'événement de la souris.</param>
        public void OnAbilityClicked(MouseUpEvent mouseEvent) {
            mouseEvent.StopPropagation();
            if (mouseEvent.button == 0 && _currentAbility != null) {
                OnAbilitySelected?.Invoke(_currentAbility);
            }
        }

        /// <summary>
        /// Active ou désactive le container en modifiant son mode de prise d'événement (picking).
        /// </summary>
        /// <param name="enabled">Vrai pour activer, faux pour désactiver.</param>
        public void SetEnabled(bool enabled) {
            Root.SetEnabled(enabled);
            Root.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            foreach (var child in Root.Children())
                child.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
        }

        /// <summary>
        /// Met à jour le style visuel du container en fonction de l'état (par exemple, "lock", "unlockable", "unlock").
        /// La méthode réinitialise d'abord les classes de style préexistantes avant d'ajouter la classe correspondant à l'état passé en paramètre.
        /// </summary>
        /// <param name="state">L'état visuel à appliquer (ex. "unlock").</param>
        public void SetAbilityStatusStyle(string state) {
            // Réinitialise les classes de style
            Root.RemoveFromClassList("lock");
            Root.RemoveFromClassList("unlockable");
            Root.RemoveFromClassList("unlock");
            // Applique la nouvelle classe
            Root.AddToClassList(state);
        }
    }
}
