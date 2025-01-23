using UnityEngine;
using UnityEngine.UIElements;
using FrostfallSaga.Kingdom.Cities;
using FrostfallSaga.Utils.UI;

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
            City[] cities = FindObjectsOfType<City>();
            foreach (City city in cities)
            {
                SetupCityNamePanel(city);
            }
        }

        private void SetupCityNamePanel(City city)
        {
            TemplateContainer cityNamePanel = CityNamePanelTemplate.Instantiate();
            cityNamePanel.Q<Label>(CITY_LABEL_UI_NAME).text = city.CityConfiguration.Name;

            WorldUIPositioner cityNamePositioner = gameObject.AddComponent<WorldUIPositioner>();
            cityNamePositioner.Setup(_uiDoc, cityNamePanel, city.CityNamePanelAnchor.transform, offset: DisplayOffset);

            _uiDoc.rootVisualElement.Add(cityNamePanel);
        }
    }
}