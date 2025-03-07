using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrostfallSaga.Utils.GameObjectVisuals.GameObjectSpawnControllers
{
    /// <summary>
    ///     Spawns a game object immediately. No animation or delay.
    /// </summary>
    [Serializable]
    public class ImmediateGameObjectSpawn : AGameObjectSpawnController
    {
        public override void SpawnGameObject(Transform reference, GameObject gameObjectPrefab)
        {
            Vector3 relativeSpawnPosition = reference.position + SpawnOffset;
            Quaternion spawnRotation = SpawnRotation != null ? SpawnRotation : reference.rotation * RotationOffset;

            GameObject spawnedGameObject = Object.Instantiate(
                gameObjectPrefab,
                relativeSpawnPosition,
                spawnRotation,
                reference
            );
            onSpawnEnded?.Invoke(spawnedGameObject);
        }
    }
}