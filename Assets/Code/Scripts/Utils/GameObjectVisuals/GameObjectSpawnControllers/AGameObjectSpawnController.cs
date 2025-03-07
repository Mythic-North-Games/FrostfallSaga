using System;
using UnityEngine;

namespace FrostfallSaga.Utils.GameObjectVisuals.GameObjectSpawnControllers
{
    /// <summary>
    ///     Base class for all GameObject spawn controllers.
    ///     Handles how a game object should spawn.
    /// </summary>
    [Serializable]
    public abstract class AGameObjectSpawnController
    {
        [SerializeField] [Tooltip("Offset to the reference.")]
        public Vector3 SpawnOffset;

        [SerializeField] [Tooltip("Default spawn rotation of the object. If set, won't consider rotation offset.")]
        public Quaternion SpawnRotation;

        [SerializeField] [Tooltip("Rotation offset to the reference.")]
        public Quaternion RotationOffset;

        public Action<GameObject> onSpawnEnded;

        public abstract void SpawnGameObject(Transform reference, GameObject gameObjectPrefab);
    }
}