using System;

namespace FrostfallSaga.Utils
{
    /// <summary>
    ///     List of all the object layers that can be used in the game.
    /// </summary>
    public enum ELayersName
    {
        DEFAULT,
        TRANSPARENT_FX,
        IGNORE_RAYCAST,
        CELL,
        WATER,
        UI,
        FIGHTER,
        FIGHTER_INTERACTABLE,
        ENTITY_VISUAL,
        BUILDING,
        HIGHLIGHT_CELL
    }

    public static class ELayersNameExtensions
    {
        public static int ToLayerInt(this ELayersName layerName)
        {
            return layerName switch
            {
                ELayersName.DEFAULT => 0,
                ELayersName.TRANSPARENT_FX => 1,
                ELayersName.IGNORE_RAYCAST => 2,
                ELayersName.CELL => 3,
                ELayersName.WATER => 4,
                ELayersName.UI => 5,
                ELayersName.FIGHTER => 6,
                ELayersName.FIGHTER_INTERACTABLE => 7,
                ELayersName.ENTITY_VISUAL => 8,
                ELayersName.BUILDING => 9,
                ELayersName.HIGHLIGHT_CELL => 10,
                _ => throw new InvalidOperationException("Unknown layer.")
            };
        }
    }
}