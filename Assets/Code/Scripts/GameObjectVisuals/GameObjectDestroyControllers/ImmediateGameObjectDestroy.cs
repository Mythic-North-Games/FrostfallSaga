using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals.GameObjectDestroyControllers
{
    /// <summary>
    /// Destroy the game object immediately.
    /// </summary>
    public class ImmediateGameObjectDestroy : AGameObjectDestroyController
    {
        public override void DestroyGameObject(GameObject gameObject)
        {
            Destroy(gameObject);
            onDestroyEnded?.Invoke();
        }
    }
}