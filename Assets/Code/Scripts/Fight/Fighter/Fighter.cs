using System;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Grid.Cells;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : MonoBehaviour
    {
        [field: SerializeField] public FighterConfigurationSO FighterConfiguration { get; private set; }
        public Cell cell;
        public Action<Fighter> OnFighterMoved;
        public Action<Fighter> OnFighterDirectAttackEnded;

        public Cell[] directAttackCells;

        [SerializeField] private EntityVisualAnimationController _animationController;
        [SerializeField] private EntityVisualMovementController _movementController;
        private MovePath _currentMovePath;
        private FighterStats _stats;

        private void Awake()
        {
            if (FighterConfiguration == null)
            {
                Debug.LogError("No fighter configuration for " + name);
            }
            if (!TrySetupEntitiyVisualMoveController())
            {
                Debug.LogError("No entity visual move controller found for fighter " + name);
            }
            if (!TrySetupEntitiyVisualAnimationController())
            {
                Debug.LogError("No entity visual animation controller found for fighter " + name);
            }

            _stats = new();
            ResetStatsToDefaultConfiguration();

            if (directAttackCells.Length > 0)
            {
                UseDirectAttack(directAttackCells);
            }
        }

        private void ResetStatsToDefaultConfiguration()
        {
            _stats.maxHealth = FighterConfiguration.MaxHealth;
            _stats.health = FighterConfiguration.MaxHealth;
            _stats.maxActionPoints = FighterConfiguration.MaxActionPoints;
            _stats.actionPoints = FighterConfiguration.MaxActionPoints;
            _stats.maxMovePoints = FighterConfiguration.MaxMovePoints;
            _stats.movePoints = FighterConfiguration.MaxMovePoints;
            _stats.strength = FighterConfiguration.Strength;
            _stats.dexterity = FighterConfiguration.Dexterity;
            _stats.physicalResistance = FighterConfiguration.PhysicalResistance;
            _stats.magicalResistances = MagicalElementToValue.GetDictionaryFromArray(FighterConfiguration.MagicalResistances);
            _stats.magicalStrengths = MagicalElementToValue.GetDictionaryFromArray(FighterConfiguration.MagicalStrengths);
            _stats.initiative = FighterConfiguration.Initiative;
        }

        /// <summary>
        /// Makes the fighter move along the given cells path.
        /// If fighter does not have enough move points, an ArgumentOutOfRangeException is thrown.
        /// </summary>
        /// <param name="cellsPath">The cells path to follow.</param>
        public void Move(Cell[] cellsPath)
        {
            if (cellsPath.Length > _stats.movePoints)
            {
                throw new ArgumentOutOfRangeException("Fighter " + name + " does not have enough move points to move.");
            }

            _currentMovePath = new(cellsPath);
            MakeNextMove();
        }

        public void UseDirectAttack(Cell[] targetedCells)
        {
            if (targetedCells.Length == 0)
            {
                throw new ArgumentException("A direct attack can't be used without one or more target cells");
            }

            if (FighterConfiguration.DirectAttackActionPointsCost > _stats.actionPoints)
            {
                throw new ArgumentOutOfRangeException("Fighter " + name + " does not have enough actions points to use its direct attack.");
            }

            PlayAnimationIfAny(FighterConfiguration.DirectAttackAnimationStateName);
            _stats.actionPoints -= FighterConfiguration.DirectAttackActionPointsCost;
            foreach (Cell targetedCell in targetedCells)
            {
                Fighter targetedCellFighter = targetedCell.GetComponent<CellFightBehaviour>().Fighter;
                if (targetedCellFighter != null)
                {
                    ApplyDirectAttackEffectsToFighter(targetedCellFighter);
                }
            }
        }

        private void ApplyDirectAttackEffectsToFighter(Fighter fighter)
        {
            foreach (AEffectSO effect in FighterConfiguration.DirectAttackEffects)
            {
                effect.ApplyEffect(fighter);
            }
        }

        public void PhysicalWithstand(int physicalDamageAmount)
        {

            PlayAnimationIfAny(FighterConfiguration.ReceiveDamageAnimationStateName);
            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            _stats.health = Math.Clamp(_stats.health - inflictedPhysicalDamageAmount, 0, _stats.maxHealth);
        }

        public void MagicalWithstand(int magicalDamageAmount, EMagicalElement magicalElement)
        {
            if (!_stats.magicalResistances.ContainsKey(magicalElement))
            {
                throw new NullReferenceException("Magical resistance element " + magicalElement + " is not set for fighter " + name);
            }

            PlayAnimationIfAny(FighterConfiguration.ReceiveDamageAnimationStateName);
            int inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.magicalResistances[magicalElement]);
            _stats.health = Math.Clamp(_stats.health - inflictedMagicalDamageAmount, 0, _stats.maxHealth);
        }

        public void Heal(int healAmount)
        {
            PlayAnimationIfAny(FighterConfiguration.HealSelfAnimationStateName);
            _stats.health = Math.Clamp(_stats.health + healAmount, 0, _stats.maxHealth);
        }

        private void PlayAnimationIfAny(string animationStateName)
        {
            try
            {
                _animationController.PlayAnimationState(animationStateName);
            }
            catch (Exception)
            {
                if (animationStateName != null)
                {
                    Debug.LogWarning("Animation named: " + animationStateName + " not found for fighter " + name);
                }
            }
        }

        #region Movement handling
        private void MakeNextMove()
        {
            Cell cellToMoveTo = _currentMovePath.GetNextCellInPath();
            cell.GetComponent<CellFightBehaviour>().Fighter = null;
            _movementController.Move(cell, cellToMoveTo, _currentMovePath.IsLastMove);
        }

        private void OnFighterArrivedAtCell(Cell destinationCell)
        {
            _stats.movePoints -= 1;
            cell = destinationCell;
            cell.GetComponent<CellFightBehaviour>().Fighter = this;
            if (!_currentMovePath.IsLastMove)
            {
                MakeNextMove();
            }
            else
            {
                OnFighterMoved?.Invoke(this);
            }
        }
        #endregion

        #region Getting children & bindings setup
        private bool TrySetupEntitiyVisualMoveController()
        {
            _movementController = GetComponentInChildren<EntityVisualMovementController>();
            if (_movementController == null)
            {
                return false;
            }
            
            _movementController.onMoveEnded += OnFighterArrivedAtCell;
            return true;
        }

        private bool TrySetupEntitiyVisualAnimationController()
        {
            _animationController = GetComponentInChildren<EntityVisualAnimationController>();
            if (_animationController == null)
            {
                return false;
            }
            return true;
        }

        private void OnDisable()
        {
            if (_movementController != null)
            {
                _movementController.onMoveEnded -= OnFighterArrivedAtCell;
            }
        }
        #endregion

        #if UNITY_EDITOR
            public void SetFighterConfigurationForTests(FighterConfigurationSO fighterConfiguration)
            {
                FighterConfiguration = fighterConfiguration;
            }

            public void SetStatsForTests(FighterStats newStats = null)
            {
                if (newStats != null)
                {
                    _stats = newStats;
                }
                else
                {
                    _stats = new();
                    ResetStatsToDefaultConfiguration();
                }
            }

            public FighterStats GetStatsForTests()
            {
                return _stats;
            }
        #endif
    }
}