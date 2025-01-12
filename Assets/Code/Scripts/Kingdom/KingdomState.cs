using FrostfallSaga.Kingdom.EntitiesGroups;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Kingdom
{
    public class KingdomState : MonoBehaviourPersistingSingleton<KingdomState>
    {
        public EntitiesGroupData heroGroupData;
        public EntitiesGroupData[] enemiesGroupsData;
    }
}
