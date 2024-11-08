using System;
using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals.GameObjectDestroyControllers
{
    /// <summary>
    /// Destroy the game object immediately.
    /// </summary>
    [Serializable]
    public class ImmediateGameObjectDestroy : AGameObjectDestroyController
    {
        public override void DestroyGameObject(GameObject gameObject)
        {
            UnityEngine.Object.Destroy(gameObject);
            onDestroyEnded?.Invoke();
        }
    }
}