using System;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Core {
    public class AbilityContainerUIController {
        #region Constantes et noms d'�l�ments UI
        private static readonly string ABILITY_ICON_UI_NAME = "abilityIcon";
        private static readonly string ABILITY_NAME_UI_NAME = "abilityName";
        private static readonly string ABILITY_COST_UI_NAME = "abilityCost";
        #endregion

        /// <summary>
        /// �v�nement d�clench� lors de la s�lection de l'ability.
        /// </summary>
        public event Action<ABaseAbility> OnAbilitySelected;

        /// <summary>
        /// L'�l�ment racine du container (le VisualElement qui contient l'UI de l'ability).
        /// </summary>
        public VisualElement Root {
            get; private set;
        }

        private readonly VisualElement _abilityIcon;
        private readonly Label _abilityName;
        private readonly Label _abilityCost;

        /// <summary>
        /// Propri�t� pour acc�der � l'ability actuellement associ�e.
        /// </summary>
        public ABaseAbility CurrentAbility => _currentAbility;
        private ABaseAbility _currentAbility;

        /// <summary>
        /// Constructeur qui initialise le container et r�cup�re les sous-�l�ments par leur nom.
        /// </summary>
        /// <param name="root">L'�l�ment racine correspondant au container de l'ability.</param>
        public AbilityContainerUIController(VisualElement root) {
            Root = root;
            // Enregistre le callback pour d�tecter le clic
            Root.RegisterCallback<MouseUpEvent>(OnAbilityClicked);
            // R�cup�re les sous-�l�ments de l'UI par leurs noms
            _abilityIcon = Root.Q<VisualElement>(ABILITY_ICON_UI_NAME);
            _abilityName = Root.Q<Label>(ABILITY_NAME_UI_NAME);
            _abilityCost = Root.Q<Label>(ABILITY_COST_UI_NAME);
        }

        /// <summary>
        /// Affecte l'affichage visuel de l'ability.
        /// </summary>
        /// <param name="ability">L'ability � afficher.</param>
        public void SetAbilityVisual(ABaseAbility ability) {
            _currentAbility = ability;
            bool abilityIsNull = ability == null;
            // Met � jour l'ic�ne (on utilise null si l'ability est absente)
            _abilityIcon.style.backgroundImage = abilityIsNull ? null : new StyleBackground(ability.IconSprite);
            // Met � jour le nom et le co�t (points de d�blocage)
            _abilityName.text = abilityIsNull ? string.Empty : ability.Name;
            _abilityCost.text = abilityIsNull ? string.Empty : ability.UnlockPoints.ToString();
        }

        /// <summary>
        /// G�re l'�v�nement de clic sur le container.
        /// Si le clic est effectu� avec le bouton gauche et que l'ability est d�finie, d�clenche l'�v�nement OnAbilitySelected.
        /// </summary>
        /// <param name="mouseEvent">L'�v�nement de la souris.</param>
        public void OnAbilityClicked(MouseUpEvent mouseEvent) {
            mouseEvent.StopPropagation();
            if (mouseEvent.button == 0 && _currentAbility != null) {
                OnAbilitySelected?.Invoke(_currentAbility);
            }
        }

        /// <summary>
        /// Active ou d�sactive le container en modifiant son mode de prise d'�v�nement (picking).
        /// </summary>
        /// <param name="enabled">Vrai pour activer, faux pour d�sactiver.</param>
        public void SetEnabled(bool enabled) {
            Root.SetEnabled(enabled);
            Root.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
            foreach (var child in Root.Children())
                child.pickingMode = enabled ? PickingMode.Position : PickingMode.Ignore;
        }

        /// <summary>
        /// Met � jour le style visuel du container en fonction de l'�tat (par exemple, "lock", "unlockable", "unlock").
        /// La m�thode r�initialise d'abord les classes de style pr�existantes avant d'ajouter la classe correspondant � l'�tat pass� en param�tre.
        /// </summary>
        /// <param name="state">L'�tat visuel � appliquer (ex. "unlock").</param>
        public void SetAbilityStatusStyle(string state) {
            // R�initialise les classes de style
            Root.RemoveFromClassList("lock");
            Root.RemoveFromClassList("unlockable");
            Root.RemoveFromClassList("unlock");
            // Applique la nouvelle classe
            Root.AddToClassList(state);
        }
    }
}
