using UnityEngine;
using FrostfallSaga.EntitiesVisual;

namespace FrostfallSaga.Kingdom.Entities
{
    [RequireComponent(typeof(Collider))]
    public class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityConfigurationSO EntityConfiguration { get; private set; }
        [field: SerializeField] public EntityVisualAnimationController EntityAnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController EntityVisualMovementController { get; private set; }
        [field: SerializeField] public EntityMouseEventsController EntityMouseEventsController { get; private set; }
        [field: SerializeField] public bool IsDead { get; private set; }

        private void Awake()
        {
            EntityAnimationController = GetComponentInChildren<EntityVisualAnimationController>();
            if (EntityAnimationController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have an animation controller and a visual.");
            }
              EntityVisualMovementController = GetComponentInChildren<EntityVisualMovementController>();
            if (EntityVisualMovementController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have a movement controller and a visual.");
            }

            EntityMouseEventsController = GetComponent<EntityMouseEventsController>();
            if (EntityMouseEventsController == null)
            {
                Debug.LogWarning("Entity " + name + " does not have a mouse events controller.");
            }

            if (EntityConfiguration == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity configuration.");
            }
        }

        public void ShowVisual()
        {
            if (EntityAnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            EntityAnimationController.gameObject.SetActive(true);
        }

        public void HideVisual()
        {
            if (EntityAnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return;
            }

            EntityAnimationController.gameObject.SetActive(false);
        }

        public bool IsVisualShown()
        {
            if (EntityAnimationController == null)
            {
                Debug.LogError("Entity " + name + " does not have an entity visual.");
                return false;
            }

            return EntityAnimationController.gameObject.activeSelf;
        }
    }
}