using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Core;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.UI
{
    public class FighterFloatingBarController : BaseUIController
    {
        private static readonly string FLOATING_BAR_CONTAINER_UI_NAME = "FighterFloatingBar";
        private static readonly string PERSONALITY_TRAIT_ICON_UI_NAME = "PersonalityTraitIcon";
        private static readonly string HP_LABEL_UI_NAME = "LifeLabel";
        private static readonly string HP_PROGRESS_UI_NAME = "LifeProgress";

        [SerializeField, Header("UI options")] private VisualTreeAsset _floatingBarUIPanel;

        [SerializeField] private FightersGenerator _fightersGenerator;
        [SerializeField] private FightManager _fightManager;
        private FighterStatusesBarController _fighterStatusesBarController;
        private Dictionary<Fighter, TemplateContainer> _fighterFloatingBars = new();

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                // Spawn the floating bar for the fighter
                _fighterFloatingBars.Add(fighter, SpawnFloatingBarPanelForFighter(fighter));
                _fighterStatusesBarController = new(_fighterFloatingBars[fighter]);

                // Start listening to the fighter's events
                fighter.onDamageReceived += (fighter, damageAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onHealReceived += (fighter, healAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onNonMagicalStatMutated += (fighter, stat, value) => UpdateHealthBar(fighter);
                fighter.onStatusApplied += (fighter, status) => _fighterStatusesBarController.UpdateStatuses(fighter);
                fighter.onStatusRemoved += (fighter, status) => _fighterStatusesBarController.UpdateStatuses(fighter);
                fighter.onFighterDied += (fighter) => _fighterFloatingBars[fighter].RemoveFromHierarchy();

                // Update the floating bar
                UpdateHealthBar(fighter);
                UpdateCharacterTrait(fighter);
                _fighterStatusesBarController.UpdateStatuses(fighter);
            }
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            foreach (Fighter fighter in _fighterFloatingBars.Keys)
            {
                _fighterFloatingBars[fighter].RemoveFromHierarchy();
            }
            _fighterFloatingBars.Clear();
        }

        private TemplateContainer SpawnFloatingBarPanelForFighter(Fighter fighter)
        {
            TemplateContainer floatingBar = _floatingBarUIPanel.Instantiate();
            floatingBar.style.position = Position.Absolute;
            floatingBar.name = $"{fighter.name}FloatingBarPanel";
            floatingBar.transform.position = ComputeFloatingBarPosition(fighter, floatingBar);
            _uiDoc.rootVisualElement.Add(floatingBar);
            floatingBar.SendToBack();
            return floatingBar;
        }

        private void UpdateHealthBar(Fighter fighter)
        {
            TemplateContainer floatingBar = _fighterFloatingBars[fighter];
            Label healthLabel = floatingBar.Q<Label>(HP_LABEL_UI_NAME);
            VisualElement healthProgress = floatingBar.Q<VisualElement>(HP_PROGRESS_UI_NAME);

            healthLabel.text = $"{fighter.GetHealth()}/{fighter.GetMaxHealth()}";
            healthProgress.style.width = new Length(
                (float)fighter.GetHealth() / fighter.GetMaxHealth() * 100,
                LengthUnit.Percent
            );
        }

        private void UpdateCharacterTrait(Fighter fighter)
        {
            TemplateContainer floatingBar = _fighterFloatingBars[fighter];
            VisualElement characterTraitIcon = floatingBar.Q<VisualElement>(PERSONALITY_TRAIT_ICON_UI_NAME);
            if (fighter.PersonalityTrait == null)
            {
                characterTraitIcon.visible = false;
            }
            else
            {
                characterTraitIcon.style.backgroundImage = new(fighter.PersonalityTrait.Icon);
            }
        }

        private void Update()
        {
            foreach (Fighter fighter in _fighterFloatingBars.Keys)
            {
                TemplateContainer floatingBar = _fighterFloatingBars[fighter];
                floatingBar.transform.position = ComputeFloatingBarPosition(fighter, floatingBar);
            }
        }

        private Vector2 ComputeFloatingBarPosition(Fighter fighter, TemplateContainer floatingBar)
        {
            // Convert the fighter's world position to screen space (pixels)
            Vector3 fighterScreenPosition = Camera.main.WorldToScreenPoint(fighter.transform.position);

            // Convert the fighter's screen position to UI space
            Vector2 fighterUIPosition = RuntimePanelUtils.ScreenToPanel(_uiDoc.rootVisualElement.panel, fighterScreenPosition);

            // Compute the floating bar's position offset
            Rect panelLayout = floatingBar.Q<VisualElement>(FLOATING_BAR_CONTAINER_UI_NAME).layout;
            float xOffset = -(panelLayout.width / 2); // Center horizontally

            return new(fighterUIPosition.x + xOffset, Screen.height - fighterUIPosition.y);
        }


        #region Setup & teardown

        private void Awake()
        {
            _fightersGenerator.onFightersGenerated += OnFightersGenerated;
            _fightManager.onFightEnded += OnFightEnded;
        }

        #endregion
    }
}