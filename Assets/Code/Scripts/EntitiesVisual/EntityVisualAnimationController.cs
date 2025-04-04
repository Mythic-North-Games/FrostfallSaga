using System;
using UnityEngine;

namespace FrostfallSaga.EntitiesVisual
{
    /// <summary>
    ///     Controls the animation of the visible entity.
    /// </summary>
    public class EntityVisualAnimationController : MonoBehaviour
    {
        [field: SerializeField] public string DefaultAnimationState { get; private set; } = "Idle";
        private Animator _animator;
        public string CurrentStateName { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            try
            {
                _animator.Play(DefaultAnimationState);
                CurrentStateName = DefaultAnimationState;
            }
            catch (Exception)
            {
                Debug.LogError("Default animation state: " + DefaultAnimationState +
                               " does not exists on animator of entity " + name);
            }
        }

        public void PlayAnimationState(string newState)
        {
            try
            {
                _animator.Play(newState);
                CurrentStateName = newState;
            }
            catch (Exception)
            {
                Debug.LogError(
                    "Default animation state: " + newState + " does not exists on animator of entity " + name);
            }
        }

        public void RestoreDefaultAnimation()
        {
            try
            {
                _animator.Play(DefaultAnimationState);
                CurrentStateName = DefaultAnimationState;
            }
            catch (Exception)
            {
                Debug.LogError("Default animation state: " + DefaultAnimationState +
                               " does not exists on animator of entity " + name);
            }
        }
    }
}