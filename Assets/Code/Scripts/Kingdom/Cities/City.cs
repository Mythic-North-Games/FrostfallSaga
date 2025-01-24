using UnityEngine;

namespace FrostfallSaga.Kingdom.Cities
{
    public class City : KingdomCellOccupier
    {
        [field: SerializeField] public CityConfigurationSO CityConfiguration { get; private set; }
        [field: SerializeField] public GameObject CityNamePanelAnchor { get; private set; }
        [field: SerializeField] public CityMouseEventsController MouseEventsController { get; private set; }

        private void Awake()
        {
            if (MouseEventsController == null)
            {
                MouseEventsController = GetComponent<CityMouseEventsController>();
            }
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