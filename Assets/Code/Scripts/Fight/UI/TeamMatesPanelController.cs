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
        #region UXML Names & classes
        private static readonly string PLAYING_FIGHTER_CHARACTER_STATE_ROOT_UI_NAME = "PlayingFighterCharacterState";
        private static readonly string MATE_1_PROGRESS_CHARACTER_STATE_UI_NAME = "Mate1CharacterState";
        private static readonly string MATE_2_PROGRESS_CHARACTER_STATE_UI_NAME = "Mate2CharacterState";
        private static readonly string MOVEMENT_POINTS_PROGRESS_ROOT_UI_NAME = "MovementPointsProgress";
        private static readonly string ACTION_POINTS_PROGRESS_ROOT_UI_NAME = "ActionPointsProgress";
        #endregion

        private readonly VisualElement _playingFighterCharacterStateRoot;
        private readonly VisualElement _mate1CharacterStateRoot;
        private readonly VisualElement _mate2CharacterStateRoot;

        private readonly Color _actionPointsProgressColor;
        private readonly VisualElement _actionPointsProgressRoot;

        private readonly Color _movementPointsProgressColor;
        private readonly VisualElement _movementPointsProgressRoot;

        private Fighter _playingFighter;
        private Fighter _mate1;
        private Fighter _mate2;

        public TeamMatesPanelController(
            VisualElement root,
            Color movementPointsProgressColor,
            Color actionPointsProgressColor
        )
        {
            _playingFighterCharacterStateRoot = root.Q<VisualElement>(PLAYING_FIGHTER_CHARACTER_STATE_ROOT_UI_NAME);
            _mate1CharacterStateRoot = root.Q<VisualElement>(MATE_1_PROGRESS_CHARACTER_STATE_UI_NAME);
            _mate2CharacterStateRoot = root.Q<VisualElement>(MATE_2_PROGRESS_CHARACTER_STATE_UI_NAME);

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
            CharacterStateContainerUIController.Setup(
                _playingFighterCharacterStateRoot,
                _playingFighter.DiamondIcon,
                _playingFighter.GetHealth(),
                _playingFighter.GetMaxHealth()
            );
            if (mate1 != null)
            {
                CharacterStateContainerUIController.Setup(
                    _mate1CharacterStateRoot,
                    _mate1.DiamondIcon,
                    _mate1.GetHealth(),
                    _mate1.GetMaxHealth()
                );
            }
            else _mate1CharacterStateRoot.style.display = DisplayStyle.None;
            if (mate2 != null)
            {
                CharacterStateContainerUIController.Setup(
                    _mate2CharacterStateRoot,
                    _mate2.DiamondIcon,
                    _mate2.GetHealth(),
                    _mate2.GetMaxHealth()
                );
            }
            else _mate2CharacterStateRoot.style.display = DisplayStyle.None;

            // Update playing fighter progress bars
            UpdateLifeProgress(_playingFighter, _playingFighterCharacterStateRoot, !_playingFighter.IsFullLife());
            UpdateMovementPointsProgress(_playingFighter);
            UpdateActionPointsProgress(_playingFighter);

            // Update mate progress bars
            if (mate1 != null) UpdateLifeProgress(_mate1, _mate1CharacterStateRoot, false);
            if (mate2 != null) UpdateLifeProgress(_mate2, _mate2CharacterStateRoot, false);

            // Register fighter events to update progress bars during turn
            RegisterFighterEvents();
        }

        private void UpdateLifeProgress(Fighter fighter, VisualElement progressRoot, bool displayLabel)
        {
            CharacterStateContainerUIController.UpdateHealth(
                progressRoot,
                fighter == null ? 1 : fighter.GetHealth(),
                fighter == null ? 1 : fighter.GetMaxHealth(),
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
            UpdateLifeProgress(_playingFighter, _playingFighterCharacterStateRoot, !_playingFighter.IsFullLife());
        }

        private void OnPlayingFighterHealed(
            Fighter playingFighter,
            int _value,
            bool _isMasterstroke
        )
        {
            UpdateLifeProgress(_playingFighter, _playingFighterCharacterStateRoot, !_playingFighter.IsFullLife());
        }

        private void OnMateFighterReceivedDamage(
            Fighter mateFighter,
            int _value,
            bool _isMasterstroke,
            EMagicalElement? _magicalElement
        )
        {
            if (mateFighter == _mate1) UpdateLifeProgress(_mate1, _mate1CharacterStateRoot, false);
            else if (mateFighter == _mate2) UpdateLifeProgress(_mate2, _mate2CharacterStateRoot, false);
        }

        private void OnMateFighterHealed(Fighter mateFighter, int _value, bool _isMasterstroke)
        {
            if (mateFighter == _mate1) UpdateLifeProgress(_mate1, _mate1CharacterStateRoot, false);
            else if (mateFighter == _mate2) UpdateLifeProgress(_mate2, _mate2CharacterStateRoot, false);
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