using UnityEngine;

namespace FrostfallSaga.Utils
{
    /// <summary>
    /// A utility class for managing layers of GameObjects in Unity.
    /// Provides methods for checking and setting layers for GameObjects, including recursively setting layers for children.
    /// </summary>
    public static class LayerUtility
    {
        /// <summary>
        /// Checks if a GameObject is on a specified layer by name.
        /// </summary>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <param name="layerName">The name of the layer to check.</param>
        /// <returns>True if the GameObject is on the specified layer, otherwise false.</returns>
        public static bool IsOnLayer(GameObject gameObject, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return gameObject != null && gameObject.layer == layer;
        }

        /// <summary>
        /// Checks if a GameObject is on a specified layer by index.
        /// </summary>
        /// <param name="gameObject">The GameObject to check.</param>
        /// <param name="layer">The index of the layer to check.</param>
        /// <returns>True if the GameObject is on the specified layer, otherwise false.</returns>
        public static bool IsOnLayer(GameObject gameObject, int layer)
        {
            return gameObject != null && gameObject.layer == layer;
        }

        /// <summary>
        /// Sets the layer of a GameObject by name.
        /// </summary>
        /// <param name="gameObject">The GameObject whose layer will be set.</param>
        /// <param name="layerName">The name of the layer to assign.</param>
        /// <remarks>Logs an error if the layer name does not exist.</remarks>
        public static void SetLayer(GameObject gameObject, string layerName)
        {
            if (gameObject == null) return;

            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogError($"Layer '{layerName}' does not exist.");
                return;
            }

            gameObject.layer = layer;
        }

        /// <summary>
        /// Sets the layer of a GameObject by index.
        /// </summary>
        /// <param name="gameObject">The GameObject whose layer will be set.</param>
        /// <param name="layer">The index of the layer to assign.</param>
        public static void SetLayer(GameObject gameObject, int layer)
        {
            if (gameObject == null) return;
            gameObject.layer = layer;
        }

        /// <summary>
        /// Sets the layer for multiple GameObjects by name.
        /// </summary>
        /// <param name="gameObjects">An array of GameObjects whose layers will be set.</param>
        /// <param name="layerName">The name of the layer to assign to all GameObjects.</param>
        /// <remarks>Logs an error if the layer name does not exist.</remarks>
        public static void SetLayerForGameObjects(GameObject[] gameObjects, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogError($"Layer '{layerName}' does not exist.");
                return;
            }

            SetLayerForGameObjects(gameObjects, layer);
        }

        /// <summary>
        /// Sets the layer for multiple GameObjects by index.
        /// </summary>
        /// <param name="gameObjects">An array of GameObjects whose layers will be set.</param>
        /// <param name="layer">The index of the layer to assign to all GameObjects.</param>
        public static void SetLayerForGameObjects(GameObject[] gameObjects, int layer)
        {
            foreach (var gameObject in gameObjects)
            {
                if (gameObject != null)
                {
                    SetLayer(gameObject, layer);
                }
            }
        }

        /// <summary>
        /// Sets the layer for a parent GameObject and all of its child GameObjects by name.
        /// </summary>
        /// <param name="parent">The parent GameObject whose layer and the layers of all its children will be set.</param>
        /// <param name="layerName">The name of the layer to assign to the parent and its children.</param>
        /// <remarks>Logs an error if the layer name does not exist.</remarks>
        public static void SetLayerRecursively(GameObject parent, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogError($"Layer '{layerName}' does not exist.");
                return;
            }

            SetLayerRecursively(parent, layer);
        }

        /// <summary>
        /// Sets the layer for a parent GameObject and all of its child GameObjects by index.
        /// </summary>
        /// <param name="parent">The parent GameObject whose layer and the layers of all its children will be set.</param>
        /// <param name="layer">The index of the layer to assign to the parent and its children.</param>
        public static void SetLayerRecursively(GameObject parent, int layer)
        {
            if (parent == null) return;

            parent.layer = layer;

            foreach (Transform child in parent.transform)
            {
                if (child != null && child.gameObject != null)
                {
                    SetLayer(child.gameObject, layer);
                }
            }
        }
    }
}
