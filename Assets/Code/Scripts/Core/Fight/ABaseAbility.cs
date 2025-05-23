using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    /// <summary>
    ///     Abilities common fields.
    /// </summary>
    public abstract class ABaseAbility : ScriptableObject, IUIObjectDescribable
    {
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public Sprite Icon { get; protected set; }
        [field: SerializeField, Range(0, 99)] public int UnlockPoints { get; protected set; }

        #region IUIObjectDescribable interface implementation
        
        public Sprite GetIcon()
        {
            return Icon;
        }

        public string GetName()
        {
            return Name;
        }

        public string GetDescription()
        {
            return Description;
        }

        public abstract Dictionary<Sprite, string> GetStatsUIData();

        public string GetPrimaryEffectsTitle()
        {
            return "Effects";
        }

        public abstract List<string> GetPrimaryEffectsUIData();

        public string GetSecondaryEffectsTitle()
        {
            return "Masterstroke Effects";
        }

        public abstract List<string> GetSecondaryEffectsUIData();

        #endregion
    }
}