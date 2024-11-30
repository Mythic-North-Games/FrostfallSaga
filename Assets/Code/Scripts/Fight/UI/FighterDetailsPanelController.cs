using System.Linq;
using System.Collections.Generic;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterDetailsPanelController : BaseUIController
    {
        #region UI elements names
        private static readonly string PANEL_UI_NAME = "FighterDetailsPanel";
        private static readonly string PANEL_HIDDEN_CLASS_NAME = "fighterDetailsPanelHidden";

        private static readonly string ICON_UI_NAME = "Icon";
        private static readonly string NAME_LABEL_UI_NAME = "NameLabel";
        private static readonly string CLASS_LABEL_UI_NAME = "ClassLabel";

        private static readonly string HEALTH_LABEL_UI_NAME = "ProgressLifeBar_Label";
        private static readonly string HEALTH_BAR_UI_NAME = "ProgressLifeBar_Progress";
        private static readonly string ACTION_POINTS_LABEL_UI_NAME = "ProgressActionBar_Label";
        private static readonly string ACTION_POINTS_BAR_UI_NAME = "ProgressActionBar_Progress";
        private static readonly string MOVE_POINTS_LABEL_UI_NAME = "ProgressMoveBar_Label";
        private static readonly string MOVE_POINTS_BAR_UI_NAME = "ProgressMoveBar_Progress";

        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesContainer";
        private static readonly string STATUS_CONTAINER_UI_NAME = "StatusContainer";

        private static readonly string STRENGTH_LABEL_UI_NAME = "StrengthLabel";
        private static readonly string DEXTERITY_LABEL_UI_NAME = "DexterityLabel";
        private static readonly string TENACITY_LABEL_UI_NAME = "TenacityLabel";
        private static readonly string PHYSICAL_RESISTANCE_LABEL_UI_NAME = "PhysicalResistanceLabel";
        private static readonly string DODGE_LABEL_UI_NAME = "DodgeLabel";
        private static readonly string MASTERSTROKE_LABEL_UI_NAME = "MasterstrokeLabel";
        private static readonly string INITIATIVE_LABEL_UI_NAME = "InitiativeLabel";

        private static readonly string MAGICAL_STRENGTHS_LABEL_UI_NAME = "StrengthLabel";
        private static readonly string MAGICAL_RESISTANCES_LABEL_UI_NAME = "ResistanceLabel";
        #endregion

        [SerializeField] private VisualTreeAsset _fighterDetailsUIPanel;
        private TemplateContainer _detailsPanel;

        private void Start()
        {
            _detailsPanel = _fighterDetailsUIPanel.Instantiate();
            _uiDoc.rootVisualElement.Add(_detailsPanel);
            Hide();
        }

        public void Display(Fighter fighter, Vector2Int displayPosition)
        {
            fighter.onDamageReceived += (fighter, damageAmount, isMasterstroke) => UpdateHealthBar(fighter);
            fighter.onHealReceived += (fighter, healAmount, isMasterstroke) => UpdateHealthBar(fighter);
            fighter.onStatMutationReceived += (fighter, stat, value) => {
                UpdateHealthBar(fighter);
                UpdateActionBar(fighter);
                UpdateMoveBar(fighter);
                UpdateNonMagicalStats(fighter);
                UpdateMagicalStats(fighter);
            };
            fighter.onStatusApplied += (fighter, status) => UpdateStatuses(fighter);
            fighter.onStatusRemoved += (fighter, status) => UpdateStatuses(fighter);

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
            UpdateMainInfo(fighter);
            UpdateHealthBar(fighter);
            UpdateActionBar(fighter);
            UpdateMoveBar(fighter);
            UpdateNonMagicalStats(fighter);
            UpdateMagicalStats(fighter);
            UpdateStatuses(fighter);
        }

        private void UpdateMainInfo(Fighter fighter)
        {
            _detailsPanel.Q<VisualElement>(ICON_UI_NAME).style.backgroundImage = new(fighter.DiamondIcon);
            _detailsPanel.Q<Label>(NAME_LABEL_UI_NAME).text = fighter.FighterName;
            _detailsPanel.Q<Label>(CLASS_LABEL_UI_NAME).text = fighter.FighterClass.ClassName;
        }

        private void UpdateHealthBar(Fighter fighter)
        {
            _detailsPanel.Q<Label>(HEALTH_LABEL_UI_NAME).text = $"{fighter.GetHealth()}/{fighter.GetMaxHealth()}";
            _detailsPanel.Q<VisualElement>(HEALTH_BAR_UI_NAME).style.width = new Length(
                (float)fighter.GetHealth() / fighter.GetMaxHealth() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateActionBar(Fighter fighter)
        {
            _detailsPanel.Q<Label>(ACTION_POINTS_LABEL_UI_NAME).text = $"{fighter.GetActionPoints()}/{fighter.GetMaxActionPoints()}";
            _detailsPanel.Q<VisualElement>(ACTION_POINTS_BAR_UI_NAME).style.width = new Length(
                (float)fighter.GetActionPoints() / fighter.GetMaxActionPoints() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateMoveBar(Fighter fighter)
        {
            _detailsPanel.Q<Label>(MOVE_POINTS_LABEL_UI_NAME).text = $"{fighter.GetMovePoints()}/{fighter.GetMaxMovePoints()}";
            _detailsPanel.Q<VisualElement>(MOVE_POINTS_BAR_UI_NAME).style.width = new Length(
                (float)fighter.GetMovePoints() / fighter.GetMaxMovePoints() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateNonMagicalStats(Fighter fighter)
        {
            _detailsPanel.Q<Label>(STRENGTH_LABEL_UI_NAME).text = fighter.GetStrength().ToString();
            _detailsPanel.Q<Label>(DEXTERITY_LABEL_UI_NAME).text = fighter.GetDexterity().ToString();
            _detailsPanel.Q<Label>(TENACITY_LABEL_UI_NAME).text = fighter.GetTenacity().ToString();
            _detailsPanel.Q<Label>(PHYSICAL_RESISTANCE_LABEL_UI_NAME).text = fighter.GetPhysicalResistance().ToString();
            _detailsPanel.Q<Label>(DODGE_LABEL_UI_NAME).text = fighter.GetDodgeChance().ToString();
            _detailsPanel.Q<Label>(MASTERSTROKE_LABEL_UI_NAME).text = fighter.GetMasterstrokeChance().ToString();
            _detailsPanel.Q<Label>(INITIATIVE_LABEL_UI_NAME).text = fighter.GetInitiative().ToString();
        }

        private void UpdateMagicalStats(Fighter fighter)
        {
            foreach (KeyValuePair<EMagicalElement, int> magicalResistance in fighter.GetMagicalResistances())
            {
                string elementName = magicalResistance.Key.ToUIString();
                int resistance = magicalResistance.Value;

                _detailsPanel.Q<Label>($"{elementName}{MAGICAL_STRENGTHS_LABEL_UI_NAME}").text = resistance.ToString();
                _detailsPanel.Q<Label>($"{elementName}{MAGICAL_RESISTANCES_LABEL_UI_NAME}").text = resistance.ToString();
            }
        }

        private void UpdateStatuses(Fighter fighter)
        {
            int maxStatusesContainers = _detailsPanel.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME).childCount;
            Dictionary<AStatus, (bool isActive, int duration)> currentFighterStatuses = fighter.GetStatuses();
            for (int i = 1; i <= currentFighterStatuses.Count; i++)
            {
                if (i > maxStatusesContainers)
                {
                    break;
                }
                VisualElement statusContainer = _detailsPanel.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = new(currentFighterStatuses.ElementAt(i - 1).Key.Icon);
            }

            for (int i = currentFighterStatuses.Count + 1; i <= maxStatusesContainers; i++)
            {
                VisualElement statusContainer = _detailsPanel.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = null;
            }
        }
    }
}