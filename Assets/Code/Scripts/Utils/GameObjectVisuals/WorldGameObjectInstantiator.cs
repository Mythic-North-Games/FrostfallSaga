using UnityEngine;

namespace FrostfallSaga.Utils.GameObjectVisuals
{
    /// <summary>
    /// Instantiates game objects in the world under a given default parent.
    /// </summary>
    public class WorldGameObjectInstantiator : MonoBehaviour
    {
        private static string DEFAULT_WORLD_GO_PARENT_NAME = "WORLD";

        [field: SerializeField, Tooltip("The game object that will contain all the world game object.")]
        public GameObject DefaultWorldGOParent { get; private set; }

        private void Awake()
        {
            if (DefaultWorldGOParent == null)
            {
                Debug.Log("DefaultWorldGOParent is null. Trying to find it in the scene.");
                DefaultWorldGOParent = GameObject.Find(DEFAULT_WORLD_GO_PARENT_NAME);
            }

            if (DefaultWorldGOParent == null)
            {
                Debug.Log("DefaultWorldGOParent is still null. Creating a new one.");
                DefaultWorldGOParent = new GameObject(DEFAULT_WORLD_GO_PARENT_NAME);
            }
        }

        /// <summary>
        /// Instantiates a game object in the world under the default parent.
        /// </summary>
        /// <param name="gameObject">The game object to instantiate.</param>
        /// <returns>The instantiated game object.</returns>
        public GameObject Instantiate(GameObject gameObject)
        {
            return Instantiate(gameObject, DefaultWorldGOParent.transform);
        }
    }
}