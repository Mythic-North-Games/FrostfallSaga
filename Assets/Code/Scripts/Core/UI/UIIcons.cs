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
        BLEED,
        PARALYSIS,
        WEAKNESS,
        SLOWNESS
    }

    public static class UIIconsMethods
    {
        public static string GetIconResourceName(this UIIcons icon)
        {
            return icon switch
            {
                UIIcons.PHYSICAL_DAMAGE => "swordBrandish",
                UIIcons.PHYSICAL_RESISTANCE => "vikingShield",
                UIIcons.RANGE => "range",
                UIIcons.ACTION_POINTS_COST => "energy",
                UIIcons.BLEED => "bleed",
                UIIcons.PARALYSIS => "bleed",
                UIIcons.WEAKNESS => "bleed",
                UIIcons.SLOWNESS => "bleed",
                _ => string.Empty
            };
        }
    }
}