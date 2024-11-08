using System;
using UnityEngine;

namespace FrostfallSaga.GameObjectVisuals.GameObjectDestroyControllers
{
    /// <summary>
    /// Base class for all GameObject destroy controllers.
    /// Controls how a GameObject is destroyed.
    /// </summary>
    [Serializable]
    public abstract class AGameObjectDestroyController
    {
        public Action onDestroyEnded;

        public abstract void DestroyGameObject(GameObject gameObject);
    }
}