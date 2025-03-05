using UnityEngine.UIElements;
using FrostfallSaga.Core.InventorySystem;
using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.InventorySystem.UI
{
    public class ItemDetailsContentUIController
    {
        #region UI Elements Names & Classes
        private static readonly string STATS_CONTAINER_UI_NAME = "StatsContainer";
        private static readonly string SPECIAL_EFFECTS_CONTENT_CONTAINER_UI_NAME = "SpecialEffectsContentContainer";
        private static readonly string SPECIAL_EFFECTS_TITLE_LABEL_UI_NAME = "SpecialEffectsTitle";

        private static readonly string SPECIAL_EFFECT_LABEL_DEFAULT_CLASSNAME = "specialEffectLabelDefault";
        #endregion

        private readonly VisualElement _root;
        private readonly VisualElement _statsContainer;
        private readonly VisualElement _specialEffectsContentContainer;
        private readonly Label _specialEffectsTitleLabel;
        private readonly VisualTreeAsset _statContainerTemplate;

        public ItemDetailsContentUIController(VisualElement root, VisualTreeAsset statContainerTemplate)
        {
            _root = root;
            _statsContainer = _root.Q<VisualElement>(STATS_CONTAINER_UI_NAME);
            _specialEffectsContentContainer = _root.Q<VisualElement>(SPECIAL_EFFECTS_CONTENT_CONTAINER_UI_NAME);
            _specialEffectsTitleLabel = _root.Q<Label>(SPECIAL_EFFECTS_TITLE_LABEL_UI_NAME);
            _statContainerTemplate = statContainerTemplate;
        }

        public void SetItem(ItemSO item)
        {
            SetupStats(item);
            SetupSpecialEffects(item);
        }

        public void ClearItem()
        {
            _statsContainer.Clear();
            _specialEffectsTitleLabel.visible = false;
            _specialEffectsContentContainer.Clear();
        }

        private void SetupStats(ItemSO item)
        {
            // Clear previous stats
            _statsContainer.Clear();

            if (item is AEquipment equipment)
            {
                _statsContainer.style.display = DisplayStyle.Flex;
                _specialEffectsTitleLabel.text = "Special Effects";

                // Classic stats
                foreach (KeyValuePair<string, string> statValue in equipment.GetStatsUIData())
                {
                    new ItemStatContainerUIController(
                        _statsContainer,
                        _statContainerTemplate,
                        UIIconsProvider.Instance.GetIcon(statValue.Key),
                        statValue.Value
                    );
                }

                // Magical stats
                foreach(KeyValuePair<EMagicalElement, string> magicalDamagesStats in equipment.GetMagicalStatsUIData())
                {
                    new ItemStatContainerUIController(
                        _statsContainer,
                        _statContainerTemplate,
                        UIIconsProvider.Instance.GetIcon(magicalDamagesStats.Key.GetIconResourceName()),
                        magicalDamagesStats.Value
                    );
                }
            }
            else if (item is AConsumable)
            {
                // Consumables don't have stats, hide the stats container
                _statsContainer.style.display = DisplayStyle.None;

                // Rename the special effects title to "Effects"
                _specialEffectsTitleLabel.text = "Effects";
            }
        }

        private void SetupSpecialEffects(ItemSO item)
        {
            // Clear previous special effects
            _specialEffectsContentContainer.Clear();

            List<string> specialEffectsUIData = new();

            if (item is AEquipment equipment)
            {
                specialEffectsUIData = equipment.GetSpecialEffectsUIData();
            }
            else if (item is AConsumable consumable)
            {
                specialEffectsUIData = consumable.GetSpecialEffectsUIData();
            }

            _specialEffectsTitleLabel.visible = specialEffectsUIData.Count > 0;
            specialEffectsUIData.ForEach(effectDescription =>
            {
                Label effectLabel = new();
                effectLabel.AddToClassList(SPECIAL_EFFECT_LABEL_DEFAULT_CLASSNAME);
                effectLabel.text = effectDescription;
                _specialEffectsContentContainer.Add(effectLabel);
            });
        }
    }
}