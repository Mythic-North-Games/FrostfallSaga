using FrostfallSaga.Utils;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    public class CellVisualController
    {
        private readonly Transform _parentTransform;
        private readonly Renderer _renderer;

        public CellVisualController(Transform parentTransform)
        {
            _parentTransform = parentTransform;
            _renderer = parentTransform.GetComponent<Renderer>();
        }
        
        public void ApplyViusals(TerrainTypeSO terrainType, bool isAccessible)
        {
            if (!terrainType)
            {
                Debug.LogError($"{_parentTransform.name} - TerrainType is null.");
                return;
            }

            GameObject[] visuals = isAccessible ? terrainType.VisualsWhenAccessible : terrainType.VisualsWhenBlocked;
            if (visuals is { Length: > 0 })
            {
                GameObject visualPrefab = Randomizer.GetRandomElementFromArray(visuals);
                GameObject visualInstance = Object.Instantiate(
                    visualPrefab,
                    _parentTransform.position,
                    Randomizer.GetRandomRotationY(_parentTransform.rotation),
                    _parentTransform
                );
                visualInstance.name = $"Visual_{_parentTransform.name}";
                LayerUtils.SetLayerRecursively(visualInstance, 2);
            }

            if (_renderer && terrainType.CellMaterial)
                _renderer.sharedMaterial = terrainType.CellMaterial;
        }
    }
}