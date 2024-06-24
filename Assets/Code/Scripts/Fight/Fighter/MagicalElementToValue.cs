using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.Effects;


namespace FrostfallSaga.Fight.Fighters
{
    [Serializable]
    public class MagicalElementToValue
    {
        public EMagicalElement magicalElement;
        public int value;

        public static Dictionary<EMagicalElement, int> GetDictionaryFromArray(MagicalElementToValue[] magicalElementToValues)
        {
            Dictionary<EMagicalElement, int> dictionary = new Dictionary<EMagicalElement, int>();
            foreach (var magicalElementToValue in magicalElementToValues)
            {
                dictionary.Add(magicalElementToValue.magicalElement, magicalElementToValue.value);
            }
            return dictionary;
        }
    }
}