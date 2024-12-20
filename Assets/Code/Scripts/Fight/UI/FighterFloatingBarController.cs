using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils.UI;

namespace FrostfallSaga.Fight.UI
{
    public class FighterFloatingBarController : BaseUIController
    {
        private static readonly string FLOATING_BAR_CONTAINER_UI_NAME = "FSF_Panel";
        private static readonly string CHARACTER_TRAIT_ICON_UI_NAME = "CharacterTraitIcon";
        private static readonly string HP_LABEL_UI_NAME = "LifeLabel";
        private static readonly string HP_PROGRESS_UI_NAME = "LifeProgress";
        private static readonly string STATUSES_CONTAINER_UI_NAME = "StatusesContainer";
        private static readonly string STATUS_CONTAINER_UI_NAME = "StatusContainer";

        [SerializeField, Header("UI options")] private VisualTreeAsset _floatingBarUIPanel;

        [SerializeField] private FightersGenerator _fightersGenerator;
        [SerializeField] private FightManager _fightManager;
        private Dictionary<Fighter, TemplateContainer> _fighterFloatingBars = new();
        private Dictionary<Fighter, WorldUIPositioner> _fighterFloatingBarsPositioner = new();

        private void OnFightersGenerated(Fighter[] allies, Fighter[] enemies)
        {
            foreach (Fighter fighter in allies.Concat(enemies))
            {
                fighter.onDamageReceived += (fighter, damageAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onHealReceived += (fighter, healAmount, isMasterstroke) => UpdateHealthBar(fighter);
                fighter.onNonMagicalStatMutated += (fighter, stat, value) => UpdateHealthBar(fighter);
                fighter.onStatusApplied += (fighter, status) => UpdateStatuses(fighter);
                fighter.onStatusRemoved += (fighter, status) => UpdateStatuses(fighter);
                fighter.onFighterDied += (fighter) =>
                {
                    _fighterFloatingBars[fighter].RemoveFromHierarchy();
                    Destroy(_fighterFloatingBarsPositioner[fighter]);
                };
                _fighterFloatingBars.Add(fighter, SpawnFloatingBarPanelForFighter(fighter));
                _fighterFloatingBarsPositioner.Add(fighter, SpawnFloatingBarPositioner(fighter, _fighterFloatingBars[fighter]));
                UpdateHealthBar(fighter);
                UpdateCharacterTrait(fighter);
                UpdateStatuses(fighter);
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
            VisualElement characterTraitIcon = floatingBar.Q<VisualElement>(CHARACTER_TRAIT_ICON_UI_NAME);
            if (fighter.PersonalityTrait == null)
            {
                characterTraitIcon.visible = false;
            }
            else
            {
                characterTraitIcon.style.backgroundImage = new(fighter.PersonalityTrait.Icon);
            }
        }

        private void UpdateStatuses(Fighter fighter)
        {
            TemplateContainer floatingBar = _fighterFloatingBars[fighter];

            int maxStatusesContainers = floatingBar.Q<VisualElement>(STATUSES_CONTAINER_UI_NAME).childCount;
            Dictionary<AStatus, (bool isActive, int duration)> currentFighterStatuses = fighter.GetStatuses();
            for (int i = 1; i <= currentFighterStatuses.Count; i++)
            {
                if (i > maxStatusesContainers)
                {
                    break;
                }
                VisualElement statusContainer = floatingBar.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = new(currentFighterStatuses.ElementAt(i - 1).Key.Icon);
            }

            for (int i = currentFighterStatuses.Count + 1; i <= maxStatusesContainers; i++)
            {
                VisualElement statusContainer = floatingBar.Q<VisualElement>($"{STATUS_CONTAINER_UI_NAME}{i}");
                statusContainer.style.backgroundImage = null;
            }
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