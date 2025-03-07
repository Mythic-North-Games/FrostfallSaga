using UnityEngine;

namespace FrostfallSaga.Core.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonConfiguration", menuName = "ScriptableObjects/Dungeons/DungeonConfiguration",
        order = 0)]
    public class DungeonConfigurationSO : ScriptableObject
    {
        [field: SerializeField] public DungeonFightConfiguration BossFightConfiguration { get; private set; }
        [field: SerializeField] public DungeonFightConfiguration[] PreBossFightConfigurations { get; private set; }
    }
}