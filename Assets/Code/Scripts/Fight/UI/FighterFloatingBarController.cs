using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Utils.UI;

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
        private readonly Dictionary<Fighter, TemplateContainer> _fighterFloatingBars = new();
        private readonly Dictionary<Fighter, WorldUIPositioner> _fighterFloatingBarsPositioner = new();

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                // Spawn the floating bar for the fighter
                _fighterFloatingBars.Add(fighter, SpawnFloatingBarPanelForFighter(fighter));
                _fighterFloatingBarsPositioner.Add(fighter, SpawnFloatingBarPositioner(fighter, _fighterFloatingBars[fighter]));
                _fighterStatusesBarController = new(_fighterFloatingBars[fighter]);

                // Start listening to the fighter's events
                fighter.onDamageReceived += (fighter, damageAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onHealReceived += (fighter, healAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onNonMagicalStatMutated += (fighter, stat, value) => UpdateHealthBar(fighter);
                fighter.onStatusApplied += (fighter, status) => _fighterStatusesBarController.UpdateStatuses(fighter);
                fighter.onStatusRemoved += (fighter, status) => _fighterStatusesBarController.UpdateStatuses(fighter);
                fighter.onFighterDied += (fighter) =>
                {
                    if (fighter == null || !_fighterFloatingBars.ContainsKey(fighter)) return;
                    _fighterFloatingBars[fighter].RemoveFromHierarchy();
                    Destroy(_fighterFloatingBarsPositioner[fighter]);
                };

                // Update the floating bar data
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
            // Instantiate and setup floating bar
            TemplateContainer floatingBar = _floatingBarUIPanel.Instantiate();
            floatingBar.style.position = Position.Absolute;
            floatingBar.name = $"{fighter.name}FloatingBarPanel";

            // Place in existing hierarchie
            _uiDoc.rootVisualElement.Add(floatingBar);
            floatingBar.SendToBack();

            return floatingBar;
        }

        private WorldUIPositioner SpawnFloatingBarPositioner(Fighter fighter, TemplateContainer floatingBar)
        {
            VisualElement floatingBarRoot = floatingBar.Q<VisualElement>(FLOATING_BAR_CONTAINER_UI_NAME);
            WorldUIPositioner floatingBarPositioner = gameObject.AddComponent<WorldUIPositioner>();
            floatingBarPositioner.Setup(
                _uiDoc,
                floatingBarRoot,
                fighter.transform,
                centerOnX: true,
                centerOnY: false,
                offset: new(0, 20)
            );
            return floatingBarPositioner;
        }

        private void UpdateHealthBar(Fighter fighter)
        {
            if (fighter == null || !_fighterFloatingBars.ContainsKey(fighter)) return;

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
            characterTraitIcon.visible = false;
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