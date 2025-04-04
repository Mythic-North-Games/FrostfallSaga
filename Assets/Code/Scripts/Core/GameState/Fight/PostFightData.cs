using System;
using System.Collections.Generic;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Core.GameState.Fight
{
    [Serializable]
    public class PostFightData
    {
        public List<SElementToValue<string, PostFightFighterState>> enemiesState;
        public bool isActive;

        public bool AlliesHaveWon()
        {
            if (enemiesState == null || enemiesState.Count == 0)
                throw new Exception("No post fight state set. Can't determine whether the allies have won or not.");

            Dictionary<string, PostFightFighterState> enemiesStateDict =
                SElementToValue<string, PostFightFighterState>.GetDictionaryFromArray(
                    enemiesState.ToArray()
                );
            foreach (PostFightFighterState enemyState in enemiesStateDict.Values)
                if (enemyState.lastingHealth > 0)
                    return false;

            return true;
        }
    }
}