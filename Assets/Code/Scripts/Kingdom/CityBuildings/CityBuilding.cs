using UnityEngine;
using FrostfallSaga.Core.Cities;

namespace FrostfallSaga.Kingdom.CityBuildings
{
    public class CityBuilding : KingdomCellOccupier
    {
        [field: SerializeField] public CityBuildingConfigurationSO CityBuildingConfiguration { get; private set; }
        [field: SerializeField] public GameObject CityNamePanelAnchor { get; private set; }
        [field: SerializeField] public CityMouseEventsController MouseEventsController { get; private set; }

        private void Awake()
        {
            if (MouseEventsController == null)
            {
                MouseEventsController = GetComponent<CityMouseEventsController>();
            }
            if (CityBuildingConfiguration == null)
            {
                Debug.LogError("CityBuildingConfiguration is not set. Won't be able to display city name.");
                return;
            }
            name = $"{CityBuildingConfiguration.Name}City";
        }

        private void Start()
        {
            if (cell != null)
            {
                transform.position = cell.GetCenter();
                if (!cell.HasOccupier()) cell.SetOccupier(this);
            }
        }
    }
}