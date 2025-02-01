using UnityEngine;

namespace FrostfallSaga.Core.Dungeons
{
    [CreateAssetMenu(fileName = "DungeonBuildingConfigurationSO", menuName = "ScriptableObjects/Dungeons/DungeonBuildingConfigurationSO", order = 0)]
    public class DungeonBuildingConfigurationSO : AInterestPointConfigurationSO
    {
        [field: SerializeField] public Sprite DungeonPreview { get; private set; }
    }
}