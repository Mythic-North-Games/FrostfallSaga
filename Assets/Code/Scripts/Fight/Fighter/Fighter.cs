using System;
using System.Linq;
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
        public Action<Fighter> onFighterMoved;
        public Action<Fighter> onFighterDirectAttackEnded;
        public Action<Fighter> onFighterActiveAbilityEnded;

        [SerializeField] private EntityVisualAnimationController _animationController;
        [SerializeField] private EntityVisualMovementController _movementController;
        private MovePath _currentMovePath;
        private FighterStats _stats = new();

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

            ResetStatsToDefaultConfiguration();
        }

        /// <summary>
        /// Makes the fighter move along the given cells path.
        /// If fighter does not have enough move points, an ArgumentOutOfRangeException is thrown.
        /// </summary>
        /// <param name="cellsPath">The cells path to follow.</param>
        /// <exception cref="ArgumentOutOfRangeException">Raised if the fighter does not have enough move points.</exception>
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
        /// Makes the fighter attack the given targeted cells with its weapon.
        /// </summary>
        /// <param name="targetedCells">The cells to use the direct attack on.</param>
        /// <exception cref="ArgumentException">Raised if targeted cells are empty.</exception>
        /// <exception cref="InvalidOperationException">Raised if not enough action points.</exception>
        public void UseDirectAttack(Cell[] targetedCells)
        {
            if (targetedCells.Length == 0)
            {
                throw new ArgumentException("A direct attack can't be used without one or more target cells");
            }

            if (FighterConfiguration.DirectAttackActionPointsCost > _stats.actionPoints)
            {
                throw new InvalidOperationException("Fighter " + name + " does not have enough actions points to use its direct attack.");
            }

            PlayAnimationIfAny(FighterConfiguration.DirectAttackAnimationStateName);
            _stats.actionPoints -= FighterConfiguration.DirectAttackActionPointsCost;
            foreach (Cell targetedCell in targetedCells)
            {
                Fighter targetedCellFighter = targetedCell.GetComponent<CellFightBehaviour>().Fighter;
                if (targetedCellFighter != null)
                {
                    ApplyEffectsOnFighter(FighterConfiguration.DirectAttackEffects, targetedCellFighter);
                }
            }
            
            onFighterDirectAttackEnded?.Invoke(this);
        }

        /// <summary>
        /// Makes the fighter use the given active ability on the given cells.
        /// </summary>
        /// <param name="activeAbilityToAnimation">The active ability to use.</param>
        /// <param name="targetedCells">The target cells for the ability.</param>
        /// <exception cref="ArgumentException">Raised if targeted cells are empty.</exception>
        /// <exception cref="InvalidOperationException">Raised if not enough action points.</exception>
        public void UseActiveAbility(ActiveAbilityToAnimation activeAbilityToAnimation, Cell[] targetedCells)
        {
            if (targetedCells.Length == 0)
            {
                throw new ArgumentException("An active ability can't be used without one or more target cells");
            }

            if (_stats.actionPoints < activeAbilityToAnimation.activeAbility.ActionPointsCost)
            {
                throw new InvalidOperationException(
                    "Fighter : " + name + " does not have enough action points to use its ability " 
                    + activeAbilityToAnimation.activeAbility.Name
                );
            }

            PlayAnimationIfAny(activeAbilityToAnimation.animationState);
            _stats.actionPoints -= activeAbilityToAnimation.activeAbility.ActionPointsCost;
            foreach (Cell cell in targetedCells)
            {
                Fighter target = cell.GetComponent<CellFightBehaviour>().Fighter;
                if (target != null)
                {
                    ApplyEffectsOnFighter(activeAbilityToAnimation.activeAbility.Effects, target);
                }
            }

            onFighterActiveAbilityEnded?.Invoke(this);
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
        /// Method to withstand a magical attack.
        /// </summary>
        /// <param name="magicalDamageAmount">The damage amount to resist to.</param>
        /// <param name="magicalElement">The magical element to resist.</param>
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
        /// Heals the fighter by the given amount.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public void Heal(int healAmount)
        {
            PlayAnimationIfAny(FighterConfiguration.HealSelfAnimationStateName);
            _stats.health = Math.Clamp(_stats.health + healAmount, 0, _stats.maxHealth);
        }

        #region Stats getter

        public int GetMovePoints()
        {
            return _stats.movePoints;
        }

        public int GetActionPoints()
        {
            return _stats.actionPoints;
        }

        #endregion

        /// <summary>
        /// Play an animation if the given state name exists.
        /// </summary>
        /// <param name="animationStateName">The animation state to launch.</param>
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
        /// Apply a list of effects to the targeted fighter.
        /// </summary>
        /// <param name="effectsToApply">The effects to apply.</param>
        /// <param name="target">The fighter to apply the effects to.</param>
        private void ApplyEffectsOnFighter(AEffectSO[] effectsToApply, Fighter target)
        {
            effectsToApply.ToList().ForEach(effect => effect.ApplyEffect(target));
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
                onFighterMoved?.Invoke(this);
            }
        }
        #endregion

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