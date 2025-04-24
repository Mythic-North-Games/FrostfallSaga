using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.InventorySystem.UI
{
    public class ObjectDetailsUIController
    {
        private readonly Label _descriptionLabel;
        private readonly string _effectLineClassname;
        private readonly VisualElement _icon;
        private readonly Label _nameLabel;
        private readonly VisualElement _primaryEffectsListContainer;
        private readonly Label _primaryEffectsTitleLabel;

        private readonly VisualElement _root;
        private readonly VisualElement _secondaryEffectsListContainer;
        private readonly Label _secondaryEffectsTitleLabel;

        private readonly VisualTreeAsset _statContainerTemplate;
        private readonly VisualElement _statsContainer;
        private readonly Color _statValueColor;
        private readonly Color _statIconTintColor;

        public ObjectDetailsUIController(
            VisualElement root,
            VisualTreeAsset statContainerTemplate,
            string effectLineClassname,
            Color statValueColor = default,
            Color statIconTintColor = default
        )
        {
            _root = root;
            _statContainerTemplate = statContainerTemplate;
            _effectLineClassname = effectLineClassname;
            _statValueColor = statValueColor;
            _statIconTintColor = statIconTintColor;

            _icon = _root.Q<VisualElement>(ICON_UI_NAME);
            _nameLabel = _root.Q<Label>(NAME_LABEL_UI_NAME);
            _descriptionLabel = _root.Q<Label>(DESCRIPTION_LABEL_UI_NAME);
            _statsContainer = _root.Q<VisualElement>(STATS_CONTAINER_UI_NAME);
            _primaryEffectsTitleLabel = _root.Q<Label>(PRIMARY_EFFECTS_LIST_TITLE_UI_NAME);
            _primaryEffectsListContainer = _root.Q<VisualElement>(PRIMARY_EFFECTS_LIST_CONTAINER_UI_NAME);
            _secondaryEffectsTitleLabel = _root.Q<Label>(SECONDARY_EFFECTS_LIST_TITLE_UI_NAME);
            _secondaryEffectsListContainer = _root.Q<VisualElement>(SECONDARY_EFFECTS_LIST_CONTAINER_UI_NAME);

            _statsContainer.style.display = DisplayStyle.None;
            _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;
            _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;
        }

        public void Setup(
            Sprite icon,
            string name,
            string description,
            Dictionary<Sprite, string> stats = null,
            string primaryEffectsTitle = null,
            List<string> primaryEffects = null,
            string secondaryEffectsTitle = null,
            List<string> secondaryEffects = null
        )
        {
            SetupHeader(icon, name, description);

            if (stats != null && stats.Count > 0)
            {
                _statsContainer.style.display = DisplayStyle.Flex;
                SetupStats(stats);
            }
            else _statsContainer.style.display = DisplayStyle.None;

            if (primaryEffects != null && primaryEffects.Count > 0)
            {
                _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.Flex;
                SetupEffectsList(_primaryEffectsTitleLabel, _primaryEffectsListContainer, primaryEffectsTitle,
                    primaryEffects);
            }
            else _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;

            if (secondaryEffects != null && secondaryEffects.Count > 0)
            {
                _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.Flex;
                SetupEffectsList(_secondaryEffectsTitleLabel, _secondaryEffectsListContainer, secondaryEffectsTitle,
                    secondaryEffects);
            }
            else _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;
        }

        private void SetupHeader(Sprite icon, string name, string description)
        {
            _icon.style.backgroundImage = new(icon);
            _nameLabel.text = name;
            _descriptionLabel.text = description;
        }

        private void SetupStats(Dictionary<Sprite, string> stats)
        {
            // Clear previous stats
            _statsContainer.Clear();

            foreach (KeyValuePair<Sprite, string> stat in stats)
            {
                VisualElement statContainerRoot = _statContainerTemplate.CloneTree();
                statContainerRoot.AddToClassList(STAT_CONTAINER_ROOT_CLASSNAME);
                StatContainerUIController.SetupStatContainer(
                    root: statContainerRoot.Children().First(),
                    icon: stat.Key,
                    statValue: stat.Value,
                    statValueColor: _statValueColor,
                    justifyContent: Justify.FlexStart,
                    iconTintColor: _statIconTintColor
                );
                _statsContainer.Add(statContainerRoot);
            }
        }

        private void SetupEffectsList(Label titleLabel, VisualElement effectsDescriptionContainer,
            string effectsListTitle, List<string> effectsDescription)
        {
            titleLabel.text = $"<u>{effectsListTitle}</u>";
            effectsDescriptionContainer.Clear();
            effectsDescription.ForEach(effectDescription =>
            {
                Label effectLabel = new();
                effectLabel.AddToClassList(_effectLineClassname);
                effectLabel.text = effectDescription;
                effectsDescriptionContainer.Add(effectLabel);
            });
        }

        #region UI Elements Names & Classes

        private static readonly string ICON_UI_NAME = "Icon";
        private static readonly string NAME_LABEL_UI_NAME = "NameLabel";
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "DescriptionLabel";
        private static readonly string STATS_CONTAINER_UI_NAME = "StatsContainer";
        private static readonly string PRIMARY_EFFECTS_LIST_TITLE_UI_NAME = "EffectsTitle";
        private static readonly string PRIMARY_EFFECTS_LIST_CONTAINER_UI_NAME = "EffectsList";
        private static readonly string SECONDARY_EFFECTS_LIST_TITLE_UI_NAME = "SecondaryEffectsTitle";
        private static readonly string SECONDARY_EFFECTS_LIST_CONTAINER_UI_NAME = "SecondaryEffectsList";

        private static readonly string STAT_CONTAINER_ROOT_CLASSNAME = "objectStatContainerRoot";

        #endregion
    }
}