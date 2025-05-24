using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.Commands;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils.Inputs;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterInfosBarController : BaseUIController
    {
        [SerializeField] private VisualTreeAsset _statusContainerTemplate;
        [SerializeField] private VisualTreeAsset _statusDetailsOverlayTemplate;
        [SerializeField] private FightLoader _fightLoader;
        [SerializeField] private FightManager _fightManager;
        [SerializeField] private FightersOrderTimelineController _timelineController;
        [SerializeField] private float _displayDurationOnAction = 2f;

        private VisualElement _infosBarContainer;
        private Label _fighterNameLabel;
        private VisualElement _statusesBar;
        private StatusDetailsOverlayUIController _statusDetailsOverlayController;

        #region Setup

        private void Awake()
        {
            _uiDoc ??= GetComponent<UIDocument>();
            if (!_uiDoc)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            _fightManager ??= FindObjectOfType<FightManager>();
            if (!_fightManager)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            _fightLoader ??= FindObjectOfType<FightLoader>();
            if (!_fightLoader)
            {
                Debug.LogError("No FightLoader to work with. UI can't be updated dynamically.");
                return;
            }

            _timelineController ??= FindObjectOfType<FightersOrderTimelineController>();
            if (!_timelineController)
            {
                Debug.LogError("No FightersOrderTimelineController to work with. UI can't be updated dynamically.");
                return;
            }

            if (!_statusContainerTemplate)
            {
                Debug.LogError("No status container template to work with.");
                return;
            }

            _fightManager.onFightEnded += OnFightEnded;
            _fightLoader.onFightLoaded += OnFightLoaded;
            _timelineController.onFighterHovered += OnFighterHovered;
            _timelineController.onFighterUnhovered += OnFighterUnhovered;
        }

        #endregion

        private void Start()
        {
            _infosBarContainer = _uiDoc.rootVisualElement.Q<VisualElement>(INFOS_BAR_CONTAINER_UI_NAME);
            _fighterNameLabel = _infosBarContainer.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME);
            _statusesBar = _infosBarContainer.Q<VisualElement>(STATUSES_BAR_UI_NAME);
            _statusDetailsOverlayController = new(_statusDetailsOverlayTemplate);
            Hide();
        }

        private void OnFighterHovered(Fighter fighter)
        {
            SetupBar(fighter);
            Display();
        }

        private void OnFighterUnhovered(Fighter fighter)
        {
            Hide();
        }

        private IEnumerator DisplayForSeconds(Fighter fighter)
        {
            SetupBar(fighter);
            Display();
            yield return new WaitForSeconds(_displayDurationOnAction);
            Hide();
        }

        private void SetupBar(Fighter fighter)
        {
            // Setup fighter name
            _fighterNameLabel.text = fighter.FighterName;

            // Setup life bar
            ProgressBarUIController.SetupProgressBar(
                _infosBarContainer.Q<VisualElement>(LIFE_BAR_UI_NAME),
                fighter.GetHealth(),
                fighter.GetMaxHealth()
            );

            // Setup statuses
            UpdateStatuses(fighter);
            fighter.onStatusApplied += (targetFighter, _appliedStatus) => UpdateStatuses(targetFighter);
            fighter.onStatusRemoved += (targetFighter, _removedStatus) => UpdateStatuses(targetFighter);
        }

        private void UpdateStatuses(Fighter fighter)
        {
            _statusesBar.Clear();
            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in fighter.GetStatuses())
            {
                VisualElement statusIconContainerRoot = _statusContainerTemplate.Instantiate();
                statusIconContainerRoot.AddToClassList(INFOS_BAR_STATUS_CONTAINER_ROOT_CLASSNAME);
                StatusContainerUIController.SetupStatusContainer(statusIconContainerRoot, status.Key);

                LongHoverEventController<VisualElement> longHoverEventController = new(statusIconContainerRoot);
                longHoverEventController.onElementLongHovered += (_) =>
                {
                    _statusDetailsOverlayController.SetStatus(status.Key, status.Value.duration);
                    _statusDetailsOverlayController.ShowOverlay(followMouse: true);
                };
                longHoverEventController.onElementLongUnhovered += (_) => _statusDetailsOverlayController.HideOverlay();

                _statusesBar.Add(statusIconContainerRoot);
            }
        }

        private void Display()
        {
            _infosBarContainer.RemoveFromClassList(INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME);
            _infosBarContainer.pickingMode = PickingMode.Position;
        }

        private void Hide()
        {
            _infosBarContainer.AddToClassList(INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME);
            _infosBarContainer.pickingMode = PickingMode.Ignore;
        }

        private void OnFightLoaded(Fighter[] allies, Fighter[] enemies)
        {
            List<Fighter> list = allies.ToList();
            list
                .Concat(enemies)
                .ToList()
                .ForEach(fighter =>
                {
                    fighter.onDamageReceived += (targetFighter, _, _, _) => StartCoroutine(DisplayForSeconds(targetFighter));
                    fighter.onHealReceived += (targetFighter, _, _) => StartCoroutine(DisplayForSeconds(targetFighter));
                    fighter.onStatusApplied += (targetFighter, _) => StartCoroutine(DisplayForSeconds(targetFighter));
                    fighter.onStatusRemoved += (targetFighter, _) => StartCoroutine(DisplayForSeconds(targetFighter));
                    fighter.onFighterDied += (targetFighter) => StartCoroutine(DisplayForSeconds(targetFighter));
                });

            TypedWorldMouseInteractor<Fighter> fightersMouseInteractor = new();
            fightersMouseInteractor.onHovered += OnFighterHovered;
            fightersMouseInteractor.onUnhovered += OnFighterUnhovered;

            TypedWorldMouseInteractor<FightCell> fightCellsMouseInteractor = new();
            fightCellsMouseInteractor.onHovered += (cell) => { if (cell.Fighter != null) OnFighterHovered(cell.Fighter); };
            fightCellsMouseInteractor.onUnhovered += (cell) => { if (cell.Fighter != null) OnFighterUnhovered(cell.Fighter); };
        }

        private void OnFightEnded(Fighter[] allies, Fighter[] enemies)
        {
            _infosBarContainer.RemoveFromHierarchy();
        }

        #region UXML UI Names & Classes

        private const string INFOS_BAR_CONTAINER_UI_NAME = "FighterInfosBarContainer";
        private const string FIGHTER_NAME_LABEL_UI_NAME = "FighterNameLabel";
        private const string LIFE_BAR_UI_NAME = "LifeBar";
        private const string STATUSES_BAR_UI_NAME = "StatusesBar";
        private const string INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME = "fighterInfoBarContainerHidden";
        private const string INFOS_BAR_STATUS_CONTAINER_ROOT_CLASSNAME = "fighterInfoBarStatusIconContainerRoot";

        #endregion
    }
}