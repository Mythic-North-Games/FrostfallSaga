using FrostfallSaga.Core;
using FrostfallSaga.Core.Dungeons;

namespace FrostfallSaga.Kingdom.UI
{
    public class EnterDungeonPanelUIController : BaseEnterInterestPointPanelUIController
    {
        protected override void OnInterestPointBuildingEncountered(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not DungeonBuildingConfigurationSO) return;
            InstantiateAndSetupPanel(interestPointConfiguration);
        }
    }
}