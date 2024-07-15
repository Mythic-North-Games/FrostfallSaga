using UnityEngine;

namespace FrostfallSaga.Fight
{
    [CreateAssetMenu(fileName = "PreFightData", menuName = "ScriptableObjects/Fight/PreFightData", order = 0)]
    public class PreFightDataSO : ScriptableObject
    {
        public FighterSetup[] alliesFighterSetup;
        public FighterSetup[] enemiesFighterSetup;
    }
}
