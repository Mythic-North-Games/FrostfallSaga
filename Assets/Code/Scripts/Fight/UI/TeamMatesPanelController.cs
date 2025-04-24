using FrostfallSaga.Core.Fight;
using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Statuses;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class TeamMatesPanelController
    {
        private readonly Color _actionPointsProgressColor;
        private readonly VisualElement _actionPointsProgressRoot;
        private readonly VisualElement _mate1HealthProgressRoot;
        private readonly VisualElement _mate1Icon;
        private readonly VisualElement _mate2HealthProgressRoot;
        private readonly VisualElement _mate2Icon;

        private readonly Color _movementPointsProgressColor;
        private readonly VisualElement _movementPointsProgressRoot;
        private readonly VisualElement _playingFighterHealthProgressRoot;

        private readonly VisualElement _playingFighterIcon;
        private Fighter _mate1;
        private Fighter _mate2;

        private Fighter _playingFighter;

        public TeamMatesPanelController(
            VisualElement root,
            Color movementPointsProgressColor,
            Color actionPointsProgressColor
        )
        {
            VisualElement playingFighterProgressRoot = root.Q<VisualElement>(PLAYING_FIGHTER_PROGRESS_ROOT_UI_NAME);
            _playingFighterIcon = playingFighterProgressRoot.Q<VisualElement>(FIGHTER_ICON_CONTAINER_UI_NAME);
            _playingFighterHealthProgressRoot =
                playingFighterProgressRoot.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME);

            VisualElement mate1ProgressRoot = root.Q<VisualElement>(MATE_1_PROGRESS_ROOT_UI_NAME);
            _mate1Icon = mate1ProgressRoot.Q<VisualElement>(FIGHTER_ICON_CONTAINER_UI_NAME);
            _mate1HealthProgressRoot = mate1ProgressRoot.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME);

            VisualElement mate2ProgressRoot = root.Q<VisualElement>(MATE_2_PROGRESS_ROOT_UI_NAME);
            _mate2Icon = mate2ProgressRoot.Q<VisualElement>(FIGHTER_ICON_CONTAINER_UI_NAME);
            _mate2HealthProgressRoot = mate2ProgressRoot.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME);

            _movementPointsProgressRoot = root.Q<VisualElement>(MOVEMENT_POINTS_PROGRESS_ROOT_UI_NAME);
            _actionPointsProgressRoot = root.Q<VisualElement>(ACTION_POINTS_PROGRESS_ROOT_UI_NAME);

            _movementPointsProgressColor = movementPointsProgressColor;
            _actionPointsProgressColor = actionPointsProgressColor;
        }

        public void Update(Fighter playingFighter, Fighter mate1 = null, Fighter mate2 = null)
        {
            // Unregister previous fighter events for proper update
            if (_playingFighter != null) UnregisterFightersEvents();

            // Update current fighters
            _playingFighter = playingFighter;
            _mate1 = mate1;
            _mate2 = mate2;

            // Set icons
            _playingFighterIcon.style.backgroundImage = new(_playingFighter.DiamondIcon);
            _mate1Icon.style.backgroundImage = new(_mate1 != null ? _mate1.DiamondIcon : null);
            _mate2Icon.style.backgroundImage = new(mate2 != null ? mate2.DiamondIcon : null);

            // Update playing fighter progress bars
            UpdateLifeProgress(_playingFighter, _playingFighterHealthProgressRoot, true);
            UpdateMovementPointsProgress(_playingFighter);
            UpdateActionPointsProgress(_playingFighter);

            // Update mate progress bars
            UpdateLifeProgress(_mate1, _mate1HealthProgressRoot, false);
            UpdateLifeProgress(mate2, _mate2HealthProgressRoot, false);

            // Register fighter events to update progress bars during turn
            RegisterFighterEvents();
        }

        private void UpdateLifeProgress(Fighter fighter, VisualElement progressRoot, bool displayLabel)
        {
            ProgressBarUIController.SetupProgressBar(
                progressRoot,
                fighter == null ? 1 : fighter.GetHealth(),
                fighter == null ? 1 : fighter.GetMaxHealth(),
                invertProgress: true,
                displayValueLabel: displayLabel
            );
        }

        private void UpdateMovementPointsProgress(Fighter fighter)
        {
            ProgressBarUIController.SetupProgressBar(
                _movementPointsProgressRoot,
                fighter.GetMovePoints(),
                fighter.GetMaxMovePoints(),
                adjustWidth: false,
                adjustHeight: true,
                displayMaxValueLabel: false,
                customColor: _movementPointsProgressColor
            );
        }

        private void UpdateActionPointsProgress(Fighter fighter)
        {
            ProgressBarUIController.SetupProgressBar(
                _actionPointsProgressRoot,
                fighter.GetActionPoints(),
                fighter.GetMaxActionPoints(),
                adjustWidth: false,
                adjustHeight: true,
                displayMaxValueLabel: false,
                customColor: _actionPointsProgressColor
            );
        }

        private void OnPlayingFighterReceivedDamage(
            Fighter playingFighter,
            int _value,
            bool _isMasterstroke,
            EMagicalElement? _magicalElement
        )
        {
            UpdateLifeProgress(_playingFighter, _playingFighterHealthProgressRoot, true);
        }

        private void OnPlayingFighterHealed(
            Fighter playingFighter,
            int _value,
            bool _isMasterstroke
        )
        {
            UpdateLifeProgress(_playingFighter, _playingFighterHealthProgressRoot, true);
        }

        private void OnMateFighterReceivedDamage(
            Fighter mateFighter,
            int _value,
            bool _isMasterstroke,
            EMagicalElement? _magicalElement
        )
        {
            if (mateFighter == _mate1) UpdateLifeProgress(_mate1, _mate1HealthProgressRoot, false);
            else if (mateFighter == _mate2) UpdateLifeProgress(_mate2, _mate2HealthProgressRoot, false);
        }

        private void OnMateFighterHealed(Fighter mateFighter, int _value, bool _isMasterstroke)
        {
            if (mateFighter == _mate1) UpdateLifeProgress(_mate1, _mate1HealthProgressRoot, false);
            else if (mateFighter == _mate2) UpdateLifeProgress(_mate2, _mate2HealthProgressRoot, false);
        }

        private void OnPlayingFighterActiveAbilityStarted(Fighter playingFighter, ActiveAbilitySO ability)
        {
            UpdateActionPointsProgress(playingFighter);
        }

        private void OnPlayingFighterDirectAttackStarted(Fighter playingFighter)
        {
            UpdateActionPointsProgress(playingFighter);
        }

        private void OnPlayingFighterMoved(Fighter playingFighter)
        {
            UpdateMovementPointsProgress(playingFighter);
        }

        private void OnPlayingFighterNonMagicalStatMutated(
            Fighter playingFighter,
            EFighterMutableStat stat,
            float value
        )
        {
            UpdateMovementPointsProgress(playingFighter);
            UpdateActionPointsProgress(playingFighter);
        }

        private void OnPlayingFighterPassiveAbilityChanged(Fighter playingFighter, PassiveAbilitySO ability)
        {
            UpdateMovementPointsProgress(playingFighter);
            UpdateActionPointsProgress(playingFighter);
        }

        private void OnPlayingFighterStatusChanged(Fighter playingFighter, AStatus status)
        {
            UpdateMovementPointsProgress(playingFighter);
            UpdateActionPointsProgress(playingFighter);
        }

        #region UXML Names & classes

        private static readonly string PLAYING_FIGHTER_PROGRESS_ROOT_UI_NAME = "PlayingFighterProgress";
        private static readonly string MATE_1_PROGRESS_ROOT_UI_NAME = "Mate1Progress";
        private static readonly string MATE_2_PROGRESS_ROOT_UI_NAME = "Mate2Progress";
        private static readonly string FIGHTER_ICON_CONTAINER_UI_NAME = "FighterIcon";
        private static readonly string HEALTH_PROGRESS_ROOT_UI_NAME = "HealthProgress";
        private static readonly string MOVEMENT_POINTS_PROGRESS_ROOT_UI_NAME = "MovementPointsProgress";
        private static readonly string ACTION_POINTS_PROGRESS_ROOT_UI_NAME = "ActionPointsProgress";

        #endregion

        #region Fighter events registration

        private void RegisterFighterEvents()
        {
            _playingFighter.onDamageReceived += OnPlayingFighterReceivedDamage;
            _playingFighter.onHealReceived += OnPlayingFighterHealed;

            _playingFighter.onActiveAbilityStarted += OnPlayingFighterActiveAbilityStarted;
            _playingFighter.onDirectAttackStarted += OnPlayingFighterDirectAttackStarted;

            _playingFighter.onFighterMoved += OnPlayingFighterMoved;

            _playingFighter.onNonMagicalStatMutated += OnPlayingFighterNonMagicalStatMutated;

            _playingFighter.onPassiveAbilityApplied += OnPlayingFighterPassiveAbilityChanged;
            _playingFighter.onPassiveAbilityRemoved += OnPlayingFighterPassiveAbilityChanged;

            _playingFighter.onStatusApplied += OnPlayingFighterStatusChanged;
            _playingFighter.onStatusRemoved += OnPlayingFighterStatusChanged;

            // Register mate fighter events
            if (_mate1 != null)
            {
                _mate1.onDamageReceived += OnMateFighterReceivedDamage;
                _mate1.onHealReceived += OnMateFighterHealed;
            }

            if (_mate2 != null)
            {
                _mate2.onDamageReceived += OnMateFighterReceivedDamage;
                _mate2.onHealReceived += OnMateFighterHealed;
            }
        }

        private void UnregisterFightersEvents()
        {
            _playingFighter.onDamageReceived -= OnPlayingFighterReceivedDamage;
            _playingFighter.onHealReceived -= OnPlayingFighterHealed;

            _playingFighter.onActiveAbilityStarted -= OnPlayingFighterActiveAbilityStarted;
            _playingFighter.onDirectAttackStarted -= OnPlayingFighterDirectAttackStarted;

            _playingFighter.onFighterMoved -= OnPlayingFighterMoved;

            _playingFighter.onNonMagicalStatMutated -= OnPlayingFighterNonMagicalStatMutated;

            _playingFighter.onPassiveAbilityApplied -= OnPlayingFighterPassiveAbilityChanged;
            _playingFighter.onPassiveAbilityRemoved -= OnPlayingFighterPassiveAbilityChanged;

            _playingFighter.onStatusApplied -= OnPlayingFighterStatusChanged;
            _playingFighter.onStatusRemoved -= OnPlayingFighterStatusChanged;

            // Unregister mate fighter events
            if (_mate1 != null)
            {
                _mate1.onDamageReceived -= OnMateFighterReceivedDamage;
                _mate1.onHealReceived -= OnMateFighterHealed;
            }

            if (_mate2 != null)
            {
                _mate2.onDamageReceived -= OnMateFighterReceivedDamage;
                _mate2.onHealReceived -= OnMateFighterHealed;
            }
        }

        #endregion
    }
}