using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class ObjectDetailsUIController
    {
        private const string MORE_PADDING_TOP_LETTERS = "lkhfdI";

        #region UI Elements Names & Classes
        private static readonly string ICON_UI_NAME = "Icon";
        private static readonly string NAME_LABEL_UI_NAME = "NameLabel";
        private static readonly string DESCRIPTION_LABEL_UI_NAME = "DescriptionLabel";
        private static readonly string DETAILS_CONTENT_UI_NAME = "DetailsContent";
        private static readonly string STATS_CONTAINER_UI_NAME = "StatsContainer";
        private static readonly string PRIMARY_EFFECTS_CONTAINER_UI_NAME = "EffectsContainer";
        private static readonly string PRIMARY_EFFECTS_LIST_TITLE_UI_NAME = "EffectsTitle";
        private static readonly string PRIMARY_EFFECTS_LIST_CONTAINER_UI_NAME = "EffectsList";
        private static readonly string SECONDARY_EFFECTS_CONTAINER_UI_NAME = "SecondaryEffectsContainer";
        private static readonly string SECONDARY_EFFECTS_LIST_TITLE_UI_NAME = "SecondaryEffectsTitle";
        private static readonly string SECONDARY_EFFECTS_LIST_CONTAINER_UI_NAME = "SecondaryEffectsList";

        private static readonly string STAT_CONTAINER_ROOT_CLASSNAME = "objectStatContainerRoot";
        #endregion

        private readonly VisualElement _root;

        private readonly Label _nameLabel;
        private readonly Label _descriptionLabel;
        private readonly VisualElement _icon;

        private readonly VisualElement _detailsContentContainer;

        private readonly string _effectLineClassname;
        private readonly VisualElement _primaryEffectsContainer;
        private readonly VisualElement _primaryEffectsListContainer;
        private readonly Label _primaryEffectsTitleLabel;

        private readonly VisualElement _secondaryEffectsContainer;
        private readonly VisualElement _secondaryEffectsListContainer;
        private readonly Label _secondaryEffectsTitleLabel;

        private readonly VisualTreeAsset _statContainerTemplate;
        private readonly VisualElement _statsContainer;
        private readonly Color _statValueColor;
        private readonly Color _statIconTintColor;

        private readonly float _extraPaddingTop;

        public ObjectDetailsUIController(
            VisualElement root,
            VisualTreeAsset statContainerTemplate,
            string effectLineClassname,
            Color statValueColor = default,
            Color statIconTintColor = default,
            float extraPaddingTopOnBigLetters = 0f
        )
        {
            _root = root;
            _statContainerTemplate = statContainerTemplate;
            _effectLineClassname = effectLineClassname;
            _statValueColor = statValueColor;
            _statIconTintColor = statIconTintColor;
            _extraPaddingTop = extraPaddingTopOnBigLetters;

            _icon = _root.Q<VisualElement>(ICON_UI_NAME);
            _nameLabel = _root.Q<Label>(NAME_LABEL_UI_NAME);
            _descriptionLabel = _root.Q<Label>(DESCRIPTION_LABEL_UI_NAME);
            _detailsContentContainer = _root.Q<VisualElement>(DETAILS_CONTENT_UI_NAME);
            _statsContainer = _root.Q<VisualElement>(STATS_CONTAINER_UI_NAME);
            _primaryEffectsContainer = _root.Q<VisualElement>(PRIMARY_EFFECTS_CONTAINER_UI_NAME);
            _primaryEffectsTitleLabel = _root.Q<Label>(PRIMARY_EFFECTS_LIST_TITLE_UI_NAME);
            _primaryEffectsListContainer = _root.Q<VisualElement>(PRIMARY_EFFECTS_LIST_CONTAINER_UI_NAME);
            _secondaryEffectsContainer = _root.Q<VisualElement>(SECONDARY_EFFECTS_CONTAINER_UI_NAME);
            _secondaryEffectsTitleLabel = _root.Q<Label>(SECONDARY_EFFECTS_LIST_TITLE_UI_NAME);
            _secondaryEffectsListContainer = _root.Q<VisualElement>(SECONDARY_EFFECTS_LIST_CONTAINER_UI_NAME);

            _statsContainer.style.display = DisplayStyle.None;
            _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;
            _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;
        }

        public void Setup(IUIObjectDescribable objectToDescribe)
        {
            SetupHeader(objectToDescribe.GetIcon(), objectToDescribe.GetName(), objectToDescribe.GetDescription());

            if (!HasAnyStatsOrEffects(objectToDescribe))
            {
                _detailsContentContainer.style.display = DisplayStyle.None;
                return;
            }
            _detailsContentContainer.style.display = DisplayStyle.Flex;

            Dictionary<Sprite, string> stats = objectToDescribe.GetStatsUIData();
            if (stats != null && stats.Count > 0)
            {
                _statsContainer.style.display = DisplayStyle.Flex;
                SetupStats(stats);
            }
            else _statsContainer.style.display = DisplayStyle.None;

            List<string> primaryEffects = objectToDescribe.GetPrimaryEffectsUIData();
            if (primaryEffects != null && primaryEffects.Count > 0)
            {
                _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.Flex;
                SetupEffectsList(
                    _primaryEffectsTitleLabel,
                    _primaryEffectsListContainer,
                    objectToDescribe.GetPrimaryEffectsTitle(),
                    primaryEffects
                );
            }
            else _primaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;

            List<string> secondaryEffects = objectToDescribe.GetSecondaryEffectsUIData();
            if (secondaryEffects != null && secondaryEffects.Count > 0)
            {
                _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.Flex;
                SetupEffectsList(
                    _secondaryEffectsTitleLabel,
                    _secondaryEffectsListContainer,
                    objectToDescribe.GetSecondaryEffectsTitle(),
                    secondaryEffects
                );
            }
            else _secondaryEffectsTitleLabel.parent.style.display = DisplayStyle.None;

            AdjustEffectsContainersMargins(primaryEffects, secondaryEffects);
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

        private void SetupEffectsList(
            Label titleLabel,
            VisualElement effectsDescriptionContainer,
            string effectsListTitle,
            List<string> effectsDescription
        )
        {
            titleLabel.text = effectsListTitle;
            effectsDescriptionContainer.Clear();
            effectsDescription.ForEach(effectDescription =>
            {
                Label effectLabel = new();
                effectLabel.AddToClassList(_effectLineClassname);
                effectLabel.text = $"- {effectDescription}";
                effectsDescriptionContainer.Add(effectLabel);
            });
        }

        private void AdjustEffectsContainersMargins(List<string> primaryEffects, List<string> secondaryEffects)
        {
            bool hasPrimaryEffects = primaryEffects != null && primaryEffects.Count > 0;
            bool hasSecondaryEffects = secondaryEffects != null && secondaryEffects.Count > 0;

            if (hasPrimaryEffects && !hasSecondaryEffects)
            {
                _primaryEffectsContainer.style.marginTop = new Length(5, LengthUnit.Percent);
            }
            else if (!hasPrimaryEffects && hasSecondaryEffects)
            {
                _secondaryEffectsContainer.style.marginTop = new Length(5, LengthUnit.Percent);
            }
            else
            {
                _primaryEffectsContainer.style.marginTop = new Length(3, LengthUnit.Percent);
                _secondaryEffectsContainer.style.marginTop = new Length(3, LengthUnit.Percent);
            }
        }

        private bool HasAnyStatsOrEffects(IUIObjectDescribable objectToDescribe)
        {
            Dictionary<Sprite, string> stats = objectToDescribe.GetStatsUIData();
            List<string> primaryEffects = objectToDescribe.GetPrimaryEffectsUIData();
            List<string> secondaryEffects = objectToDescribe.GetSecondaryEffectsUIData();

            return (stats != null && stats.Count > 0) ||
                   (primaryEffects != null && primaryEffects.Count > 0) ||
                   (secondaryEffects != null && secondaryEffects.Count > 0);
        }
    }
}