using System.Collections;
using FrostfallSaga.Utils;
using FrostfallSaga.Utils.GameObjectVisuals;
using UnityEngine;

namespace FrostfallSaga.Grid
{
    public class CellVisual : MonoBehaviour
    {
        public MaterialHighlightable HighlightController { get; private set; }

        private void Awake()
        {
            HighlightController = GetComponent<MaterialHighlightable>();
            if (HighlightController == null)
            {
                Debug.LogError("CellVisual : MaterialHighlightable not found.");
            }
        }

        public void Initializer()
        {
            Awake();
        }

        public void UpdateHeightVisual(float newHeight, float duration)
        {
            if (duration == 0)
            {
                transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
            }
            else
            {
                StartCoroutine(SmoothMoveToHeight(newHeight, duration));
            }
        }

        private IEnumerator SmoothMoveToHeight(float targetHeight, float duration)
        {
            float startHeight = transform.position.y;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newY = Mathf.Lerp(startHeight, (float)targetHeight, elapsedTime / duration);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                yield return null;
            }

            transform.position = new Vector3(transform.position.x, (float)targetHeight, transform.position.z);
        }

        public void SetTerrainVisual(TerrainTypeSO terrainType)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();

            if (renderer != null && terrainType != null && terrainType.CellMaterial != null)
            {
                if (terrainType.VisualsTerrain != null && terrainType.VisualsTerrain.Length != 0)
                {
                    GameObject visualTerrain = Randomizer.GetRandomElementFromArray(terrainType.VisualsTerrain);
                    GameObject newVisualTerrain = Instantiate<GameObject>(visualTerrain, transform.position, Randomizer.GetRandomRotationY(transform.rotation), transform);
                    newVisualTerrain.name = "Visual" + name;
                    LayerUtils.SetLayerRecursively(newVisualTerrain, 2);
                }
            }
            else
            {
                Debug.LogError("Cell " + name + " doesn't have a renderer or a valid material.");
            }
        }
    }
}
