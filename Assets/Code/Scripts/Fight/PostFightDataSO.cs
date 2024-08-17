using System;
using UnityEngine;

namespace FrostfallSaga.Fight
{
    [CreateAssetMenu(fileName = "PostFightData", menuName = "ScriptableObjects/Fight/PostFightData", order = 0)]
    public class PostFightDataSO : ScriptableObject
    {
        public PostFightFighterState[] alliesState;
        public PostFightFighterState[] enemiesState;

        public bool AlliesHaveWon()
        {
            if (alliesState == null || alliesState.Length == 0 || enemiesState == null || enemiesState.Length == 0)
            {
                throw new Exception("No post fight state set. Can't determine whether the allies have won or not.");
            }

            foreach (PostFightFighterState allyState in alliesState)
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
