using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals.GameObjectSpawnControllers
{
    /// <summary>
    /// Spawns a game object immediately. No animation or delay.
    /// </summary>
    public class ImmediateGameObjectSpawn : AGameObjectSpawnController
    {
        public override void SpawnGameObject(Transform reference, GameObject gameObjectPrefab)
        {
            Vector3 relativeSpawnPosition = reference.position + _spawnOffset;
            Quaternion spawnRotation = _spawnRotation != null ? _spawnRotation : reference.rotation * _rotationOffset;

            GameObject spawnedGameObject = Instantiate(
                gameObjectPrefab,
                relativeSpawnPosition,
                spawnRotation,
                reference
            );
            onSpawnEnded?.Invoke(spawnedGameObject);
        }
    }
}