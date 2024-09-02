using System;
using System.Linq;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
        [field:SerializeField] public Transform CameraAnchor { get; private set; }
        public Sprite FighterIcon { get; private set; }
        public string EntitySessionId { get; private set; }
        public Cell cell;

        public Action<Fighter> onFighterMoved;
        public Action<Fighter> onFighterDirectAttackEnded;
        public Action<Fighter> onFighterActiveAbilityEnded;
        public Action<Fighter> onFighterDied;

        private MovePath _currentMovePath;
        private FighterStats _stats = new();
        private FighterStats _initialStats = new();
        public TargeterSO DirectAttackTargeter { get; private set; }
        public int DirectAttackActionPointsCost { get; private set; }
        public AEffectSO[] DirectAttackEffects { get; private set; }
        public ActiveAbilityToAnimation[] ActiveAbilities { get; private set; }
        private AAbilityAnimationSO _directAttackAnimation;
        private string _receiveDamageAnimationStateName;
        private string _healSelfAnimationStateName;
        private ActiveAbilityToAnimation _currentActiveAbility;

        private void Awake()
        {
            if (!TrySetupEntitiyVisualMoveController())
            {
                Debug.LogError("No entity visual move controller found for fighter " + name);
            }
            if (!TrySetupEntitiyVisualAnimationController())
            {
                Debug.LogError("No entity visual animation controller found for fighter " + name);
            }
        }

        /// <summary>
        /// Meant to be called when generating the fighter before the fight begins.
        /// </summary>
        public void Setup(FighterSetup fighterSetup)
        {
            EntitySessionId = fighterSetup.sessionId;
            FighterIcon = fighterSetup.icon;
            _initialStats = fighterSetup.initialStats;
            DirectAttackTargeter = fighterSetup.directAttackTargeter;
            DirectAttackActionPointsCost = fighterSetup.directAttackActionPointsCost;
            DirectAttackEffects = fighterSetup.directAttackEffects;
            _directAttackAnimation = fighterSetup.directAttackAnimation;
            ActiveAbilities = fighterSetup.activeAbilities;
            _receiveDamageAnimationStateName = fighterSetup.receiveDamageAnimationStateName;
            _healSelfAnimationStateName = fighterSetup.healSelfAnimationStateName;

            ResetStatsToDefaultConfiguration();
        }

        #region Concrete actions

        /// <summary>
        /// Makes the fighter move along the given cells path.
        /// If fighter does not have enough move points, an ArgumentOutOfRangeException is thrown.
        /// Don't forget to listen to `onFighterMoved` event if you want to know when he's done moving.
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

            if (DirectAttackActionPointsCost > _stats.actionPoints)
            {
                throw new InvalidOperationException("Fighter " + name + " does not have enough actions points to use its direct attack.");
            }

            if (_directAttackAnimation == null)
            {
                Debug.LogWarning($"No animation attached to direct attack for fighter {name}");
                targetedCells.ToList()
                    .Where(cell => cell.GetComponent<CellFightBehaviour>().Fighter != null).ToList()
                    .ForEach(cell =>
                    {
                        ApplyEffectsOnFighter(
                            DirectAttackEffects,
                            cell.GetComponent<CellFightBehaviour>().Fighter
                        );
                    });
                onFighterDirectAttackEnded?.Invoke(this);
            }
            else
            {
                _directAttackAnimation.onFighterTouched += OnDirectAttackTouchedFighter;
                _directAttackAnimation.onAnimationEnded += OnDirectAttackAnimationEnded;
                _directAttackAnimation.Execute(this, targetedCells);
            }
            _stats.actionPoints -= DirectAttackActionPointsCost;
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

            if (activeAbilityToAnimation.animation == null)
            {
                Debug.LogWarning($"No animation attached to active ability {activeAbilityToAnimation.activeAbility.Name} for fighter {name}");
                targetedCells.ToList()
                    .Where(cell => cell.GetComponent<CellFightBehaviour>().Fighter != null).ToList()
                    .ForEach(cell =>
                    {
                        ApplyEffectsOnFighter(
                            activeAbilityToAnimation.activeAbility.Effects,
                            cell.GetComponent<CellFightBehaviour>().Fighter
                        );
                    });
                onFighterActiveAbilityEnded?.Invoke(this);
            }
            else
            {
                _currentActiveAbility = activeAbilityToAnimation;
                _currentActiveAbility.animation.onFighterTouched += OnActiveAbilityTouchedFighter;
                _currentActiveAbility.animation.onAnimationEnded += OnActiveAbilityAnimationEnded;
                _currentActiveAbility.animation.Execute(this, targetedCells);
            }
            _stats.actionPoints -= activeAbilityToAnimation.activeAbility.ActionPointsCost;
        }

        /// <summary>
        /// Method to withstand a physical attack.
        /// </summary>
        /// <param name="physicalDamageAmount">The damage taken before withstanding.</param>
        public void PhysicalWithstand(int physicalDamageAmount)
        {

            PlayAnimationIfAny(_receiveDamageAnimationStateName);
            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            Debug.Log($"{name} + received {inflictedPhysicalDamageAmount} physical damages");
            DecreaseHealth(inflictedPhysicalDamageAmount);
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
                throw new NullReferenceException($"Magical resistance element {magicalElement} is not set for fighter {name}");
            }

            PlayAnimationIfAny(_receiveDamageAnimationStateName);
            int inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.magicalResistances[magicalElement]);
            Debug.Log($"{name} + received {inflictedMagicalDamageAmount} {magicalElement} magical damages");
            DecreaseHealth(inflictedMagicalDamageAmount);
        }

        /// <summary>
        /// Heals the fighter by the given amount.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        public void Heal(int healAmount)
        {
            PlayAnimationIfAny(_healSelfAnimationStateName);
            IncreaseHealth(healAmount);
        }

        #endregion

        #region Actions process management

        public void OnDirectAttackTouchedFighter(Fighter touchedFighter)
        {
            ApplyEffectsOnFighter(DirectAttackEffects, touchedFighter);
        }

        public void OnDirectAttackAnimationEnded(Fighter initiator)
        {
            _directAttackAnimation.onFighterTouched -= OnDirectAttackTouchedFighter;
            _directAttackAnimation.onAnimationEnded -= OnDirectAttackAnimationEnded;

            onFighterDirectAttackEnded?.Invoke(initiator);
        }

        public void OnActiveAbilityTouchedFighter(Fighter touchedFighter)
        {
            ApplyEffectsOnFighter(_currentActiveAbility.activeAbility.Effects, touchedFighter);
        }

        public void OnActiveAbilityAnimationEnded(Fighter initiator)
        {
            _currentActiveAbility.animation.onFighterTouched -= OnActiveAbilityTouchedFighter;
            _currentActiveAbility.animation.onAnimationEnded -= OnActiveAbilityAnimationEnded;

            onFighterActiveAbilityEnded?.Invoke(initiator);
        }

        #endregion

        #region Stats getters & manipulation

        public int GetMovePoints()
        {
            return _stats.movePoints;
        }

        public int GetActionPoints()
        {
            return _stats.actionPoints;
        }

        public int GetHealth()
        {
            return _stats.health;
        }

        public int GetInitiative()
        {
            return _stats.initiative;
        }

        public FighterCollider GetWeaponCollider()
        {
            return GetComponentInChildren<FighterCollider>();
        }

        public void ResetMovementAndActionPoints()
        {
            _stats.actionPoints = _stats.maxActionPoints;
            _stats.movePoints = _stats.maxMovePoints;
        }

        #endregion

        #region Actions feasability

        /// <summary>
        /// Returns whether the fighter can move in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he has enough move points and if there is at least on cell free around him.</returns>
        public bool CanMove(HexGrid fightGrid)
        {
            return _stats.movePoints > 0 && FightCellNeighbors.GetNeighbors(fightGrid, cell).Length > 0;
        }

        /// <summary>
        /// Returns whether the fighter can use its direct attack in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he has enough actions points and if the direct attack targeter can be resolved around him.</returns>
        public bool CanDirectAttack(HexGrid fightGrid)
        {
            return (
                DirectAttackActionPointsCost <= _stats.actionPoints &&
                DirectAttackTargeter.AtLeastOneCellResolvable(fightGrid, cell)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use one if its active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he has enough actions points and if an active ability targeter can be resolved around him.</returns>
        public bool CanUseAtLeastOneActiveAbility(HexGrid fightGrid)
        {
            return ActiveAbilities.Any(
                activeAbilityToAnimation => CanUseActiveAbility(fightGrid, activeAbilityToAnimation.activeAbility)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use the given active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <param name="activeAbility">The active ability to check if it can be used.</param>
        /// <returns>True if he has enough actions points and if the active ability targeter can be resolved around him.</returns>
        public bool CanUseActiveAbility(HexGrid fightGrid, ActiveAbilitySO activeAbility)
        {
            return (
                activeAbility.ActionPointsCost <= _stats.actionPoints &&
                activeAbility.Targeter.AtLeastOneCellResolvable(fightGrid, cell)
            );
        }

        /// <summary>
        /// Returns whether the fighter can act in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he can move, direct attack or use one of its active ability.</returns>
        public bool CanAct(HexGrid fightGrid)
        {
            return CanMove(fightGrid) || CanDirectAttack(fightGrid) || CanUseAtLeastOneActiveAbility(fightGrid);
        }

        #endregion

        #region Private helpers
        /// <summary>
        /// Play an animation if the given state name exists.
        /// </summary>
        /// <param name="animationStateName">The animation state to launch.</param>
        private void PlayAnimationIfAny(string animationStateName)
        {
            try
            {
                AnimationController.PlayAnimationState(animationStateName);
            }
            catch (Exception)
            {
                if (animationStateName != null)
                {
                    Debug.LogWarning($"Animation named: {animationStateName} not found for fighter {name}");
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

        private void DecreaseHealth(int amount)
        {
            _stats.health = Math.Clamp(_stats.health - amount, 0, _stats.maxHealth);
            if (_stats.health == 0)
            {
                onFighterDied?.Invoke(this);
            }
        }

        private void IncreaseHealth(int amount)
        {
            _stats.health = Math.Clamp(_stats.health + amount, 0, _stats.maxHealth);
            if (_stats.health == 0)
            {
                onFighterDied?.Invoke(this);
            }
        }
        #endregion

        #region Movement handling
        private void MakeNextMove()
        {
            Cell cellToMoveTo = _currentMovePath.GetNextCellInPath();
            cell.GetComponent<CellFightBehaviour>().Fighter = null;
            MovementController.Move(cell, cellToMoveTo, _currentMovePath.IsLastMove);
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
            _stats.maxHealth = _initialStats.maxHealth;
            _stats.health = _initialStats.maxHealth;
            _stats.maxActionPoints = _initialStats.maxActionPoints;
            _stats.actionPoints = _initialStats.maxActionPoints;
            _stats.maxMovePoints = _initialStats.maxMovePoints;
            _stats.movePoints = _initialStats.maxMovePoints;
            _stats.strength = _initialStats.strength;
            _stats.dexterity = _initialStats.dexterity;
            _stats.physicalResistance = _initialStats.physicalResistance;
            _stats.magicalResistances = _initialStats.magicalResistances;
            _stats.magicalStrengths = _initialStats.magicalStrengths;
            _stats.initiative = _initialStats.initiative;
        }

        #region Getting children & bindings setup
        public bool TrySetupEntitiyVisualMoveController()
        {
            MovementController = GetComponentInChildren<EntityVisualMovementController>();
            if (MovementController == null)
            {
                return false;
            }

            MovementController.onMoveEnded += OnFighterArrivedAtCell;
            return true;
        }

        private bool TrySetupEntitiyVisualAnimationController()
        {
            AnimationController = GetComponentInChildren<EntityVisualAnimationController>();
            if (AnimationController == null)
            {
                return false;
            }
            return true;
        }

        public void UnsubscribeToMovementControllerEvents()
        {
            if (MovementController != null)
            {
                MovementController.onMoveEnded -= OnFighterArrivedAtCell;
            }
        }

        private void OnDisable()
        {
            UnsubscribeToMovementControllerEvents();
        }
        #endregion

#if UNITY_EDITOR
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