using System;
using System.Collections.Generic;

namespace FrostfallSaga.Utils
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

        public static Dictionary<T, U> GetDictionaryFromArray(SElementToValue<T, U>[] elementToValues)
        {
            Dictionary<T, U> dictionary = new();
            foreach (SElementToValue<T, U> magicalElementToValue in elementToValues)
                dictionary.Add(magicalElementToValue.element, magicalElementToValue.value);
            return dictionary;
        }

        public static SElementToValue<T, U>[] GetArrayFromDictionary(Dictionary<T, U> dictionary)
        {
            SElementToValue<T, U>[] array = new SElementToValue<T, U>[dictionary.Count];
            int i = 0;
            foreach (KeyValuePair<T, U> kvp in dictionary)
            {
                array[i] = new SElementToValue<T, U>(kvp.Key, kvp.Value);
                i++;
            }
            return array;
        }
    }
}