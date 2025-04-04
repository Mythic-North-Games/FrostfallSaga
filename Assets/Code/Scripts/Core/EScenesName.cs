using System;

namespace FrostfallSaga.Core
{
    /// <summary>
    ///     List of all the scenes that can be loaded in the game.
    ///     ! Always use this enumeration to load scenes.
    /// </summary>
    public enum EScenesName
    {
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
                EScenesName.KINGDOM => "KingdomScene",
                EScenesName.FIGHT => "FightScene",
                EScenesName.CITY => "CityScene",
                EScenesName.DUNGEON => "DungeonScene",
                _ => throw new InvalidOperationException("Unknown scene.")
            };
        }
    }
}