using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrostfallSaga.Utils.GameObjectVisuals.GameObjectDestroyControllers
{
    /// <summary>
    ///     Destroy the game object immediately.
    /// </summary>
    [Serializable]
    public class ImmediateGameObjectDestroy : AGameObjectDestroyController
    {
        public override void DestroyGameObject(GameObject gameObject)
        {
            Object.Destroy(gameObject);
            onDestroyEnded?.Invoke();
        }
    }
}