using FrostfallSaga.Utils;
using UnityEditor;
using UnityEngine;

namespace FrostfallSaga.Grid.Cells
{
    public class CellVisualController
    {
        public string PrefabPath { get; private set; }
        public Transform VisualTransform => _visualInstance != null ? _visualInstance.transform : null;

        private readonly Transform _parentTransform;
        private GameObject _visualInstance;

        public CellVisualController(Transform parentTransform)
        {
            _parentTransform = parentTransform;
        }

        public void ApplyVisuals(TerrainTypeSO terrainType, bool isAccessible)
        {
            if (!terrainType)
            {
                Debug.LogError($"{_parentTransform.name} - TerrainType is null.");
                return;
            }

            GameObject[] visuals = isAccessible ? terrainType.VisualsWhenAccessible : terrainType.VisualsWhenBlocked;
            if (visuals.Length == 0)
            {
                Debug.LogError($"{_parentTransform.name} - No visuals available for this terrain type.");
                return;
            }

            GameObject visualPrefab = Randomizer.GetRandomElementFromArray(visuals);
            PrefabPath = AssetDatabase.GetAssetPath(visualPrefab).Split("Resources/")[1].Split(".prefab")[0];
            Vector3 position = _parentTransform.position;
            position.y += 0.12f;
            _visualInstance = Object.Instantiate(
                visualPrefab,
                position,
                Randomizer.GetRandomRotationY(_parentTransform.rotation),
                _parentTransform
            );
            _visualInstance.name = GetVisualName();
            LayerUtils.SetLayerRecursively(_visualInstance, ELayersName.IGNORE_RAYCAST.ToLayerInt());
            SetBaseCellMaterial(terrainType);
        }

        public void RecoverVisuals(
            TerrainTypeSO terrainType,
            string visualPrefabPath,
            Vector3 visualPosition,
            Quaternion visualRotation
        )
        {
            PrefabPath = visualPrefabPath;
            _visualInstance = Object.Instantiate(
                Resources.Load<GameObject>(visualPrefabPath),
                visualPosition,
                visualRotation,
                _parentTransform
            );
            _visualInstance.name = GetVisualName();
            LayerUtils.SetLayerRecursively(_visualInstance, ELayersName.IGNORE_RAYCAST.ToLayerInt());
            SetBaseCellMaterial(terrainType);
        }

        public void DestroyVisual()
        {
            if (_visualInstance != null)
            {
                Object.Destroy(_visualInstance);
                _visualInstance = null;
            }
        }

        public void SetBaseCellMaterial(TerrainTypeSO terrainType)
        {
            Material[] mats = _parentTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterials;
            mats[1] = terrainType.CellMaterial;
            _parentTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        }

        private string GetVisualName()
        {
            return $"Visual_{_parentTransform.name}";
        }
    }
}