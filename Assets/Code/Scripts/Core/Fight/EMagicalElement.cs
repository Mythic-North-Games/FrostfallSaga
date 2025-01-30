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
                EMagicalElement.FIRE => "Fire",
                EMagicalElement.ICE => "Ice",
                EMagicalElement.LIGHTNING => "Lightning",
                EMagicalElement.EARTH => "Earth",
                EMagicalElement.LIGHT => "Light",
                EMagicalElement.DARKNESS => "Darkness",
                _ => throw new System.InvalidOperationException("Unknown magical element."),
            };
        }
    }
}