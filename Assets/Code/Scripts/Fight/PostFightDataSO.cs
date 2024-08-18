using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    [CreateAssetMenu(fileName = "PostFightData", menuName = "ScriptableObjects/Fight/PostFightData", order = 0)]
    public class PostFightDataSO : ScriptableObject
    {
        public Dictionary<string, PostFightFighterState> alliesState;
        public Dictionary<string, PostFightFighterState> enemiesState;

        public bool AlliesHaveWon()
        {
            if (alliesState == null || alliesState.Count == 0 || enemiesState == null || enemiesState.Count == 0)
            {
                throw new Exception("No post fight state set. Can't determine whether the allies have won or not.");
            }

            foreach (PostFightFighterState allyState in alliesState.Values)
            {
                if (allyState.lastingHealth != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
