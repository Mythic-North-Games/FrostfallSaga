using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public static class ProgressBarUIController
    {
        #region UXML UI Names & Classes
        private static readonly string PROGRESS_BAR_FILL_UI_NAME = "ProgressBarFill";
        private static readonly string PROGRESS_BAR_LABEL_UI_NAME = "ProgressBarLabel";
        #endregion

        public static void SetupProgressBar(VisualElement root, float value, float maxValue)
        {
            root.Q<Label>(PROGRESS_BAR_LABEL_UI_NAME).text = $"{value}/{maxValue}";
            root.Q<VisualElement>(PROGRESS_BAR_FILL_UI_NAME).style.width = new Length(
                (float)value / maxValue * 100,
                LengthUnit.Percent
            );
        }
    }
}