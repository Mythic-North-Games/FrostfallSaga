using System;

namespace FrostfallSaga.Utils
{
    /// <summary>
    ///     List of all the scenes that can be loaded in the game.
    ///     ! Always use this enumeration to load scenes.
    /// </summary>
    public enum EScenesName
    {
        TITLE_SCREEN,
        KINGDOM,
        FIGHT,
        CITY,
        DUNGEON
    }

    public static class ESceneNameMethods
    {
        public static string ToSceneString(this EScenesName sceneName)
        {
            return sceneName switch
            {
                EScenesName.TITLE_SCREEN => "TitleScreenScene",
                EScenesName.KINGDOM => "KingdomScene",
                EScenesName.FIGHT => "FightScene",
                EScenesName.CITY => "CityScene",
                EScenesName.DUNGEON => "DungeonScene",
                _ => throw new InvalidOperationException("Unknown scene.")
            };
        }
    }
}