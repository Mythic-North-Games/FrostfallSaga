using UnityEngine;
using UnityEngine.UIElements;

public static class StatContainerUIController
{
    #region UXLM elements name & classes

    private static readonly string STAT_ICON_UI_NAME = "StatIcon";
    private static readonly string STAT_LABEL_UI_NAME = "StatLabel";

    #endregion

    public static void SetupStatContainer(VisualElement root, Sprite icon, string statValue)
    {
        root.Q<VisualElement>(STAT_ICON_UI_NAME).style.backgroundImage = new(icon);
        root.Q<Label>(STAT_LABEL_UI_NAME).text = statValue;
    }
}