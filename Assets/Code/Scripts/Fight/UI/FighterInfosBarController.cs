using System.Linq;
using System.Collections;
using System.Collections.Generic;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class FighterInfosBarController : BaseUIController
    {
        #region UXML UI Names & Classes
        private static readonly string INFOS_BAR_CONTAINER_UI_NAME = "FighterInfosBarContainer";
        private static readonly string FIGHTER_NAME_LABEL_UI_NAME = "FighterNameLabel";
        private static readonly string LIFE_BAR_UI_NAME = "LifeBar";
        private static readonly string STATUSES_BAR_UI_NAME = "StatusesBar";

        private static readonly string INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME = "fighterInfoBarContainerHidden";
        private static readonly string INFOS_BAR_STATUS_CONTAINER_ROOT_CLASSNAME = "fighterInfoBarStatusIconContainerRoot";
        #endregion

        [SerializeField] private VisualTreeAsset _statusContainerTemplate;
        [SerializeField] private FightManager _fightManager;
        [SerializeField] private FightLoader _fightLoader;
        [SerializeField] private FightersOrderTimelineController _timelineController;
        [SerializeField] private float _displayDurationOnAction = 2f;

        private VisualElement _infosBarContainer;
        private Label _fighterNameLabel;
        private VisualElement _statusesBar;

        private void Start()
        {
            _infosBarContainer = _uiDoc.rootVisualElement.Q<VisualElement>(INFOS_BAR_CONTAINER_UI_NAME);
            _fighterNameLabel = _infosBarContainer.Q<Label>(FIGHTER_NAME_LABEL_UI_NAME);
            _statusesBar = _infosBarContainer.Q<VisualElement>(STATUSES_BAR_UI_NAME);
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
            fighter.onStatusApplied += (fighter, _appliedStatus) => UpdateStatuses(fighter);
            fighter.onStatusRemoved += (fighter, _removedStatus) => UpdateStatuses(fighter);
        }

        private void UpdateStatuses(Fighter fighter)
        {
            _statusesBar.Clear();
            foreach (KeyValuePair<AStatus, (bool isActive, int duration)> status in fighter.GetStatuses())
            {
                VisualElement statusIconContainerRoot = _statusContainerTemplate.Instantiate();
                statusIconContainerRoot.AddToClassList(INFOS_BAR_STATUS_CONTAINER_ROOT_CLASSNAME);
                StatusContainerUIController.SetupStatusContainer(statusIconContainerRoot, status.Key);
                _statusesBar.Add(statusIconContainerRoot);
            }
        }

        private void Display()
        {
            _infosBarContainer.RemoveFromClassList(INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME);
        }

        private void Hide()
        {
            _infosBarContainer.AddToClassList(INFOS_BAR_CONTAINER_HIDDEN_CLASSNAME);
        }

        private void OnFightLoaded(Fighter[] allies, Fighter[] enemies)
        {
            allies
                .ToList()
                .Concat(enemies)
                .ToList()
                .ForEach(fighter =>
                {
                    fighter.FighterMouseEventsController.OnElementHover += OnFighterHovered;
                    fighter.FighterMouseEventsController.OnElementUnhover += OnFighterUnhovered;
                    fighter.onDamageReceived += (fighter, _, _) => StartCoroutine(DisplayForSeconds(fighter));
                    fighter.onHealReceived += (fighter, _, _) => StartCoroutine(DisplayForSeconds(fighter));
                    fighter.onStatusApplied += (fighter, _) => StartCoroutine(DisplayForSeconds(fighter));
                    fighter.onStatusRemoved += (fighter, _) => StartCoroutine(DisplayForSeconds(fighter));
                    fighter.onFighterDied += (fighter) => StartCoroutine(DisplayForSeconds(fighter));
                });
        }

        private void OnFightEnded(Fighter[] _allies, Fighter[] _enemies)
        {
            _infosBarContainer.RemoveFromHierarchy();
        }

        #region Setup

        private void Awake()
        {
            if (_uiDoc == null) _uiDoc = GetComponent<UIDocument>();
            if (_uiDoc == null)
            {
                Debug.LogError("No UI Document to work with.");
                return;
            }

            if (_fightManager == null) _fightManager = FindObjectOfType<FightManager>();
            if (_fightManager == null)
            {
                Debug.LogError("No FightManager to work with. UI can't be updated dynamically.");
                return;
            }

            if (_fightLoader == null) _fightLoader = FindObjectOfType<FightLoader>();
            if (_fightLoader == null)
            {
                Debug.LogError("No FightLoader to work with. UI can't be updated dynamically.");
                return;
            }

            if (_timelineController == null) _timelineController = FindObjectOfType<FightersOrderTimelineController>();
            if (_timelineController == null)
            {
                Debug.LogError("No FightersOrderTimelineController to work with. UI can't be updated dynamically.");
                return;
            }

            if (_statusContainerTemplate == null)
            {
                Debug.LogError("No status container template to work with.");
                return;
            }

            _fightManager.onFightEnded += OnFightEnded;
            _fightLoader.OnFightLoaded += OnFightLoaded;
            _timelineController.onFighterHovered += OnFighterHovered;
            _timelineController.onFighterUnhovered += OnFighterUnhovered;
        }

        #endregion
    }
}