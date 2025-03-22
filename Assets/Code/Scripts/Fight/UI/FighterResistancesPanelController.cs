using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterResistancesPanelController
    {
        #region UXML names and classes
        private static readonly string RESISTANCES_STATS_CONTAINER_UI_NAME = "FighterResistancesPanel";

        private static readonly string RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME = "fighterResistancesPanelHidden";
        private static readonly string RESISTANCE_STAT_CONTAINER_ROOT_CLASSNAME = "resistanceStatContainerRoot";
        #endregion

        private readonly VisualTreeAsset _statTemplateContainer;
        private readonly VisualElement _root;
        private readonly VisualElement _resistancesStatsContainer;

        private Fighter _hoveredFighter;

        public FighterResistancesPanelController(VisualElement root, VisualTreeAsset statTemplateContainer)
        {
            _statTemplateContainer = statTemplateContainer;
            _root = root;
            _resistancesStatsContainer = root.Q<VisualElement>(RESISTANCES_STATS_CONTAINER_UI_NAME);
        }

        public void Display(TimelineCharacterUIController hoveredCharacter)
        {
            _hoveredFighter = hoveredCharacter.Fighter;
            SetPosition(hoveredCharacter.Root, _hoveredFighter.GetStatuses().Count);

            UpdateStats(_hoveredFighter);
            _hoveredFighter.onStatusApplied += OnFighterStatusUpdated;
            _hoveredFighter.onStatusRemoved += OnFighterStatusUpdated;

            _root.RemoveFromClassList(RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME);
            _root.SetEnabled(true);
        }

        public void Hide()
        {
            _hoveredFighter.onStatusApplied += OnFighterStatusUpdated;
            _hoveredFighter.onStatusRemoved += OnFighterStatusUpdated;

            _root.AddToClassList(RESISTANCES_STATS_CONTAINER_HIDDEN_CLASSNAME);
            _root.SetEnabled(false);
        }

        private void UpdateStats(Fighter fighter)
        {
            // Clear previous stats
            _resistancesStatsContainer.Clear();

            // Setup new stats
            foreach (KeyValuePair<EMagicalElement, int> magicalResistance in fighter.GetMagicalResistances())
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

        private void SetPosition(VisualElement hoveredCharacter, int hoveredFighterStatusCount)
        {
            _root.style.left = GetResistancePanelLeft(hoveredFighterStatusCount);
            _root.style.top = hoveredCharacter.worldBound.y - hoveredCharacter.worldBound.height + 15;
        }

        private Length GetResistancePanelLeft(int hoveredFighterStatusCount)
        {
            if (hoveredFighterStatusCount == 0) return new Length(30, LengthUnit.Percent);
            if (hoveredFighterStatusCount == 1) return new Length(20, LengthUnit.Percent);
            return new Length(10, LengthUnit.Percent);
        }

        private void OnFighterStatusUpdated(Fighter fighter, AStatus status)
        {
            UpdateStats(fighter);
        }
    }
}