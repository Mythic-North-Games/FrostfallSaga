namespace FrostfallSaga.Core.UI
{
    /// <summary>
    /// Enum representing all the icons used in the UI. Used to get the resource name of the icon for easy
    /// dinamic loading.
    /// </summary>
    public enum UIIcons
    {
        PHYSICAL_DAMAGE,
        PHYSICAL_RESISTANCE,
        RANGE,
        ACTION_POINTS_COST,
    }

    public static class UIIconsMethods
    {
        public static string GetIconResourceName(this UIIcons icon)
        {
            return icon switch
            {
                UIIcons.PHYSICAL_DAMAGE => "physicalDamage",
                UIIcons.PHYSICAL_RESISTANCE => "physicalResistance",
                UIIcons.RANGE => "diamondOutline",
                UIIcons.ACTION_POINTS_COST => "actionPointsCost",
                _ => string.Empty
            };
        }
    }
}