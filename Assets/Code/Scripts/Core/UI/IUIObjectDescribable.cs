using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Core.UI
{
    public interface IUIObjectDescribable
    {
        public Sprite GetIcon();
        public string GetName();
        public string GetDescription();
        public Dictionary<Sprite, string> GetStatsUIData();
        public string GetPrimaryEffectsTitle();
        public List<string> GetPrimaryEffectsUIData();
        public string GetSecondaryEffectsTitle();
        public List<string> GetSecondaryEffectsUIData();
    }
}