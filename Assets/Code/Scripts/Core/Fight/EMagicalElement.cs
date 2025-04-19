using System;
using UnityEngine;

namespace FrostfallSaga.Core.Fight
{
    public enum EMagicalElement
    {
        FIRE,
        ICE,
        LIGHTNING,
        EARTH,
        LIGHT,
        DARKNESS
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
                _ => throw new InvalidOperationException("Unknown magical element."),
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
                _ => throw new InvalidOperationException("Unknown magical element."),
            };
        }

        public static Color GetUIColor(this EMagicalElement magicalElement)
        {
            return magicalElement switch
            {
                EMagicalElement.FIRE => new Color(202f, 103f, 2f, 255f) / 255f,
                EMagicalElement.ICE => new Color(173f, 232f, 244f, 255f) / 255f,
                EMagicalElement.LIGHTNING => new Color(76f, 201f, 240f, 255f) / 255f,
                EMagicalElement.EARTH => new Color(111f, 69f, 24f, 255f) / 255f,
                EMagicalElement.LIGHT => new Color(248f, 249f, 250f, 255f) / 255f,
                EMagicalElement.DARKNESS => new Color(60f, 9f, 108f, 255f) / 255f,
                _ => throw new InvalidOperationException("Unknown magical element."),
            };
        }
    }
}