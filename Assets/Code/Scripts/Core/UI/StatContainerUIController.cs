using UnityEngine;
using UnityEngine.UIElements;

public static class StatContainerUIController
{
    public static void SetupStatContainer(VisualElement root, Sprite icon, string statValue)
    {
        root.Q<VisualElement>(STAT_ICON_UI_NAME).style.backgroundImage = new(icon);
        root.Q<Label>(STAT_LABEL_UI_NAME).text = statValue;
    }

    public static void SetupStatContainer(VisualElement root, Sprite icon, string statValue, Justify justifyContent)
    {
        root.Q<VisualElement>(STAT_ICON_UI_NAME).style.backgroundImage = new(icon);
        root.Q<Label>(STAT_LABEL_UI_NAME).text = statValue;
        root.style.justifyContent = justifyContent;
    }

    public static void SetupStatContainer(
        VisualElement root,
        Sprite icon,
        string statValue,
        Color statValueColor,
        Justify justifyContent,
        Color iconTintColor = default
    )
    {
        root.Q<VisualElement>(STAT_ICON_UI_NAME).style.backgroundImage = new(icon);
        root.Q<VisualElement>(STAT_ICON_UI_NAME).style.unityBackgroundImageTintColor = iconTintColor;
        root.Q<Label>(STAT_LABEL_UI_NAME).text = statValue;
        root.Q<Label>(STAT_LABEL_UI_NAME).style.color = statValueColor;
        root.Q<Label>(STAT_LABEL_UI_NAME).style.unityTextOutlineColor = statValueColor;
        root.style.justifyContent = justifyContent;
    }

    #region UXLM elements name & classes

    private static readonly string STAT_ICON_UI_NAME = "StatIcon";
    private static readonly string STAT_LABEL_UI_NAME = "StatLabel";

    #endregion
}