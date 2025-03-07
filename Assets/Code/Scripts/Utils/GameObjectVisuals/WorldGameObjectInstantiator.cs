using UnityEngine;

namespace FrostfallSaga.Utils.GameObjectVisuals
{
    /// <summary>
    ///     Instantiates game objects in the world under a given default parent.
    /// </summary>
    public class WorldGameObjectInstantiator : MonoBehaviourPersistingSingleton<WorldGameObjectInstantiator>
    {
        private static readonly string DEFAULT_WORLD_GO_PARENT_NAME = "WORLD";

        private GameObject FindOrCreateDefaultWorldGOParent()
        {
            GameObject defaultWorldGOParent = GameObject.Find(DEFAULT_WORLD_GO_PARENT_NAME);
            if (defaultWorldGOParent == null)
            {
                Debug.Log("DefaultWorldGOParent is still null. Creating a new one.");
                defaultWorldGOParent = new GameObject(DEFAULT_WORLD_GO_PARENT_NAME);
            }

            return defaultWorldGOParent;
        }

        /// <summary>
        ///     Instantiates a game object in the world under the default parent.
        /// </summary>
        /// <param name="gameObject">The game object to instantiate.</param>
        /// <returns>The instantiated game object.</returns>
        public GameObject Instantiate(GameObject gameObject)
        {
            return Instantiate(gameObject, FindOrCreateDefaultWorldGOParent().transform);
        }
    }
}