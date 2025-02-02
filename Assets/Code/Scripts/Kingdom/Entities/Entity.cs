using System;
using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.EntitiesVisual;

namespace FrostfallSaga.Kingdom.Entities
{
    [RequireComponent(typeof(Collider))]
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
        [field: SerializeField] public EntityMouseEventsController MouseEventsController { get; private set; }
        [field: SerializeField] public EntityConfigurationSO EntityConfiguration { get; private set; }
        [field: SerializeField] public bool IsDead { get; set; }
        [field: SerializeField] public string SessionId { get; private set; }

        public void ShowVisual()
        {
            if (AnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            AnimationController.gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            if (AnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            AnimationController.gameObject.SetActive(false);
        }

        public bool IsVisualShown()
        {
            if (AnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return false;
            }

            return AnimationController.gameObject.activeSelf;
        }

        public void Setup(string sessionId, bool isDead)
        {
            SessionId = sessionId;
            IsDead = isDead;
        }

        #region Components setup

        private void Awake()
        {
            AnimationController = GetComponentInChildren<EntityVisualAnimationController>();
            if (AnimationController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have an animation controller and a visual.");
            }
            MovementController = GetComponentInChildren<EntityVisualMovementController>();
            if (MovementController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have a movement controller and a visual.");
            }

            MouseEventsController = GetComponent<EntityMouseEventsController>();
            if (MouseEventsController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have a mouse events controller.");
            }

            SessionId = Guid.NewGuid().ToString();
            name = $"{EntityConfiguration.Name}_{SessionId}";
        }

        #endregion
    }
}