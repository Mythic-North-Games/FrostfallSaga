using System;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Kingdom
{
    [CreateAssetMenu(fileName = "PostFightData", menuName = "ScriptableObjects/Fight/PostFightData", order = 0)]
    public class PostFightDataSO : ScriptableObject
    {
        public List<SElementToValue<string, PostFightFighterState>> alliesState;
        public List<SElementToValue<string, PostFightFighterState>> enemiesState;
        public bool enabled = false;

        public bool AlliesHaveWon()
        {
            if (alliesState == null || alliesState.Count == 0 || enemiesState == null || enemiesState.Count == 0)
            {
                throw new Exception("No post fight state set. Can't determine whether the allies have won or not.");
            }

            Dictionary<string, PostFightFighterState> alliesStateDict = SElementToValue<string, PostFightFighterState>.GetDictionaryFromArray(alliesState.ToArray());
            foreach (PostFightFighterState allyState in alliesStateDict.Values)
            {
                if (allyState.lastingHealth > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
