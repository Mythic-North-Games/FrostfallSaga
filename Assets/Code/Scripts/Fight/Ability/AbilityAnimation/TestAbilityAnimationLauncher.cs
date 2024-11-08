using UnityEngine;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    /// <summary>
    /// Only used to test ability animations in ability animation test scene.
    /// </summary>
    public class TestAbilityAnimationLauncher : MonoBehaviour
    {
        [SerializeReference] private AInternalAbilityAnimationExecutor _abilityAnimationExecutor;
        [SerializeField] private Fighter _initator;
        [SerializeField] private Cell[] _abilityCells;
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationDuration;
        [SerializeField] private FighterCollider _colliderToTrack;

        private void Start()
        {
            _abilityAnimationExecutor.onAnimationEnded += OnAnimationEnded;
            _abilityAnimationExecutor.onFighterTouched += OnFighterTouched;
            _abilityAnimationExecutor.Execute(_initator, _abilityCells, _animationStateName, _animationDuration, _colliderToTrack);
        }

        private void OnAnimationEnded(Fighter fighter)
        {
            Debug.Log($"Ended animation of fighter {fighter.name}");
        }

        private void OnFighterTouched(Fighter fighter)
        {
            Debug.Log($"Touched fighter {fighter.name}");
        }
    }
}