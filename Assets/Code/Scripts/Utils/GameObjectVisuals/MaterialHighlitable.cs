using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FrostfallSaga.Utils.GameObjectVisuals
{
    [RequireComponent(typeof(MeshRenderer))]
    public class MaterialHighlightable : MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer TargetRenderer { get; private set; }
        [SerializeField] private List<HighlightColorData> highlightColors;

        private Dictionary<HighlightColor, Color> _colorLookup;
        private Material _materialInstance;

        private void Awake()
        {
            if (!TargetRenderer)
            {
                Debug.LogError($"{nameof(TargetRenderer)} is not set");
                return;
            }

            if (!TargetRenderer.material)
            {
                Debug.LogError($"{nameof(TargetRenderer)} material is not set");
                return;
            }

            _materialInstance = TargetRenderer.material;
            _colorLookup = highlightColors.ToDictionary(color => color.colorType, color => color.color);

            TargetRenderer.gameObject.SetActive(false);
        }

        public void Highlight(HighlightColor type)
        {
            if (!_materialInstance) return;
            _materialInstance.color = _colorLookup.TryGetValue(type, out Color color) ? color : Color.white;

            TargetRenderer.gameObject.SetActive(true);
        }

        public void ResetToInitialColor()
        {
            if (!_materialInstance) return;
            _materialInstance.color =
                _colorLookup.TryGetValue(HighlightColor.NONE, out Color color) ? color : Color.white;

            TargetRenderer.gameObject.SetActive(false);
        }
    }
}