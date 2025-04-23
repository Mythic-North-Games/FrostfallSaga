using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    public class CellVisualController
    {
        private readonly Transform _parentTransform;
        private readonly MaterialHighlightable _highlightable;


        public CellVisualController(Transform parentTransform)
        {
            _parentTransform = parentTransform;
            _highlightable = _parentTransform.GetComponentInChildren<MaterialHighlightable>();
        }
        
        public void ApplyVisuals(TerrainTypeSO terrainType, bool isAccessible)
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
                Vector3 position = _parentTransform.position;
                position.y += 0.12f;
                GameObject visualInstance = Object.Instantiate(
                    visualPrefab,
                    position,
                    Randomizer.GetRandomRotationY(_parentTransform.rotation),
                    _parentTransform
                );
                visualInstance.name = $"Visual_{_parentTransform.name}";
                LayerUtils.SetLayerRecursively(visualInstance, 2);
            }

            Material[] mats = _parentTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
            mats[1] = terrainType.CellMaterial;
            _parentTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        }
    }
}