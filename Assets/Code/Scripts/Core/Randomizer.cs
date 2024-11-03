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

        public static T GetRandomElementFromEnum<T>(T[] toExclude = null) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var randomIndex = _randomizer.Next(0, values.Length);

            if (toExclude != null)
            {
                while (Array.Exists(toExclude, element => element.Equals(values.GetValue(randomIndex))))
                {
                    randomIndex = _randomizer.Next(0, values.Length);
                }
            }

            return (T) values.GetValue(randomIndex);
        }
    }
}