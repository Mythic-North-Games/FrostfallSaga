using FrostfallSaga.Core;
using FrostfallSaga.Core.Cities;

namespace FrostfallSaga.Kingdom.UI
{
    public class EnterCityPanelUIController : BaseEnterInterestPointPanelUIController
    {
        protected override void OnInterestPointBuildingEncountered(AInterestPointConfigurationSO interestPointConfiguration)
        {
            if (interestPointConfiguration is not CityBuildingConfigurationSO) return;
            InstantiateAndSetupPanel(interestPointConfiguration);
        }
    }
}