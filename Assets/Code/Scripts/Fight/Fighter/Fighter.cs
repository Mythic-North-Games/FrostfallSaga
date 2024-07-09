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
        [SerializeField] private EntityVisualAnimationController _entityVisualAnimationController;
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
        /// <summary>
        /// Use a simple attack to the targeted cell(s)
        /// </summary>
        /// <param name="targetedCells"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// <summary>
        /// Apply the simple attack effect(s) to the targeted fighter
        /// </summary>
        /// <param name="fighter"></param>
        private void ApplyDirectAttackEffectsToFighter(Fighter fighter)
        {
            foreach (AEffectSO effect in FighterConfiguration.DirectAttackEffects)
            {
                effect.ApplyEffect(fighter);
            }
        }
        /// <summary>
        /// Method to withstand a physical attack
        /// </summary>
        /// <param name="physicalDamageAmount"></param>
        public void PhysicalWithstand(int physicalDamageAmount)
        {

            PlayAnimationIfAny(FighterConfiguration.ReceiveDamageAnimationStateName);
            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            _stats.health = Math.Clamp(_stats.health - inflictedPhysicalDamageAmount, 0, _stats.maxHealth);
        }
        /// <summary>
        /// Method to withstand a magical attack
        /// </summary>
        /// <param name="magicalDamageAmount"></param>
        /// <param name="magicalElement"></param>
        /// <exception cref="NullReferenceException"></exception>
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
        /// <summary>
        /// Heal the fighter
        /// </summary>
        /// <param name="healAmount"></param>

        public void Heal(int healAmount)
        {
            PlayAnimationIfAny(FighterConfiguration.HealSelfAnimationStateName);
            _stats.health = Math.Clamp(_stats.health + healAmount, 0, _stats.maxHealth);
        }

        /// <summary>
        /// Play an animation
        /// </summary>
        /// <param name="animationStateName"></param>
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

        /// <summary>
        /// Applies an active ability of a fighter to a series of cells
        /// </summary>
        /// <param name="activeAbilityToAnimation"></param>
        /// <param name="cells"></param>
        public void UseActiveAbility(ActiveAbilitiesToAnimation activeAbilityToAnimation, Cell[] cells)
        {
            PlayAnimationIfAny(activeAbilityToAnimation.animationState);
            if (_stats.actionPoints < activeAbilityToAnimation.activeAbility.ActionPointsCost)
            {
                throw new InvalidOperationException("Fighter : " + name + " does not have enough action points");
            }
            _stats.actionPoints = _stats.actionPoints - activeAbilityToAnimation.activeAbility.ActionPointsCost;

            foreach (Cell cell in cells)
            {
                Fighter target = cell.GetComponent<CellFightBehaviour>().Fighter;
                if (target) 
                {
                    ApplyEffectsOnFighter(activeAbilityToAnimation.activeAbility.Effects, target);
                }
            }
        }
        /// <summary>
        /// Apply an effect to the targeted fighter 
        /// </summary>
        /// <param name="aEffects"></param>
        /// <param name="target"></param>
        private void ApplyEffectsOnFighter(AEffectSO[] aEffects, Fighter target)
        {
            
            foreach (var effect in aEffects)
            {
                effect.ApplyEffect(target);
            }
        }
        /// <summary>
        /// Play the active ability animation
        /// </summary>
        /// <param name="animationState"></param>
        private void PlayActiveAbilityAnimationIfAny(string animationState)
        {
            try
            {
                _entityVisualAnimationController.PlayAnimationState(animationState);

            }
            catch (Exception e)
            {
                Debug.LogWarning("Error " + animationState + " : " + e.Message);
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