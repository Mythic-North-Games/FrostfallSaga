using System;
using System.Collections.Generic;

namespace FrostfallSaga.Fight.Fighters
{
    /// <summary>
    /// Because Unity can't serialize a dictionary, we need to use a custom class to serialize a dictionary.
    /// </summary>
    [Serializable]
    public class GodFavorsActionToInt
    {
        public EGodFavorsAction action;
        public int value;

        public static Dictionary<EGodFavorsAction, int> GetDictionaryFromArray(GodFavorsActionToInt[] godFavoriteActionToInts)
        {
            Dictionary<EGodFavorsAction, int> dictionary = new();
            foreach (var godFavoriteActionToInt in godFavoriteActionToInts)
            {
                dictionary.Add(godFavoriteActionToInt.action, godFavoriteActionToInt.value);
            }
            return dictionary;
        }
    }
}