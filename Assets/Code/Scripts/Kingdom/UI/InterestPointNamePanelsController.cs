using FrostfallSaga.Core.UI;
using FrostfallSaga.Kingdom.InterestPoints;
using FrostfallSaga.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Kingdom.UI
{
    /// <summary>
    ///     Controls the panels to display the name of interest points on top of the 3D model.
    /// </summary>
    public class InterestPointNamePanelsController : BaseUIController
    {
        private static readonly string INTEREST_POINT_LABEL_UI_NAME = "InterestPointNameLabel";

        [field: SerializeField] public VisualTreeAsset InterestPointNamePanelTemplate { get; private set; }
        [field: SerializeField] public Vector2 DisplayOffset { get; private set; } = new(-75, -40);

        [SerializeField] private KingdomLoader _kingdomLoader;

        private void Awake()
        {
            if (_kingdomLoader == null)
            {
                Debug.LogError("KingdomLoader is not set. Won't be able to dispay interest points name.");
                return;
            }

            _kingdomLoader.onKingdomLoaded += OnKingdomLoaded;
        }

        private void OnKingdomLoaded()
        {
            InterestPoint[] interestPoints = FindObjectsOfType<InterestPoint>();
            foreach (InterestPoint interestPoint in interestPoints) SetupInterestPointNamePanel(interestPoint);
        }

        private void SetupInterestPointNamePanel(InterestPoint interestPoint)
        {
            VisualElement interestPointNamePanel = InterestPointNamePanelTemplate.Instantiate();
            interestPointNamePanel.Q<Label>(INTEREST_POINT_LABEL_UI_NAME).text =
                interestPoint.InterestPointConfiguration.Name;

            WorldUIPositioner interestPointNamePositioner = gameObject.AddComponent<WorldUIPositioner>();
            interestPointNamePositioner.Setup(
                uiDocToDisplayOn: _uiDoc,
                uiToDisplay: interestPointNamePanel,
                anchorTransform: interestPoint.NamePanelAnchor.transform,
                offset: DisplayOffset
            );

            _uiDoc.rootVisualElement.Add(interestPointNamePanel);
        }
    }
}