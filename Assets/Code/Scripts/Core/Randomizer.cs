using System;

namespace FrostfallSaga.Core
{
    public static class Randomizer
    {
        private static readonly Random _randomizer = new();

        public static int GetRandomIntBetween(int min, int max)
        {
            return _randomizer.Next(min, max);
        }

        public static bool GetBooleanOnChance(float chance)
        {
            return _randomizer.NextDouble() < chance;
        }

        public static T GetRandomElementFromArray<T>(T[] array)
        {
            return array[_randomizer.Next(0, array.Length)];
        }
    }
}