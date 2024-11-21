using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid.Cells;
using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{
    /// <summary>
    /// Only used to test ability animations in ability animation test scene.
    /// </summary>
    public class TestAbilityAnimationLauncher : MonoBehaviour
    {
        [SerializeReference] private AExternalAbilityAnimationExecutor _abilityAnimationExecutor;
        [SerializeField] private Fighter _initator;
        [SerializeField] private FightCell[] _abilityCells;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private string _animationStateName;
        [SerializeField] private float _animationDuration;
        [SerializeField] private FighterCollider _colliderToTrack;

        private void Start()
        {
            _abilityAnimationExecutor.onAnimationEnded += OnAnimationEnded;
            _abilityAnimationExecutor.onCellTouched += OnCellTouched;
            _abilityAnimationExecutor.onFighterTouched += OnFighterTouched;
            _abilityAnimationExecutor.Execute(_initator, _abilityCells, _projectilePrefab);
        }

        private void OnAnimationEnded(Fighter fighter)
        {
            Debug.Log($"Ended animation of fighter {fighter.name}");
        }

        private void OnCellTouched(FightCell cell)
        {
            Debug.Log($"Touched cell {cell.name}");
        }

        private void OnFighterTouched(Fighter fighter)
        {
            Debug.Log($"Touched fighter {fighter.name}");
        }
    }
}