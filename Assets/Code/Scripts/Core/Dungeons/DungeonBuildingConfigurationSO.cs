using UnityEngine;

namespace FrostfallSaga.Core.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonBuildingConfiguration",
        menuName = "ScriptableObjects/Dungeons/DungeonBuildingConfiguration", order = 0)]
    public class DungeonBuildingConfigurationSO : AInterestPointConfigurationSO
    {
        [field: SerializeField] public DungeonConfigurationSO DungeonConfiguration { get; private set; }
        [field: SerializeField] public AudioClip EnterDungeonSound { get; private set; }
    }
}