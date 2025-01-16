using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight
{
    public class PreFightData : MonoBehaviourPersistingSingleton<PreFightData>
    {
        public FighterSetup[] alliesFighterSetup;
        public FighterSetup[] enemiesFighterSetup;
    }
}
