using System;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.EntitiesVisual;
using UnityEngine;

namespace FrostfallSaga.Kingdom.Entities
{
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
        [field: SerializeField] public EntityConfigurationSO EntityConfiguration { get; private set; }
        [field: SerializeField] public bool IsDead { get; set; }
        [field: SerializeField] public string SessionId { get; private set; }

        #region Components setup

        private void Awake()
        {
            AnimationController = GetComponentInChildren<EntityVisualAnimationController>();
            if (!AnimationController)
                Debug.LogWarning("Entity " + name + " does not have an animation controller and a visual.");
            MovementController = GetComponentInChildren<EntityVisualMovementController>();
            if (!MovementController)
                Debug.LogWarning("Entity " + name + " does not have a movement controller and a visual.");
        }

        #endregion

        public void ShowVisual()
        {
            if (!AnimationController)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            AnimationController.gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            if (!AnimationController)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            AnimationController.gameObject.SetActive(false);
        }

        public bool IsVisualShown()
        {
            if (AnimationController) return AnimationController.gameObject.activeSelf;
            Debug.LogError("Entity " + name + " does not have an entity visual.");
            return false;
        }

        public bool CanSeeTarget(Transform target)
        {
            return MovementController.CanSeeTarget(target, EntityConfiguration.KingdomDetectionRange);
        }

        public void Setup(EntityConfigurationSO entityConfigurationSO, string sessionId, bool isDead)
        {
            EntityConfiguration = entityConfigurationSO;
            SessionId = sessionId;
            IsDead = isDead;
        }

        public void Setup(EntityConfigurationSO entityConfiguration)
        {
            EntityConfiguration = entityConfiguration;
            SessionId = Guid.NewGuid().ToString();
            name = $"{EntityConfiguration.Name}_{SessionId}";
            if (EntityConfiguration.FighterConfiguration is PersistedFighterConfigurationSO
                persistedFighterConfiguration) IsDead = persistedFighterConfiguration.Health == 0;
        }
    }
}