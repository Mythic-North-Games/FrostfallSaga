using System;
using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals.GameObjectSpawnControllers
{
    /// <summary>
    /// Base class for all GameObject spawn controllers.
    /// Handles how a game object should spawn.
    /// </summary>
    public abstract class AGameObjectSpawnController : MonoBehaviour
    {
        [SerializeField, Tooltip("Offset to the reference.")] protected Vector3 _spawnOffset;
        [
            SerializeField,
            Tooltip("Default spawn rotation of the object. If set, won't consider rotation offset.")
        ]
        protected Quaternion _spawnRotation;
        [SerializeField, Tooltip("Rotation offset to the reference.")] protected Quaternion _rotationOffset;

        public Action<GameObject> onSpawnEnded;

        public abstract void SpawnGameObject(Transform reference, GameObject gameObjectPrefab);
    }
}