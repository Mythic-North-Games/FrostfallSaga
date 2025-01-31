namespace FrostfallSaga.Core
{
    public enum EScenesName
    {
        Kingdom,
        Fight,
        City
    }

    public static class ESceneNameMethods
    {
        public static string ToString(this EScenesName sceneName)
        {
            return sceneName switch
            {
                EScenesName.Kingdom => "KingdomScene",
                EScenesName.Fight => "FightScene",
                EScenesName.City => "CityScene",
                _ => throw new System.InvalidOperationException("Unknown scene."),
            };
        }
    }
}