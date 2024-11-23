using System;
using System.Collections.Generic;


namespace FrostfallSaga.Core
{
    [Serializable]
    public class SElementToValue<T, U>
    {
        public T element;
        public U value;

        public SElementToValue(T element, U value)
        {
            this.element = element;
            this.value = value;
        }

        public static Dictionary<T, U> GetDictionaryFromArray(SElementToValue<T, U>[] magicalElementToValues)
        {
            Dictionary<T, U> dictionary = new();
            foreach (var magicalElementToValue in magicalElementToValues)
            {
                dictionary.Add(magicalElementToValue.element, magicalElementToValue.value);
            }
            return dictionary;
        }
    }
}