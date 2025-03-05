namespace FrostfallSaga.Core.Fight
{
    public enum EMagicalElement
    {
        FIRE,
        ICE,
        LIGHTNING,
        EARTH,
        LIGHT,
        DARKNESS,
    }

    public static class EMagicalElementMethods
    {
        public static string ToUIString(this EMagicalElement magicalElement)
        {
            return magicalElement switch
            {
                EMagicalElement.FIRE => "<b>Fire</b>",
                EMagicalElement.ICE => "<b>Ice</b>",
                EMagicalElement.LIGHTNING => "<b>Lightning</b>",
                EMagicalElement.EARTH => "<b>Earth</b>",
                EMagicalElement.LIGHT => "<b>Light</b>",
                EMagicalElement.DARKNESS => "<b>Darkness</b>",
                _ => throw new System.InvalidOperationException("Unknown magical element."),
            };
        }

        public static string GetIconResourceName(this EMagicalElement magicalElement)
        {
            return magicalElement switch
            {
                EMagicalElement.FIRE => "fire",
                EMagicalElement.ICE => "ice",
                EMagicalElement.LIGHTNING => "lightning",
                EMagicalElement.EARTH => "earth",
                EMagicalElement.LIGHT => "light",
                EMagicalElement.DARKNESS => "darkness",
                _ => throw new System.InvalidOperationException("Unknown magical element."),
            };
        }
    }
}