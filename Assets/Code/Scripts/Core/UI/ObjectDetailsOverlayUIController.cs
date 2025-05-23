using UnityEngine;
using UnityEngine.UIElements;

namespace FrostfallSaga.Core.UI
{
    public class ObjectDetailsOverlayUIController : BaseOverlayUIController
    {
        private static readonly string OBJECT_DETAILS_PANEL_DEFAULT_CLASSNAME = "objectDetailsContainer";
        private static readonly string OBJECT_DETAILS_PANEL_HIDDEN_CLASSNAME = "objectDetailsContainerHidden";
        private static readonly string OBJECT_DETAILS_EFFECT_LINE_CLASSNAME = "objectDetailsEffectLine";
        private static readonly Color STAT_VALUE_COLOR = new(225f / 255f, 225f / 255f, 225f / 255f);
        private static readonly Color STAT_ICON_COLOR = new(225f / 255f, 225f / 255f, 225f / 255f);
        private static readonly float EXTRA_NAME_PADDDING_TOP_ON_BIG_LETTERS = 5f;

        private readonly ObjectDetailsUIController _objectDetailsPanelController;

        public ObjectDetailsOverlayUIController(
            VisualTreeAsset overlayTemplate,
            VisualTreeAsset statContainerTemplate
        ) : base(overlayTemplate, OBJECT_DETAILS_PANEL_DEFAULT_CLASSNAME, OBJECT_DETAILS_PANEL_HIDDEN_CLASSNAME)
        {
            _objectDetailsPanelController = new(
                OverlayRoot,
                statContainerTemplate,
                OBJECT_DETAILS_EFFECT_LINE_CLASSNAME,
                STAT_VALUE_COLOR,
                STAT_ICON_COLOR,
                EXTRA_NAME_PADDDING_TOP_ON_BIG_LETTERS
            );
        }

        public void SetObject(IUIObjectDescribable describableObject)
        {
            _objectDetailsPanelController.Setup(describableObject);
        }
    }
}