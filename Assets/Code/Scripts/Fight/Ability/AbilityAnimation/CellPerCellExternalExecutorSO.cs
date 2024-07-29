using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{

    [CreateAssetMenu(
        fileName = "CellPerCellExternalExecutor",
        menuName = "ScriptableObjects/Fight/Abililties/AnimationExecutors/External/CellPerCellExternalExecutor",
        order = 0
    )]
    public class CellPerCellExternalExecutorSO : AExternalAbilityAnimationExecutorSO
    {
        [field: SerializeField] public Vector3 StartLocation { get; private set; }
        [field: SerializeField] public Vector3 EndLocation { get; private set; }
        [field: SerializeField] public Quaternion StartRotation { get; private set; }
        [field: SerializeField] public Quaternion EndRotation { get; private set; }
        [field: SerializeField, Range(0.1f, 999f)] public float MoveSpeed { get; private set; }
        [field: SerializeField, Range(0f, 999f)] public float TimeBeforNextMovementStart { get; private set; }

        private Fighter _fighterThatExecutes;
        private List<GameObject> _projectilesToFinishedTranslation;

        public override void Execute(Fighter fighterThatExecutes, Cell[] abilityCells, GameObject projectilePrefab)
        {
            _fighterThatExecutes = fighterThatExecutes;
            _projectilesToFinishedTranslation = new();

            if (projectilePrefab.GetComponent<FighterCollider>() == null)
            {
                Debug.LogWarning("No fighter collider found on projectile prefab. onFighterTouched event will not be triggered.");
            }

            foreach (Cell targetCell in abilityCells)
            {
                GameObject projectile = Instantiate(projectilePrefab, targetCell.transform);
                SetupProjectileColliderEventIfAny(projectile);
                projectile.transform.position = new(999999, 999999);    // Hide before launching
                _projectilesToFinishedTranslation.Add(projectile);
            }

            _fighterThatExecutes.StartCoroutine(LaunchProjectiles());
        }

        private IEnumerator LaunchProjectiles()
        {
            foreach (GameObject projectile in _projectilesToFinishedTranslation)
            {
                // Add translator to make the projectile move
                GameObjectTranslator translator = projectile.AddComponent<GameObjectTranslator>();
                translator.onTargetLocationReached += OnProjectileReachedEndLocation;

                projectile.transform.SetLocalPositionAndRotation(StartLocation, StartRotation);
                translator.TranslateTo(projectile.transform, EndLocation, MoveSpeed);
                yield return new WaitForSeconds(TimeBeforNextMovementStart);
            }
        }

        private void OnProjectileReachedEndLocation(Transform projectileTransform)
        {
            projectileTransform.gameObject.GetComponent<GameObjectTranslator>().onTargetLocationReached -= OnProjectileReachedEndLocation;
            projectileTransform.gameObject.SetActive(false);

            if (_projectilesToFinishedTranslation.All(projectile => !projectile.activeInHierarchy))
            {
                onAnimationEnded?.Invoke(_fighterThatExecutes);
                _projectilesToFinishedTranslation.ForEach(projectile => Destroy(projectile));
            }
        }
    }
}
