using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Utils.GameObjectVisuals;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.FightCells;
using System.Linq;

namespace FrostfallSaga.Fight.Abilities.AbilityAnimation
{

    [Serializable]
    public class CellPerCellExternalExecutor : AExternalAbilityAnimationExecutor
    {
        [SerializeField] public Vector3 StartLocation;
        [SerializeField] public Vector3 EndLocation;
        [SerializeField] public Quaternion StartRotation;
        [SerializeField] public Quaternion EndRotation;
        [SerializeField, Range(0.1f, 999f)] public float MoveSpeed;
        [SerializeField, Range(0f, 999f)] public float TimeBeforNextMovementStart;

        private Fighter _fighterThatExecutes;
        private List<GameObject> _projectilesToFinishedTranslation;

        public override void Execute(Fighter fighterThatExecutes, FightCell[] abilityCells, GameObject projectilePrefab)
        {
            _fighterThatExecutes = fighterThatExecutes;
            _projectilesToFinishedTranslation = new();

            if (projectilePrefab.GetComponent<FighterCollider>() == null)
            {
                Debug.LogWarning("No fighter collider found on projectile prefab. onFighterTouched event will not be triggered.");
            }

            foreach (FightCell targetCell in abilityCells)
            {
                GameObject projectile = UnityEngine.Object.Instantiate(projectilePrefab, targetCell.transform);
                SetupProjectileColliderEventsIfAny(projectile);
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
                _projectilesToFinishedTranslation.ForEach(projectile => UnityEngine.Object.Destroy(projectile));
            }
        }
    }
}
