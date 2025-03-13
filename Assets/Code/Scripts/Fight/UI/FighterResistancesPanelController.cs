using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterResistancesPanelController
    {
        #region UXML names and classes
        private static readonly string RESISTANCES_STATS_CONTAINER_UI_NAME = "FighterResistancesPanel";

        private static readonly string RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME = "fighterResistancesPanelRootHidden";
        private static readonly string RESISTANCE_STAT_CONTAINER_ROOT_CLASSNAME = "resistanceStatContainerRoot";
        #endregion

        private readonly VisualTreeAsset _statTemplateContainer;
        private readonly VisualElement _root;
        private readonly VisualElement _resistancesStatsContainer;
        private readonly Fighter _fighter;

        public FighterResistancesPanelController(VisualElement root, Fighter fighter, VisualTreeAsset statTemplateContainer)
        {
            _statTemplateContainer = statTemplateContainer;
            _root = root;
            _resistancesStatsContainer = root.Q<VisualElement>(RESISTANCES_STATS_CONTAINER_UI_NAME);
            _fighter = fighter;
        }

        public void Display()
        {
            _root.RemoveFromClassList(RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME);
            _root.SetEnabled(true);
        }

        public void Hide()
        {
            _root.AddToClassList(RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME);
            _root.SetEnabled(false);
        }

        public void UpdateStats()
        {
            // Clear previous stats
            _resistancesStatsContainer.Clear();

            // Setup new stats
            foreach (KeyValuePair<EMagicalElement, int> magicalResistance in _fighter.GetMagicalResistances())
            {
                VisualElement statContainerRoot = _statTemplateContainer.Instantiate();
                statContainerRoot.AddToClassList(RESISTANCE_STAT_CONTAINER_ROOT_CLASSNAME);

                Sprite magicalStatIcon = UIIconsProvider.Instance.GetIcon(magicalResistance.Key.GetIconResourceName());
                StatContainerUIController.SetupStatContainer(
                    statContainerRoot, magicalStatIcon, magicalResistance.Value.ToString()
                );

                _resistancesStatsContainer.Add(statContainerRoot);
            }
        }
    }
}