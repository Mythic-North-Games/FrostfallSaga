namespace FrostfallSaga.Core
{
    public enum EScenesName
    {
        KINGDOM,
        FIGHT,
        CITY
    }

    public static class ESceneNameMethods
    {
        public static string ToSceneString(this EScenesName sceneName)
        {
            return sceneName switch
            {
                EScenesName.KINGDOM => "KingdomScene",
                EScenesName.FIGHT => "FightScene",
                EScenesName.CITY => "CityScene",
                _ => throw new System.InvalidOperationException("Unknown scene."),
            };
        }
    }
}