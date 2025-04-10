using UnityEngine;

namespace FrostfallSaga.InventorySystem.UI
{
    public class InventoryHeroRenderTextureSceneController : MonoBehaviour
    {
        [SerializeField] private Transform sceneRoot;
        [SerializeField] private Transform heroModelContainer;

        #region Setup

        private void Awake()
        {
            if (!sceneRoot)
            {
                Debug.LogError("Scene root is not assigned.");
                return;
            }

            if (!heroModelContainer)
            {
                Debug.LogError("Hero model container is not assigned.");
            }
        }

        #endregion

        private void Start()
        {
            SetSceneActive(false); // Only activate the scene when inventory is open
        }

        /// <summary>
        /// Clears any existing hero model and instantiates a new one.
        /// </summary>
        /// <param name="heroModelPrefab">The prefab of the hero model to be instantiated.</param>
        public void SetupHeroModel(GameObject heroModelPrefab)
        {
            if (!heroModelContainer)
            {
                Debug.LogError("Hero model container is not assigned.");
                return;
            }

            // Clear any existing hero model
            CleanupHeroModel();

            // Instantiate the new hero model under the container
            Instantiate(heroModelPrefab, heroModelContainer);

            // Activate the scene
            SetSceneActive(true);
        }

        public void SetSceneActive(bool isActive)
        {
            sceneRoot.gameObject.SetActive(isActive);
            if (!isActive) CleanupHeroModel();
        }

        private void CleanupHeroModel()
        {
            foreach (Transform child in heroModelContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}