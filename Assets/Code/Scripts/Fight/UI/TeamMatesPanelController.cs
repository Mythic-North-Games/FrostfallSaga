using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI
{
    public class TeamMatesPanelController
    {
        #region UXML Names & classes
        private static readonly string PLAYING_FIGHTER_PROGRESS_ROOT_UI_NAME = "PlayingFighterProgress";
        private static readonly string MATE_1_PROGRESS_ROOT_UI_NAME = "Mate1Progress";
        private static readonly string MATE_2_PROGRESS_ROOT_UI_NAME = "Mate2Progress";
        private static readonly string FIGHTER_ICON_CONTAINER_UI_NAME = "FighterIcon";
        private static readonly string HEALTH_PROGRESS_ROOT_UI_NAME = "HealthProgress";
        private static readonly string MOVEMENT_POINTS_PROGRESS_ROOT_UI_NAME = "MovementPointsProgress";
        private static readonly string ACTION_POINTS_PROGRESS_ROOT_UI_NAME = "ActionPointsProgress";
        #endregion

        private readonly VisualElement _playingFighterIcon;
        private readonly VisualElement _mate1Icon;
        private readonly VisualElement _mate2Icon;
        private readonly VisualElement _playingFighterHealthProgressRoot;
        private readonly VisualElement _mate1HealthProgressRoot;
        private readonly VisualElement _mate2HealthProgressRoot;
        private readonly VisualElement _movementPointsProgressRoot;
        private readonly VisualElement _actionPointsProgressRoot;

        private readonly Color _movementPointsProgressColor;
        private readonly Color _actionPointsProgressColor;

        public TeamMatesPanelController(
            VisualElement root,
            Color movementPointsProgressColor,
            Color actionPointsProgressColor
        )
        {
            VisualElement playingFighterProgressRoot = root.Q<VisualElement>(PLAYING_FIGHTER_PROGRESS_ROOT_UI_NAME);
            _playingFighterIcon = playingFighterProgressRoot.Q<VisualElement>(FIGHTER_ICON_CONTAINER_UI_NAME);
            _playingFighterHealthProgressRoot = playingFighterProgressRoot.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME);

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

        public void Setup(Fighter playingFighter, Fighter mate1 = null, Fighter mate2 = null)
        {
            // Set icons
            _playingFighterIcon.style.backgroundImage = new(playingFighter.DiamondIcon);
            _mate1Icon.style.backgroundImage = new(mate1 != null ? mate1.DiamondIcon : null);
            _mate2Icon.style.backgroundImage = new(mate2 != null ? mate2.DiamondIcon : null);

            // Update playing fighter progress bars
            UpdateLifeProgress(playingFighter, _playingFighterHealthProgressRoot, true);
            UpdateMovementPointsProgress(playingFighter);
            UpdateActionPointsProgress(playingFighter);

            // Update mate progress bars
            UpdateLifeProgress(mate1, _mate1HealthProgressRoot, false);
            UpdateLifeProgress(mate2, _mate2HealthProgressRoot, false);

            // Register playing fighter events
            RegisterPlayingFighterEvents(playingFighter);

            // Register mate fighter events
            if (mate1 != null)
            {
                mate1.onDamageReceived += (_, _, _) => {
                    UpdateLifeProgress(mate1, _mate1HealthProgressRoot, false);
                };
                mate1.onHealReceived += (_, _, _) => UpdateLifeProgress(mate1, _mate1HealthProgressRoot, false);
            }
            if (mate2 != null)
            {
                mate2.onDamageReceived += (_, _, _) => UpdateLifeProgress(mate2, _mate2HealthProgressRoot, false);
                mate2.onHealReceived += (_, _, _) => UpdateLifeProgress(mate2, _mate2HealthProgressRoot, false);
            }
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

        private void RegisterPlayingFighterEvents(Fighter playingFighter)
        {
            playingFighter.onDamageReceived += (_, _, _) => {
                if (playingFighter.IsDead()) return;
                UpdateLifeProgress(playingFighter, _playingFighterHealthProgressRoot, true);
            };
            playingFighter.onHealReceived += (_, _, _) => UpdateLifeProgress(playingFighter, _playingFighterHealthProgressRoot, true);
            playingFighter.onActiveAbilityStarted += (_, _) => UpdateActionPointsProgress(playingFighter);
            playingFighter.onDirectAttackEnded += (_) => UpdateActionPointsProgress(playingFighter);
            playingFighter.onFighterMoved += (_) => UpdateMovementPointsProgress(playingFighter);
            playingFighter.onNonMagicalStatMutated += (_, _, _) =>
            {
                UpdateMovementPointsProgress(playingFighter);
                UpdateActionPointsProgress(playingFighter);
            };
            playingFighter.onPassiveAbilityApplied += (_, _) =>
            {
                UpdateMovementPointsProgress(playingFighter);
                UpdateActionPointsProgress(playingFighter);
            };
            playingFighter.onPassiveAbilityRemoved += (_, _) =>
            {
                UpdateMovementPointsProgress(playingFighter);
                UpdateActionPointsProgress(playingFighter);
            };
            playingFighter.onStatusApplied += (_, _) =>
            {
                UpdateMovementPointsProgress(playingFighter);
                UpdateActionPointsProgress(playingFighter);
            };
            playingFighter.onStatusRemoved += (_, _) =>
            {
                UpdateMovementPointsProgress(playingFighter);
                UpdateActionPointsProgress(playingFighter);
            };
        }
    }
}