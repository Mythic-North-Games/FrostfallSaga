using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public static class ProgressBarUIController
    {
        public static void SetupProgressBar(
            VisualElement root,
            float value,
            float maxValue,
            bool invertProgress = false,
            bool adjustWidth = true,
            bool adjustHeight = false,
            bool displayValueLabel = true,
            bool displayMaxValueLabel = true,
            Color customColor = default
        )
        {
            Label progressBarValueLabel = root.Q<Label>(PROGRESS_BAR_LABEL_UI_NAME);
            if (displayValueLabel)
            {
                string valueLabelText = value.ToString() + (displayMaxValueLabel ? $" <b>/</b> {maxValue}" : "");
                progressBarValueLabel.text = valueLabelText;
            }
            else if (progressBarValueLabel != null)
            {
                progressBarValueLabel.text = "";
            }

            float progress = value / maxValue * 100;
            if (invertProgress)
            {
                progress = 100 - progress;
            }

            if (adjustWidth)
            {
                root.Q<VisualElement>(PROGRESS_BAR_FILL_UI_NAME).style.width = new Length(
                    progress,
                    LengthUnit.Percent
                );
            }
            else if (adjustHeight)
            {
                root.Q<VisualElement>(PROGRESS_BAR_FILL_UI_NAME).style.height = new Length(
                    progress,
                    LengthUnit.Percent
                );
            }

            if (customColor != default)
            {
                root.Q<VisualElement>(PROGRESS_BAR_FILL_UI_NAME).style.backgroundColor = customColor;
            }
        }

        #region UXML UI Names & Classes

        private static readonly string PROGRESS_BAR_FILL_UI_NAME = "ProgressBarFill";
        private static readonly string PROGRESS_BAR_LABEL_UI_NAME = "ProgressBarLabel";

        #endregion
    }
}