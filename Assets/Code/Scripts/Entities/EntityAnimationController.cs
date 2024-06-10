using System;
using UnityEngine;

namespace FrostfallSaga.Entities
{
    /// <summary>
    /// Controls the animation of the visible entity.
    /// </summary>
    public class EntityAnimationController : MonoBehaviour
    {
        [field: SerializeField] public EntitySO EntityConfiguration { get; private set; }
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
                Debug.Log("startiub");
                _animator.Play(EntityConfiguration.DefaultAnimationState);
                CurrentStateName = EntityConfiguration.DefaultAnimationState;
            }
            catch (Exception)
            {
                Debug.LogError("Default animation state: " + EntityConfiguration.DefaultAnimationState + " does not exists on animator of entity " + name);
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
                Debug.LogError("Default animation state: " + newState + " does not exists on animator of entity " + name);
            }
        }

        public void RestoreDefaultAnimation()
        {
            try
            {
                _animator.Play(EntityConfiguration.DefaultAnimationState);
                CurrentStateName = EntityConfiguration.DefaultAnimationState;
            }
            catch (Exception)
            {
                Debug.LogError("Default animation state: " + EntityConfiguration.DefaultAnimationState + " does not exists on animator of entity " + name);
            }
        }
    }
}