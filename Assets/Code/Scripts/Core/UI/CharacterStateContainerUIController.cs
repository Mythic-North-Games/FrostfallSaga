using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    /// <summary>
    /// Responsible for displaying the health and dead state of a character.
    /// </summary>
    public static class CharacterStateContainerUIController
    {

        #region UXML Names and classes
        private static readonly string FIGHTER_ICON_UI_NAME = "FighterIcon";
        private static readonly string HEALTH_PROGRESS_ROOT_UI_NAME = "HealthProgress";
        private static readonly string DEAD_ICON_CONTAINER_UI_NAME = "DeadIconContainer";
        #endregion

        public static void Setup(
            VisualElement root,
            Sprite diamondIcon,
            int currentHealth,
            int maxHealth,
            bool displayValueLabel = false
        )
        {
            root.Q<VisualElement>(FIGHTER_ICON_UI_NAME).style.backgroundImage = new(diamondIcon);
            root.Q<VisualElement>(DEAD_ICON_CONTAINER_UI_NAME).visible = currentHealth <= 0;
            UpdateHealth(root, currentHealth, maxHealth, displayValueLabel);
        }

        public static void UpdateHealth(
            VisualElement root,
            int currentHealth,
            int maxHealth,
            bool displayValueLabel = false
        )
        {
            root.Q<VisualElement>(DEAD_ICON_CONTAINER_UI_NAME).visible = currentHealth <= 0;
            ProgressBarUIController.SetupProgressBar(
                root.Q<VisualElement>(HEALTH_PROGRESS_ROOT_UI_NAME),
                currentHealth,
                maxHealth,
                invertProgress: true,
                displayValueLabel: displayValueLabel
            );
        }
    }
}