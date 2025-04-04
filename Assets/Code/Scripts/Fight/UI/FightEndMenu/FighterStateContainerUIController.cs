using FrostfallSaga.Core.UI;
using FrostfallSaga.Fight.Fighters;
using UnityEngine.UIElements;

namespace FrostfallSaga.Fight.UI.FightEndMenu
{
    /// <summary>
    ///     Responsible for controlling the fight end menu.
    /// </summary>
    public static class FighterStateContainerUIController
    {
        #region UXML Names and classes
        private static readonly string FIGHTER_ICON_UI_NAME = "FighterIcon";
        private static readonly string HEALTH_PROGRESS_ROOT_UI_NAME = "HealthProgress";
        private static readonly string DEAD_ICON_CONTAINER_UI_NAME = "DeadIconContainer";
        #endregion

        public static void Setup(Fighter fighter, VisualElement root)
        {
            root.Q<VisualElement>(FIGHTER_ICON_UI_NAME).style.backgroundImage = new(fighter.DiamondIcon);
            root.Q<VisualElement>(DEAD_ICON_CONTAINER_UI_NAME).visible = fighter.IsDead();
            ProgressBarUIController.SetupProgressBar(
                root.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME),
                fighter == null ? 1 : fighter.GetHealth(),
                fighter == null ? 1 : fighter.GetMaxHealth(),
                invertProgress: true,
                displayValueLabel: false
            );
        }
    }
}