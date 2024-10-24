using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Abilities.AbilityAnimation;
using FrostfallSaga.Fight.Statuses;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : MonoBehaviour
    {
        [field: SerializeField] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
        [field: SerializeField] public FighterMouseEventsController FighterMouseEventsController { get; private set; }
        [field: SerializeField] public StatusesManager StatusesManager { get; private set; }
        [field: SerializeField] public Transform CameraAnchor { get; private set; }
        public Sprite FighterIcon { get; private set; }

        public string EntitySessionId { get; private set; }
        public Cell cell;
        public bool IsParalyzed { get; private set; }

        public Action<Fighter> onFighterMoved;
        public Action<Fighter> onFighterDirectAttackEnded;
        public Action<Fighter> onFighterActiveAbilityEnded;

        // <Fighter that dodged, Fighter that attacked, Effect that was dodged>
        public Action<Fighter, Fighter, AEffectSO> onEffectDodged;

        // <Fighter that received, Fighter that attacked, Effect that was received, If masterstroke>
        public Action<Fighter, Fighter, AEffectSO, bool> onEffectReceived;

        // <Fighter that received, Status that was applied>
        public Action<Fighter, AStatus> onStatusApplied;

        // <Fighter that received, Status that was removed>
        public Action<Fighter, AStatus> onStatusRemoved;

        public Action<Fighter> onFighterDied;

        private MovePath _currentMovePath;
        private FighterStats _stats = new();
        private FighterStats _initialStats = new();
        private FighterClassSO _fighterClass;
        public TargeterSO DirectAttackTargeter { get; private set; }
        public int DirectAttackActionPointsCost { get; private set; }
        public AEffectSO[] DirectAttackEffects { get; private set; }
        public ActiveAbilityToAnimation[] ActiveAbilities { get; private set; }
        private AAbilityAnimationSO _directAttackAnimation;
        private string _receiveDamageAnimationName;
        private string _healSelfAnimationName;
        private string _reduceStatAnimationName;
        private string _increaseStatAnimationName;
        private ActiveAbilityToAnimation _currentActiveAbility;

        public Fighter()
        {
            StatusesManager = new StatusesManager(this);
        }

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
            _fighterClass = fighterSetup.fighterClassSO;
            DirectAttackTargeter = fighterSetup.directAttackTargeter;
            DirectAttackActionPointsCost = fighterSetup.directAttackActionPointsCost;
            DirectAttackEffects = fighterSetup.directAttackEffects;
            _directAttackAnimation = fighterSetup.directAttackAnimation;
            ActiveAbilities = fighterSetup.activeAbilities;
            _receiveDamageAnimationName = fighterSetup.receiveDamageAnimationName;
            _healSelfAnimationName = fighterSetup.healSelfAnimationName;
            _reduceStatAnimationName = fighterSetup.reduceStatAnimationName;
            _increaseStatAnimationName = fighterSetup.increaseStatAnimationName;
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

            ActiveAbilitySO activeAbilityToUse = activeAbilityToAnimation.activeAbility;
            if (activeAbilityToAnimation.animation == null)
            {
                Debug.LogWarning($"No animation attached to active ability {activeAbilityToAnimation.activeAbility.Name} for fighter {name}");
                targetedCells.ToList()
                    .Where(cell => cell.GetComponent<CellFightBehaviour>().Fighter != null).ToList()
                    .ForEach(cell =>
                    {
                        Fighter targetedFighter = cell.GetComponent<CellFightBehaviour>().Fighter;
                        ApplyStatusesOnFighter(activeAbilityToUse.Statuses, targetedFighter);
                        ApplyEffectsOnFighter(activeAbilityToUse.Effects, targetedFighter);
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
            _stats.actionPoints -= activeAbilityToUse.ActionPointsCost;
        }

        /// <summary>
        /// Method to withstand a physical attack.
        /// </summary>
        /// <param name="physicalDamageAmount">The damage taken before withstanding.</param>
        public void PhysicalWithstand(int physicalDamageAmount)
        {
            PlayAnimationIfAny(_receiveDamageAnimationName);
            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            Debug.Log($"{name} + received {inflictedPhysicalDamageAmount} physical damages");
            DecreaseHealth(inflictedPhysicalDamageAmount);
        }

        /// <summary>
        /// Method to withstand a magical attack.
        /// </summary>
        /// <param name="magicalDamageAmount">The damage amount to resist to.</param>
        /// <param name="magicalElement">The magical element to resist.</param>
        public void MagicalWithstand(int magicalDamageAmount, EMagicalElement magicalElement)
        {

            if (!_stats.magicalResistances.ContainsKey(magicalElement))
            {
                throw new NullReferenceException($"Magical resistance element {magicalElement} is not set for fighter {name}");
            }

            PlayAnimationIfAny(_receiveDamageAnimationName);
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
            PlayAnimationIfAny(_healSelfAnimationName);
            IncreaseHealth(healAmount);
            Debug.Log($"{name} + has been healed by {healAmount}.");
        }

        /// <summary>
        /// Receives raw damages without any resistance calculation.
        /// </summary>
        /// <param name="damages">The damages to receive.</param>
        public void ReceiveRawDamages(int damages)
        {
            PlayAnimationIfAny(_receiveDamageAnimationName);
            DecreaseHealth(damages);
            Debug.Log($"{name} + received {damages} raw damages.");
        }

        /// <summary>
        /// Updates a mutable stat by the given amount. For magical resistances and magical strengths use ReduceMagicalStat method instead.
        /// </summary>
        /// <param name="mutableStat">The mutable stat to increase or reduce.</param>
        /// <param name="amount">The amount to increase or reduce.</param>
        /// <param name="triggerAnimation">True to trigger the stat update animation, false otherwise.</param>
        public void UpdateMutableStat(EFighterMutableStat mutableStat, int amount, bool triggerAnimation = true)
        {
            switch (mutableStat)
            {
                case EFighterMutableStat.MaxHealth:
                    Math.Min(0, _stats.maxHealth += amount);
                    break;
                case EFighterMutableStat.MaxActionPoints:
                    Math.Min(0, _stats.maxActionPoints += amount);
                    break;
                case EFighterMutableStat.MaxMovePoints:
                    Math.Min(0, _stats.maxMovePoints += amount);
                    break;
                case EFighterMutableStat.Strength:
                    Math.Min(0, _stats.strength += amount);
                    break;
                case EFighterMutableStat.Dexterity:
                    Math.Min(0, _stats.dexterity += amount);
                    break;
                case EFighterMutableStat.Tenacity:
                    Math.Min(0, _stats.tenacity += amount);
                    break;
                case EFighterMutableStat.DodgeChance:
                    Math.Min(0, _stats.dodgeChance += amount);
                    break;
                case EFighterMutableStat.PhysicalResistance:
                    Math.Min(0, _stats.physicalResistance += amount);
                    break;
                case EFighterMutableStat.MasterstrokeChance:
                    Math.Min(0, _stats.masterstrokeChance += amount);
                    break;
                case EFighterMutableStat.Initiative:
                    Math.Min(0, _stats.initiative += amount);
                    break;
                default:
                    Debug.LogWarning($"Unmanaged stat to reduce: {mutableStat}. If you want to modify a magical stat, use ReduceMagicalStat method instead.");
                    return;
            }

            if (triggerAnimation)
            {
                PlayAnimationIfAny(amount > 0 ? _increaseStatAnimationName : _reduceStatAnimationName);
            }
        }

        /// <summary>
        /// Reduces or increases a magical stat by the given amount. To update other stats use UpdateMutableStat method instead.
        /// </summary>
        /// <param name="magicalElement">The related magical element.</param>
        /// <param name="amount">The amount to increase or reduce.</param>
        /// <param name="isResistance">True to update magical resistance stat, false to update magical strength</param>
        /// <param name="triggerAnimation">True to trigger the stat update animation, false otherwise.</param>
        public void UpdateMagicalStat(EMagicalElement magicalElement, int amount, bool isResistance, bool triggerAnimation = true)
        {
            try
            {
                if (isResistance)
                {
                    _stats.magicalResistances[magicalElement] = Math.Min(0, _stats.magicalResistances[magicalElement] += amount);
                }
                else
                {
                    _stats.magicalStrengths[magicalElement] = Math.Min(0, _stats.magicalStrengths[magicalElement] += amount);
                }

                if (triggerAnimation)
                {
                    PlayAnimationIfAny(amount > 0 ? _increaseStatAnimationName : _reduceStatAnimationName);
                }
                Debug.Log($"{name} {magicalElement} {(isResistance ? "resistance" : "strength")} has been modified by {amount}.");
            }
            catch (NullReferenceException)
            {
                Debug.LogError($"Magical element {magicalElement} is not set for fighter {name}");
            }
        }

        /// <summary>
        /// Set the fighter as paralyzed or not.
        /// </summary>
        /// <param name="isParalyzed">True if the fighter is paralized.</param>
        public void SetIsParalyzed(bool isParalyzed)
        {
            IsParalyzed = isParalyzed;
        }

        /// <summary>
        /// Apply a new status to the fighter.
        /// </summary>
        /// <param name="status">The new status to apply.</param>
        public void ApplyStatus(AStatus status)
        {
            StatusesManager.ApplyStatus(status);
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
            ApplyStatusesOnFighter(_currentActiveAbility.activeAbility.Statuses, touchedFighter);
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

        public int GetMovePoints() => _stats.movePoints;

        public int GetActionPoints() => _stats.actionPoints;

        public int GetHealth() => _stats.health;

        public int GetStrength() => _stats.strength;

        public float GetDodgeChance() => _stats.dodgeChance;

        public float GetMasterstrokeChance() => _stats.masterstrokeChance;

        public int GetInitiative() => _stats.initiative;

        public FighterCollider GetWeaponCollider()
        {
            return GetComponent<FighterCollider>();
        }

        public void ResetMovementAndActionPoints()
        {
            _stats.actionPoints = _stats.maxActionPoints;
            _stats.movePoints = _stats.maxMovePoints;
        }

        private void ResetStatsToDefaultConfiguration()
        {
            _stats.maxHealth = _initialStats.maxHealth + _fighterClass.classMaxHealth;
            _stats.health = _initialStats.maxHealth;
            _stats.maxActionPoints = _initialStats.maxActionPoints + _fighterClass.classMaxActionPoints;
            _stats.actionPoints = _initialStats.maxActionPoints;
            _stats.maxMovePoints = _initialStats.maxMovePoints + _fighterClass.classMaxMovePoints;
            _stats.movePoints = _initialStats.maxMovePoints;
            _stats.strength = _initialStats.strength + _fighterClass.classMaxMovePoints;
            _stats.dexterity = _initialStats.dexterity + _fighterClass.classDexterity;
            _stats.tenacity = _initialStats.tenacity + _fighterClass.classTenacity;
            _stats.physicalResistance = _initialStats.physicalResistance + _fighterClass.classPhysicalResistance;

            _stats.magicalResistances = _initialStats.magicalResistances;
            _stats.AddMagicalResistances(MagicalElementToValue.GetDictionaryFromArray(_fighterClass.classMagicalResistances));

            _stats.magicalStrengths = _initialStats.magicalStrengths;
            _stats.AddMagicalStrengths(MagicalElementToValue.GetDictionaryFromArray(_fighterClass.classMagicalStrengths));

            _stats.dodgeChance = _initialStats.dodgeChance + _fighterClass.classDodgeChance;
            _stats.masterstrokeChance = _initialStats.masterstrokeChance + _fighterClass.classMasterstrokeChance;
            _stats.initiative = _initialStats.initiative + _fighterClass.classInitiative;
        }

        #endregion

        #region Actions feasability

        /// <summary>
        /// Returns whether the fighter can move in the given context or not.
        /// Move points > 0 && at least one cell around him is free (no fighter, no obstacle, height accessible...).
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
        public bool CanDirectAttack(HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return (
                DirectAttackActionPointsCost <= _stats.actionPoints &&
                DirectAttackTargeter.AtLeastOneCellResolvable(fightGrid, cell, fightersTeams)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use one if its active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he has enough actions points and if an active ability targeter can be resolved around him.</returns>
        public bool CanUseAtLeastOneActiveAbility(HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return ActiveAbilities.Any(
                activeAbilityToAnimation => CanUseActiveAbility(fightGrid, activeAbilityToAnimation.activeAbility, fightersTeams)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use the given active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <param name="activeAbility">The active ability to check if it can be used.</param>
        /// <returns>True if he has enough actions points and if the active ability targeter can be resolved around him.</returns>
        public bool CanUseActiveAbility(HexGrid fightGrid, ActiveAbilitySO activeAbility, Dictionary<Fighter, bool> fightersTeams)
        {
            return (
                activeAbility.ActionPointsCost <= _stats.actionPoints &&
                activeAbility.Targeter.AtLeastOneCellResolvable(fightGrid, cell, fightersTeams)
            );
        }

        /// <summary>
        /// Returns whether the fighter can act in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he can move, direct attack or use one of its active ability.</returns>
        public bool CanAct(HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return CanMove(fightGrid) || CanDirectAttack(fightGrid, fightersTeams) || CanUseAtLeastOneActiveAbility(fightGrid, fightersTeams);
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
            effectsToApply.ToList().ForEach(effect => effect.ApplyEffect(this, target, effect.Masterstrokable, effect.Dodgable));
        }

        /// <summary>
        /// Apply a list of statuses to the targeted fighter.
        /// </summary>
        /// <param name="statusesToApply">The statuses to apply.</param>
        /// <param name="target">The fighter to apply the statuses to.</param>
        private void ApplyStatusesOnFighter(AStatus[] statusesToApply, Fighter target)
        {
            statusesToApply.ToList().ForEach(status => target.ApplyStatus(status));
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