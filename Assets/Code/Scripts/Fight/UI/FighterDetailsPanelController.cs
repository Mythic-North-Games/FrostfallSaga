using System.Collections.Generic;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterDetailsPanelController : BaseUIController
    {
        [SerializeField] private VisualTreeAsset _fighterDetailsUIPanel;
        [SerializeField] private SElementToValue<EFighterMutableStat, Texture2D>[] _fighterStatIcons;
        [SerializeField] private SElementToValue<EMagicalElement, Texture2D>[] _magicalElementIcons;

        private TemplateContainer _detailsPanel;
        private FighterProgressBarsController _progressBarsController;
        private FighterStatusesBarController _statusesBarController;

        private void Start()
        {
            _detailsPanel = _fighterDetailsUIPanel.Instantiate();
            _detailsPanel.style.position = Position.Absolute;
            _detailsPanel.AddToClassList(PANEL_HIDDEN_CLASS_NAME);
            SetupStatIcons();
            _uiDoc.rootVisualElement.Add(_detailsPanel);
            Hide();
        }

        public void Display(Fighter fighter, Vector2Int displayPosition)
        {
            fighter.onDamageReceived += (fighter, damageAmount, isMasterstroke) =>
                _progressBarsController.UpdateHealthBar(fighter);
            fighter.onHealReceived += (fighter, healAmount, isMasterstroke) =>
                _progressBarsController.UpdateHealthBar(fighter);
            fighter.onNonMagicalStatMutated += (fighter, stat, value) =>
            {
                _progressBarsController.UpdateAllBars(fighter);
                UpdateNonMagicalStats(fighter);
            };
            fighter.onMagicalStatMutated +=
                (fighter, magicalElement, value, isResistance) => UpdateMagicalStats(fighter);
            fighter.onStatusApplied += (fighter, status) => _statusesBarController.UpdateStatuses(fighter);
            fighter.onStatusRemoved += (fighter, status) => _statusesBarController.UpdateStatuses(fighter);

            UpdatePanelData(fighter);
            _detailsPanel.style.left = new Length(displayPosition.x, LengthUnit.Pixel);
            _detailsPanel.style.top = new Length(displayPosition.y, LengthUnit.Pixel);
            _detailsPanel.RemoveFromClassList(PANEL_HIDDEN_CLASS_NAME);
        }

        public void Hide()
        {
            _detailsPanel.AddToClassList(PANEL_HIDDEN_CLASS_NAME);
        }

        public Vector2Int GetPanelSize()
        {
            return new Vector2Int(
                (int)_detailsPanel.Q<VisualElement>(PANEL_UI_NAME).layout.width,
                (int)_detailsPanel.Q<VisualElement>(PANEL_UI_NAME).layout.height
            );
        }

        private void UpdatePanelData(Fighter fighter)
        {
            _progressBarsController =
                new FighterProgressBarsController(_detailsPanel.Q(PROGRESS_BARS_CONTAINER_UI_NAME));
            _statusesBarController = new FighterStatusesBarController(_detailsPanel.Q(STATUSES_BAR_CONTAINER_UI_NAME));

            UpdateMainInfo(fighter);
            _progressBarsController.UpdateAllBars(fighter);
            _statusesBarController.UpdateStatuses(fighter);
            UpdateNonMagicalStats(fighter);
            UpdateMagicalStats(fighter);
        }

        private void UpdateMainInfo(Fighter fighter)
        {
            _detailsPanel.Q<VisualElement>(FIGHTER_ICON_UI_NAME).style.backgroundImage =
                new StyleBackground(fighter.DiamondIcon);
            _detailsPanel.Q<Label>(NAME_LABEL_UI_NAME).text = fighter.FighterName;
            _detailsPanel.Q<Label>(CLASS_LABEL_UI_NAME).text = fighter.FighterClass.ClassName;
        }

        private void UpdateNonMagicalStats(Fighter fighter)
        {
            _detailsPanel.Q(STRENGTH_STAT_CONTAINER_UI_NAME).Q<Label>("StatLabel").text =
                fighter.GetStrength().ToString();
            _detailsPanel.Q(DEXTERITY_STAT_CONTAINER_UI_NAME).Q<Label>("StatLabel").text =
                fighter.GetDexterity().ToString();
            _detailsPanel.Q(TENACITY_STAT_CONTAINER_NAME).Q<Label>("StatLabel").text = fighter.GetTenacity().ToString();
            _detailsPanel.Q(PHYSICAL_RESISTANCE_STAT_CONTAINER_NAME).Q<Label>("StatLabel").text =
                fighter.GetPhysicalResistance().ToString();
            _detailsPanel.Q(DODGE_STAT_CONTAINER_NAME).Q<Label>("StatLabel").text = fighter.GetDodgeChance().ToString();
            _detailsPanel.Q(MASTERSTROKE_STAT_CONTAINER_NAME).Q<Label>("StatLabel").text =
                fighter.GetMasterstrokeChance().ToString();
            _detailsPanel.Q(INITIATIVE_STAT_CONTAINER_NAME).Q<Label>("StatLabel").text =
                fighter.GetInitiative().ToString();
        }

        private void UpdateMagicalStats(Fighter fighter)
        {
            foreach (KeyValuePair<EMagicalElement, int> magicalResistance in fighter.GetMagicalResistances())
            {
                string elementName = magicalResistance.Key.ToUIString();
                int resistance = magicalResistance.Value;

                _detailsPanel.Q($"{elementName}{MAGICAL_STRENGTHS_STAT_CONTAINER_NAME}").Q<Label>("StatLabel").text =
                    resistance.ToString();
                _detailsPanel.Q($"{elementName}{MAGICAL_RESISTANCES_STAT_CONTAINER_NAME}").Q<Label>("StatLabel").text =
                    resistance.ToString();
            }
        }

        private void SetupStatIcons()
        {
            Dictionary<EFighterMutableStat, Texture2D> statIcons =
                SElementToValue<EFighterMutableStat, Texture2D>.GetDictionaryFromArray(
                    _fighterStatIcons
                );

            AssignSingleStatIcon(_detailsPanel.Q(STRENGTH_STAT_CONTAINER_UI_NAME),
                statIcons[EFighterMutableStat.Strength]);
            AssignSingleStatIcon(_detailsPanel.Q(DEXTERITY_STAT_CONTAINER_UI_NAME),
                statIcons[EFighterMutableStat.Dexterity]);
            AssignSingleStatIcon(_detailsPanel.Q(TENACITY_STAT_CONTAINER_NAME),
                statIcons[EFighterMutableStat.Tenacity]);
            AssignSingleStatIcon(_detailsPanel.Q(PHYSICAL_RESISTANCE_STAT_CONTAINER_NAME),
                statIcons[EFighterMutableStat.PhysicalResistance]);
            AssignSingleStatIcon(_detailsPanel.Q(DODGE_STAT_CONTAINER_NAME),
                statIcons[EFighterMutableStat.DodgeChance]);
            AssignSingleStatIcon(_detailsPanel.Q(MASTERSTROKE_STAT_CONTAINER_NAME),
                statIcons[EFighterMutableStat.MasterstrokeChance]);
            AssignSingleStatIcon(_detailsPanel.Q(INITIATIVE_STAT_CONTAINER_NAME),
                statIcons[EFighterMutableStat.Initiative]);

            Dictionary<EMagicalElement, Texture2D> magicalElementIcons =
                SElementToValue<EMagicalElement, Texture2D>.GetDictionaryFromArray(
                    _magicalElementIcons
                );

            foreach (KeyValuePair<EMagicalElement, Texture2D> magicalElementIcon in magicalElementIcons)
            {
                string elementName = magicalElementIcon.Key.ToUIString();
                AssignSingleStatIcon(_detailsPanel.Q($"{elementName}{MAGICAL_STRENGTHS_STAT_CONTAINER_NAME}"),
                    magicalElementIcon.Value);
                AssignSingleStatIcon(_detailsPanel.Q($"{elementName}{MAGICAL_RESISTANCES_STAT_CONTAINER_NAME}"),
                    magicalElementIcon.Value);
            }
        }

        private void AssignSingleStatIcon(VisualElement statContainer, Texture2D icon)
        {
            statContainer.Q<VisualElement>("StatIcon").style.backgroundImage = new StyleBackground(icon);
        }

        #region UI elements names

        private static readonly string PANEL_UI_NAME = "FighterDetailsPanel";
        private static readonly string PANEL_HIDDEN_CLASS_NAME = "fighterDetailsPanelHidden";

        private static readonly string FIGHTER_ICON_UI_NAME = "WhiteDiamondBackground";
        private static readonly string NAME_LABEL_UI_NAME = "NameLabel";
        private static readonly string CLASS_LABEL_UI_NAME = "ClassLabel";

        private static readonly string PROGRESS_BARS_CONTAINER_UI_NAME = "ProgressBars";
        private static readonly string STATUSES_BAR_CONTAINER_UI_NAME = "StatusesBar";

        private static readonly string STRENGTH_STAT_CONTAINER_UI_NAME = "StrengthStatContainer";
        private static readonly string DEXTERITY_STAT_CONTAINER_UI_NAME = "DexterityStatContainer";
        private static readonly string TENACITY_STAT_CONTAINER_NAME = "TenacityStatContainer";
        private static readonly string PHYSICAL_RESISTANCE_STAT_CONTAINER_NAME = "PhysicalResistanceStatContainer";
        private static readonly string DODGE_STAT_CONTAINER_NAME = "DodgeStatContainer";
        private static readonly string MASTERSTROKE_STAT_CONTAINER_NAME = "MasterstrokeStatContainer";
        private static readonly string INITIATIVE_STAT_CONTAINER_NAME = "InitiativeStatContainer";

        private static readonly string MAGICAL_STRENGTHS_STAT_CONTAINER_NAME = "StrengthStatContainer";
        private static readonly string MAGICAL_RESISTANCES_STAT_CONTAINER_NAME = "ResistanceStatContainer";

        #endregion
    }
}