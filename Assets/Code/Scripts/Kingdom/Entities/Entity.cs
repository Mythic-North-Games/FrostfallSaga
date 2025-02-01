using System;
using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Core.Fight;

namespace FrostfallSaga.Kingdom.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
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

            name = $"{EntityConfiguration.Name}_{SessionId}";
            SessionId = Guid.NewGuid().ToString();
            if (EntityConfiguration.FighterConfiguration is PersistedFighterConfigurationSO persistedFighterConfiguration)
            {
                IsDead = persistedFighterConfiguration.Health == 0;
            }
        }

        #endregion
    }
}