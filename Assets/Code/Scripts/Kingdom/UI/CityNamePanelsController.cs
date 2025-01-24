using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Utils.UI;
using FrostfallSaga.Kingdom.CityBuildings;

namespace FrostfallSaga.Kingdom.UI
{
    public class CityNamePanelController : BaseUIController
    {
        private static readonly string CITY_LABEL_UI_NAME = "CityNameLabel";

        [field: SerializeField] public VisualTreeAsset CityNamePanelTemplate { get; private set; }
        [field: SerializeField] public Vector2 DisplayOffset { get; private set; } = new(-75, -40);
        
        [SerializeField] private KingdomLoader _kingdomLoader;

        private void Awake()
        {
            if (_kingdomLoader == null)
            {
                Debug.LogError("KingdomLoader is not set. Won't be able to displaty cities name.");
                return;
            }
            _kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
        }

        private void OnKingdomLoaded()
        {
            CityBuilding[] cities = FindObjectsOfType<CityBuilding>();
            foreach (CityBuilding city in cities)
            {
                SetupCityNamePanel(city);
            }
        }

        private void SetupCityNamePanel(CityBuilding city)
        {
            TemplateContainer cityNamePanel = CityNamePanelTemplate.Instantiate();
            cityNamePanel.Q<Label>(CITY_LABEL_UI_NAME).text = city.CityBuildingConfiguration.Name;

            WorldUIPositioner cityNamePositioner = gameObject.AddComponent<WorldUIPositioner>();
            cityNamePositioner.Setup(_uiDoc, cityNamePanel, city.CityNamePanelAnchor.transform, offset: DisplayOffset);

            _uiDoc.rootVisualElement.Add(cityNamePanel);
        }
    }
}