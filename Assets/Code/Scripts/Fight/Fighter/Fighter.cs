using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Core.Entities;
using FrostfallSaga.Core.Fight;
using FrostfallSaga.InventorySystem;
using FrostfallSaga.Grid;
using FrostfallSaga.Grid.Cells;
using FrostfallSaga.EntitiesVisual;
using FrostfallSaga.Fight.FightItems;
using FrostfallSaga.Fight.FightCells;
using FrostfallSaga.Fight.FightCells.Impediments;
using FrostfallSaga.Fight.FightCells.FightCellAlterations;
using FrostfallSaga.Fight.Targeters;
using FrostfallSaga.Fight.Abilities;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Statuses;
using FrostfallSaga.Utils;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : AFighter
    {
        ////////////////////////////////////////.
        // Visuals and Game object controllers //
        ////////////////////////////////////////.
        [field: SerializeField, Header("Visuals and GO controllers")] public EntityVisualAnimationController AnimationController { get; private set; }
        [field: SerializeField] public EntityVisualMovementController MovementController { get; private set; }
        [field: SerializeField] public FighterMouseEventsController FighterMouseEventsController { get; private set; }
        [field: SerializeField] public Transform CameraAnchor { get; private set; }

        ////////////////////////
        // Mechanics managers //
        ////////////////////////
        [field: SerializeField, Header("Mechanics managers")] public StatusesManager StatusesManager { get; private set; }
        [field: SerializeField] public PassiveAbilitiesManager PassiveAbilitiesManager { get; private set; }
        [field: SerializeField] public DirectAttackManager DirectAttackManager { get; private set; }

        //////////////////////
        // Fight properties //
        //////////////////////
        [field: SerializeField, Header("Fight properties")] public FighterConfigurationSO FighterConfiguration { get; private set; }
        [field: SerializeField] public EEntityRace Race { get; private set; }
        [SerializeField] private FighterStats _stats = new();
        [field: SerializeField] private FighterStats _initialStats = new();
        [SerializeField] private int _godFavorsPoints;
        [field: SerializeField] public FighterClassSO FighterClass { get; private set; }
        [field: SerializeField] public PersonalityTraitSO PersonalityTrait { get; private set; }
        [SerializeField] private Inventory _inventory;
        [field: SerializeField] public WeaponSO Weapon { get; private set; }
        [field: SerializeField] public ActiveAbilitySO[] ActiveAbilities { get; private set; }
        [field: SerializeField] public PassiveAbilitySO[] PassiveAbilities { get; private set; }
        [field: SerializeField] public bool IsParalyzed { get; private set; }

        /////////////////////////////////////
        // Movements & location properties //
        /////////////////////////////////////
        [Header("Movements & location properties")] public FightCell cell;
        [SerializeField] private MovePath _currentMovePath;

        ////////////////
        // Animations //
        ////////////////
        [SerializeField, Header("Animations")] private string _receiveDamageAnimationName;
        [SerializeField] private string _healSelfAnimationName;
        [SerializeField] private string _reduceStatAnimationName;
        [SerializeField] private string _increaseStatAnimationName;

        ////////////////
        // For the UI //
        ////////////////
        [field: SerializeField, Header("For the UI")] public string FighterName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Sprite DiamondIcon { get; private set; }


        ////////////
        // Events //
        ////////////

        // <Fighter that moved>
        public Action<Fighter> onFighterMoved;

        // <Fighter that is going to attack>
        public Action<Fighter> onDirectAttackStarted;

        // <Fighter that attacked>
        public Action<Fighter> onDirectAttackEnded;

        // <Fighter that used an active ability>
        public Action<Fighter, ActiveAbilitySO> onActiveAbilityStarted;

        // <Fighter that used an active ability>
        public Action<Fighter, ActiveAbilitySO> onActiveAbilityEnded;

        // <Fighter that dodged>
        public Action<Fighter> onActionDodged;

        // <Fighter that received damages, the damages taken, if masterstroke>
        public Action<Fighter, int, bool> onDamageReceived;

        // <Fighter that received heal, the heal amount received, if masterstroke>
        public Action<Fighter, int, bool> onHealReceived;

        // <Fighter that received, Status that was applied>
        public Action<Fighter, AStatus> onStatusApplied;

        // <Fighter that received, Status that was removed>
        public Action<Fighter, AStatus> onStatusRemoved;

        // <Fighter that received, stat that was updated, the updated amount>
        public Action<Fighter, EFighterMutableStat, float> onNonMagicalStatMutated;

        // <Fighter that received, magical element, the updated amount, if resistance (false for strength)>
        public Action<Fighter, EMagicalElement, int, bool> onMagicalStatMutated;

        // <Fighter that received, Status that was applied>
        public Action<Fighter, PassiveAbilitySO> onPassiveAbilityApplied;

        // <Fighter that received, Status that was removed>
        public Action<Fighter, PassiveAbilitySO> onPassiveAbilityRemoved;

        // <Fighter that died>
        public Action<Fighter> onFighterDied;

        public Fighter()
        {
            StatusesManager = new StatusesManager(this);
            PassiveAbilitiesManager = new PassiveAbilitiesManager(this);
            DirectAttackManager = new DirectAttackManager(this);
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
        public void Setup(
            EntityConfigurationSO entityConfiguration,
            FighterConfigurationSO fighterConfiguration,
            ActiveAbilitySO[] equippedActiveAbilities,
            PassiveAbilitySO[] equippedPassiveAbilities,
            Inventory inventory,
            string sessionId = null
        )
        {
            EntitySessionId = sessionId;
            FighterConfiguration = fighterConfiguration;
            Race = entityConfiguration.Race;
            FighterName = entityConfiguration.Name;
            Icon = entityConfiguration.Icon;
            DiamondIcon = entityConfiguration.DiamondIcon;
            _initialStats = fighterConfiguration.ExtractFighterStats();
            FighterClass = fighterConfiguration.FighterClass;
            PersonalityTrait = fighterConfiguration.PersonalityTrait as PersonalityTraitSO;
            _inventory = inventory;
            Weapon = _inventory.GetWeapon() as WeaponSO;
            ActiveAbilities = equippedActiveAbilities;
            PassiveAbilities = equippedPassiveAbilities;
            _receiveDamageAnimationName = fighterConfiguration.ReceiveDamageAnimationName;
            _healSelfAnimationName = fighterConfiguration.HealSelfAnimationName;
            _reduceStatAnimationName = fighterConfiguration.ReduceStatAnimationName;
            _increaseStatAnimationName = fighterConfiguration.IncreaseStatAnimationName;
            ResetStatsToDefaultConfiguration();
        }

        #region Concrete actions

        /// <summary>
        /// Makes the fighter move along the given cells path.
        /// If fighter does not have enough move points, an ArgumentOutOfRangeException is thrown.
        /// Don't forget to listen to `onFighterMoved` event if you want to know when he's done moving.
        /// </summary>
        /// <param name="cellsPath">The cells path to follow.</param>
        /// <param name="goUntilAllMovePointsUsed">If the path is longer than the current move points, seting this to True will make the fighter move until he runs out of movement points. Seting it to False will raise an exception instead.</param>
        /// <exception cref="ArgumentOutOfRangeException">Raised if the fighter does not have enough move points.</exception>
        public void Move(FightCell[] cellsPath, bool goUntilAllMovePointsUsed = false)
        {
            bool pathLongerThanMovePoints = cellsPath.Length > _stats.movePoints;

            // Check if the fighter has enough move points to move
            if (!goUntilAllMovePointsUsed && pathLongerThanMovePoints)
            {
                throw new ArgumentOutOfRangeException("Fighter " + name + " does not have enough move points to move.");
            }

            // If the path is longer than the move points, we shrink it to the move points
            if (goUntilAllMovePointsUsed && pathLongerThanMovePoints)
            {
                cellsPath = cellsPath.Take(_stats.movePoints).ToArray();
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
        public void UseDirectAttack(FightCell[] targetedCells)
        {
            // Do various checks to confirm the direct attack can be used
            if (targetedCells.Length == 0)
            {
                throw new ArgumentException("A direct attack can't be used without one or more target cells");
            }

            if (Weapon.UseActionPointsCost > _stats.actionPoints)
            {
                throw new InvalidOperationException("Fighter " + name + " does not have enough actions points to use its direct attack.");
            }

            // Trigger the direct attack
            onDirectAttackStarted?.Invoke(this);
            _stats.actionPoints -= Weapon.UseActionPointsCost;
            DirectAttackManager.onDirectAttackEnded += OnDirectAttackEnded;
            DirectAttackManager.DirectAttack(targetedCells.ToList());
        }

        /// <summary>
        /// Makes the fighter use the given active ability on the given cells.
        /// </summary>
        /// <param name="activeAbility">The active ability to use.</param>
        /// <param name="targetedCells">The target cells for the ability.</param>
        /// <exception cref="ArgumentException">Raised if targeted cells are empty.</exception>
        /// <exception cref="InvalidOperationException">Raised if not enough action points.</exception>
        public void UseActiveAbility(ActiveAbilitySO activeAbility, FightCell[] targetedCells)
        {
            // Do various checks to confirm the ability can be used
            if (targetedCells.Length == 0)
            {
                throw new ArgumentException("An active ability can't be used without one or more target cells");
            }

            if (_stats.actionPoints < activeAbility.ActionPointsCost)
            {
                throw new InvalidOperationException(
                    "Fighter : " + name + " does not have enough action points to use its ability "
                    + activeAbility.Name
                );
            }

            if (_godFavorsPoints < activeAbility.GodFavorsPointsCost)
            {
                throw new InvalidOperationException(
                    "Fighter : " + name + " does not have enough god favors points to use its ability "
                    + activeAbility.Name
                );
            }

            // Trigger the ability
            _stats.actionPoints -= activeAbility.ActionPointsCost;
            onActiveAbilityStarted?.Invoke(this, activeAbility);
            activeAbility.onActiveAbilityEnded += OnActiveAbilityEnded;
            activeAbility.Trigger(targetedCells, this);
        }

        /// <summary>
        /// Method to withstand a physical attack.
        /// </summary>
        /// <param name="physicalDamageAmount">The damage taken before withstanding.</param>
        /// <param name="isMasterstroke">True if the attack is a masterstroke.</param>
        public void PhysicalWithstand(int physicalDamageAmount, bool isMasterstroke)
        {
            // Play the receive damage animation
            PlayAnimationIfAny(_receiveDamageAnimationName);

            // Compute the inflicted physical damage amount
            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            inflictedPhysicalDamageAmount -= GetArmorPhysicalResistance();

            // Decrease the health of the fighter
            DecreaseHealth(inflictedPhysicalDamageAmount);

            // Trigger damage received event
            onDamageReceived?.Invoke(this, inflictedPhysicalDamageAmount, isMasterstroke);
            Debug.Log($"{name} + received {inflictedPhysicalDamageAmount} physical damages");
        }

        /// <summary>
        /// Method to withstand a magical attack.
        /// </summary>
        /// <param name="magicalDamageAmount">The damage amount to resist to.</param>
        /// <param name="magicalElement">The magical element to resist.</param>
        /// <param name="isMasterstroke">True if the attack is a masterstroke.</param>
        public void MagicalWithstand(int magicalDamageAmount, EMagicalElement magicalElement, bool isMasterstroke)
        {
            // Check if the magical resistance element is set
            if (!_stats.magicalResistances.ContainsKey(magicalElement))
            {
                throw new NullReferenceException($"Magical resistance element {magicalElement} is not set for fighter {name}");
            }

            // Play the receive damage animation
            PlayAnimationIfAny(_receiveDamageAnimationName);

            // Compute the inflicted magical damage amount
            int inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.magicalResistances[magicalElement]);
            inflictedMagicalDamageAmount -= GetArmorMagicalResistances()[magicalElement];

            // Decrease the health of the fighter
            DecreaseHealth(inflictedMagicalDamageAmount);

            // Trigger damage received event
            onDamageReceived?.Invoke(this, inflictedMagicalDamageAmount, isMasterstroke);
            Debug.Log($"{name} + received {inflictedMagicalDamageAmount} {magicalElement} magical damages");
        }

        /// <summary>
        /// Heals the fighter by the given amount.
        /// </summary>
        /// <param name="healAmount">The amount of health to restore.</param>
        /// <param name="isMasterstroke">True if the heal is a masterstroke.</param>
        public void Heal(int healAmount, bool isMasterstroke)
        {
            PlayAnimationIfAny(_healSelfAnimationName);
            IncreaseHealth(healAmount);
            onHealReceived?.Invoke(this, healAmount, isMasterstroke);
            Debug.Log($"{name} + has been healed by {healAmount}.");
        }

        /// <summary>
        /// Receives raw damages without any resistance calculation.
        /// </summary>
        /// <param name="damages">The damages to receive.</param>
        public void ReceiveRawDamages(int damages, bool isMasterstroke)
        {
            PlayAnimationIfAny(_receiveDamageAnimationName);
            DecreaseHealth(damages);
            onDamageReceived?.Invoke(this, damages, isMasterstroke);
            Debug.Log($"{name} + received {damages} raw damages.");
        }

        /// <summary>
        /// Updates a mutable stat by the given amount. For magical resistances and magical strengths use ReduceMagicalStat method instead.
        /// </summary>
        /// <param name="mutableStat">The mutable stat to increase or reduce.</param>
        /// <param name="amount">The amount to increase or reduce.</param>
        /// <param name="triggerAnimation">True to trigger the stat update animation, false otherwise.</param>
        public void UpdateMutableStat(EFighterMutableStat mutableStat, float amount, bool triggerAnimation = true, bool triggerEvent = true)
        {
            switch (mutableStat)
            {
                case EFighterMutableStat.MaxHealth:
                    Math.Min(0, _stats.maxHealth += (int)amount);
                    break;
                case EFighterMutableStat.MaxActionPoints:
                    Math.Min(0, _stats.maxActionPoints += (int)amount);
                    break;
                case EFighterMutableStat.MaxMovePoints:
                    Math.Min(0, _stats.maxMovePoints += (int)amount);
                    break;
                case EFighterMutableStat.Strength:
                    Math.Min(0, _stats.strength += (int)amount);
                    break;
                case EFighterMutableStat.Dexterity:
                    Math.Min(0, _stats.dexterity += (int)amount);
                    break;
                case EFighterMutableStat.Tenacity:
                    Math.Min(0, _stats.tenacity += (int)amount);
                    break;
                case EFighterMutableStat.DodgeChance:
                    Math.Min(0, _stats.dodgeChance += amount);
                    break;
                case EFighterMutableStat.PhysicalResistance:
                    Math.Min(0, _stats.physicalResistance += (int)amount);
                    break;
                case EFighterMutableStat.MasterstrokeChance:
                    Math.Min(0, _stats.masterstrokeChance += amount);
                    break;
                case EFighterMutableStat.Initiative:
                    Math.Min(0, _stats.initiative += (int)amount);
                    break;
                default:
                    Debug.LogWarning($"Unmanaged stat to reduce: {mutableStat}. If you want to modify a magical stat, use ReduceMagicalStat method instead.");
                    return;
            }

            if (triggerAnimation) PlayAnimationIfAny(amount > 0 ? _increaseStatAnimationName : _reduceStatAnimationName);
            if (triggerEvent) onNonMagicalStatMutated?.Invoke(this, mutableStat, amount);
        }

        /// <summary>
        /// Reduces or increases a magical stat by the given amount. To update other stats use UpdateMutableStat method instead.
        /// </summary>
        /// <param name="magicalElement">The related magical element.</param>
        /// <param name="amount">The amount to increase or reduce.</param>
        /// <param name="isResistance">True to update magical resistance stat, false to update magical strength</param>
        /// <param name="triggerAnimation">True to trigger the stat update animation, false otherwise.</param>
        public void UpdateMagicalStat(
            EMagicalElement magicalElement,
            int amount,
            bool isResistance,
            bool triggerAnimation = true,
            bool triggerEvent = true
        )
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

                if (triggerAnimation) PlayAnimationIfAny(amount > 0 ? _increaseStatAnimationName : _reduceStatAnimationName);
                if (triggerEvent) onMagicalStatMutated?.Invoke(this, magicalElement, amount, isResistance);
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
        /// <param name="statusToApply">The new status to apply.</param>
        public void ApplyStatus(AStatus statusToApply)
        {
            StatusesManager.ApplyStatus(statusToApply);
        }

        /// <summary>
        /// Remove a given status to the fighter.
        /// </summary>
        /// <param name="statusToRemove">The status to remove.</param>
        public void RemoveStatus(AStatus statusToRemove)
        {
            StatusesManager.RemoveStatus(statusToRemove);
        }

        /// <summary>
        /// Make the fighter try to dodge something.
        /// </summary>
        /// <returns>True if the dodge is successfull, false otherwise</returns>
        public bool TryDodge()
        {
            return Randomizer.GetBooleanOnChance(_stats.dodgeChance);
        }

        /// <summary>
        /// Make the fighter try to masterstroke.
        /// </summary>        
        /// <returns>True if the masterstroke is successfull, false otherwise</returns>
        public bool TryMasterstroke()
        {
            return Randomizer.GetBooleanOnChance(_stats.masterstrokeChance);
        }

        #endregion

        #region Actions process management

        public void OnDirectAttackEnded()
        {
            DirectAttackManager.onDirectAttackEnded -= OnDirectAttackEnded;
            onDirectAttackEnded?.Invoke(this);
        }

        private void OnActiveAbilityEnded(ActiveAbilitySO activeAbility)
        {
            activeAbility.onActiveAbilityEnded -= OnActiveAbilityEnded;
            onActiveAbilityEnded?.Invoke(this, activeAbility);
        }

        #endregion

        #region Stats getters & manipulation

        public int GetMovePoints() => _stats.movePoints;

        public int GetMaxMovePoints() => _stats.maxMovePoints;

        public int GetActionPoints() => _stats.actionPoints;

        public int GetMaxActionPoints() => _stats.maxActionPoints;

        public override int GetHealth() => _stats.health;

        public int GetMaxHealth() => _stats.maxHealth;

        public int GetStrength() => _stats.strength;

        public int GetDexterity() => _stats.dexterity;

        public int GetTenacity() => _stats.tenacity;

        public int GetPhysicalResistance() => _stats.physicalResistance;

        public float GetMasterstrokeChance() => _stats.masterstrokeChance;

        public float GetDodgeChance() => _stats.dodgeChance;

        public int GetInitiative() => _stats.initiative;

        public int GetGodFavorsPoints() => _godFavorsPoints;

        public Dictionary<EMagicalElement, int> GetMagicalStrengths() => _stats.magicalStrengths;

        public Dictionary<EMagicalElement, int> GetMagicalResistances() => _stats.magicalResistances;

        public void TryIncreaseGodFavorsPointsForAction(EGodFavorsAction action)
        {
            Dictionary<EGodFavorsAction, int> amountPerAction = SElementToValue<EGodFavorsAction, int>.GetDictionaryFromArray(
                FighterClass.God.FavorGivingActions
            );
            if (amountPerAction.Keys.Contains(action))
            {
                _godFavorsPoints += amountPerAction[action];
            }
        }

        public float GetMutableStat(EFighterMutableStat mutableStat)
        {
            return mutableStat switch
            {
                EFighterMutableStat.MaxHealth => _stats.maxHealth,
                EFighterMutableStat.MaxActionPoints => _stats.maxActionPoints,
                EFighterMutableStat.MaxMovePoints => _stats.maxMovePoints,
                EFighterMutableStat.Strength => _stats.strength,
                EFighterMutableStat.Dexterity => _stats.dexterity,
                EFighterMutableStat.Tenacity => _stats.tenacity,
                EFighterMutableStat.PhysicalResistance => _stats.physicalResistance,
                EFighterMutableStat.DodgeChance => _stats.dodgeChance,
                EFighterMutableStat.MasterstrokeChance => _stats.masterstrokeChance,
                EFighterMutableStat.Initiative => _stats.initiative,
                _ => throw new ArgumentOutOfRangeException(nameof(mutableStat), mutableStat, null),
            };
        }

        /// <summary>
        /// Returns a dictionary of all statuses that are currently active on the fighter.
        /// </summary>
        /// <returns></returns>
        public Dictionary<AStatus, (bool isActive, int duration)> GetStatuses() => StatusesManager.GetStatuses();

        public FighterCollider GetWeaponCollider()
        {
            return GetComponentInChildren<FighterCollider>();
        }

        public void ResetMovementAndActionPoints()
        {
            _stats.actionPoints = _stats.maxActionPoints;
            _stats.movePoints = _stats.maxMovePoints;
        }

        private void ResetStatsToDefaultConfiguration()
        {
            _stats.maxHealth = _initialStats.maxHealth + FighterClass.ClassMaxHealth;
            _stats.health = _initialStats.health;
            _stats.maxActionPoints = _initialStats.maxActionPoints + FighterClass.ClassMaxActionPoints;
            _stats.actionPoints = _initialStats.maxActionPoints;
            _stats.maxMovePoints = _initialStats.maxMovePoints + FighterClass.ClassMaxMovePoints;
            _stats.movePoints = _initialStats.maxMovePoints;
            _stats.strength = _initialStats.strength + FighterClass.ClassMaxMovePoints;
            _stats.dexterity = _initialStats.dexterity + FighterClass.ClassDexterity;
            _stats.tenacity = _initialStats.tenacity + FighterClass.ClassTenacity;
            _stats.physicalResistance = _initialStats.physicalResistance + FighterClass.ClassPhysicalResistance;

            _stats.magicalResistances = _initialStats.magicalResistances;
            _stats.AddMagicalResistances(SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(FighterClass.ClassMagicalResistances));

            _stats.magicalStrengths = _initialStats.magicalStrengths;
            _stats.AddMagicalStrengths(SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(FighterClass.ClassMagicalStrengths));

            _stats.dodgeChance = _initialStats.dodgeChance + FighterClass.ClassDodgeChance;
            _stats.masterstrokeChance = _initialStats.masterstrokeChance + FighterClass.ClassMasterstrokeChance;
            _stats.initiative = _initialStats.initiative + FighterClass.ClassInitiative;
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
            return _stats.movePoints > 0 && CellsNeighbors.GetNeighbors(fightGrid, cell).Length > 0;
        }

        /// <summary>
        /// Returns whether the fighter can use its direct attack in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <param name="fightersTeams">The teams of the fighters in the fight.</param>
        /// <param name="target">The optional target to check if the direct attack can be used on.</param>
        /// <returns>True if he has enough actions points and if the direct attack targeter can be resolved around him.</returns>
        public bool CanDirectAttack(HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams, Fighter target = null)
        {
            return Weapon.UseActionPointsCost <= _stats.actionPoints && (
                (
                    Weapon.AttackTargeter.AtLeastOneCellResolvable(fightGrid, cell, fightersTeams) &&
                    target == null
                ) ||
                CanUseTargeterOnFighter(Weapon.AttackTargeter, target, fightGrid, fightersTeams)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use one if its active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <param name="fightersTeams">The teams of the fighters in the fight.</param>
        /// <param name="target">The optional target to check if the active ability can be used on.</param>
        /// <param name="mandatoryEffectTypes">The optionnal effect to apply.</param>
        /// <returns>True if he has enough actions points and if an active ability targeter can be resolved around him.</returns>
        public bool CanUseAtLeastOneActiveAbility(
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams,
            Fighter target = null,
            ListOfTypes<AEffect> mandatoryEffectTypes = null
        )
        {
            return ActiveAbilities.Any
                (
                    activeAbility =>
                    {
                        if (!CanUseActiveAbility(fightGrid, activeAbility, fightersTeams, target)) return false;
                        if (mandatoryEffectTypes != null && mandatoryEffectTypes.Any())
                        {

                            AEffect[] abilityEffects = activeAbility.Effects;
                            return mandatoryEffectTypes.Any(effect => abilityEffects.Any(e => e.GetType() == effect.GetType()));
                        }
                        return true;
                    }
                );
        }

        /// <summary>
        /// Returns the potential first touching cell sequence of the targeter on the target.
        /// Returns null if no sequence is found.
        /// </summary>
        /// <param name="targeter">The targeter to get the sequence from.</param>
        /// <param name="target">The target to try to touch.</param>
        /// <param name="fightGrid">The current fight grid.</param>
        /// <param name="fightersTeams">The current fighters and corresponding team.</param>
        /// <returns>The first touching cell sequence if it exists, null otherwise.</returns>
        public FightCell[] GetFirstTouchingCellSequence(
            Targeter targeter,
            Fighter target,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams
        )
        {
            return targeter.GetAllResolvedCellsSequences(fightGrid, cell, fightersTeams).FirstOrDefault(
                cellsSequence => cellsSequence.Contains(target.cell)
            );
        }

        /// <summary>
        /// Returns whether the fighter can use the given active ability in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <param name="activeAbility">The active ability to check if it can be used.</param>
        /// <param name="fightersTeams">The teams of the fighters in the fight.</param>
        /// <param name="target">The optional target to check if the active ability can be used on.</param>
        /// <returns>True if he has enough actions points and if the active ability targeter can be resolved around him.</returns>
        public bool CanUseActiveAbility(
            HexGrid fightGrid,
            ActiveAbilitySO activeAbility,
            Dictionary<Fighter, bool> fightersTeams,
            Fighter target = null
        )
        {
            return (
                activeAbility.ActionPointsCost <= _stats.actionPoints &&
                activeAbility.GodFavorsPointsCost <= _godFavorsPoints &&
                (
                    (
                        activeAbility.Targeter.AtLeastOneCellResolvable(
                            fightGrid,
                            cell,
                            fightersTeams,
                            activeAbility.CellAlterations
                        ) &&
                        target == null
                    ) ||
                    CanUseTargeterOnFighter(activeAbility.Targeter, target, fightGrid, fightersTeams, activeAbility.CellAlterations)
                )
            );
        }

        /// <summary>
        /// Returns whether the fighter can act in the given context or not.
        /// </summary>
        /// <param name="fightGrid">The fight grid where the fighter is currently fighting.</param>
        /// <returns>True if he can move, direct attack or use one of its active ability.</returns>
        public bool CanAct(HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            return (
                CanMove(fightGrid) ||
                CanDirectAttack(fightGrid, fightersTeams) ||
                CanUseAtLeastOneActiveAbility(fightGrid, fightersTeams)
            );
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

        private void DecreaseHealth(int amount)
        {
            _stats.health = Math.Clamp(_stats.health - amount, 0, _stats.maxHealth);
            if (FighterConfiguration is PersistedFighterConfigurationSO persistedFighterConfiguration)
            {
                persistedFighterConfiguration.SetHealth(_stats.health);
            }
            if (_stats.health == 0)
            {
                onFighterDied?.Invoke(this);
            }
        }

        private void IncreaseHealth(int amount)
        {
            _stats.health = Math.Clamp(_stats.health + amount, 0, _stats.maxHealth);
            if (FighterConfiguration is PersistedFighterConfigurationSO persistedFighterConfiguration)
            {
                persistedFighterConfiguration.SetHealth(_stats.health);
            }
            if (_stats.health == 0)
            {
                onFighterDied?.Invoke(this);
            }
        }

        private bool CanUseTargeterOnFighter(
            Targeter targeter,
            Fighter target,
            HexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams,
            AFightCellAlteration[] cellAlterations = null
        )
        {
            return targeter.GetAllResolvedCellsSequences(fightGrid, cell, fightersTeams, cellAlterations).Any(
                cellsSequence => cellsSequence.Contains(target.cell)
            );
        }

        private int GetArmorPhysicalResistance()
        {
            return _inventory.GetArmorPieces().Sum(armorPiece => ((ArmorSO)armorPiece).PhysicalResistance);
        }

        private Dictionary<EMagicalElement, int> GetArmorMagicalResistances()
        {
            Dictionary<EMagicalElement, int> armorMagicalResistances = new();
            foreach (EMagicalElement magicalElement in Enum.GetValues(typeof(EMagicalElement)))
            {
                armorMagicalResistances.Add(magicalElement, 0);
            }

            foreach (ArmorSO armorPiece in _inventory.GetArmorPieces())
            {
                Dictionary<EMagicalElement, int> armorPieceMagicalResistances = SElementToValue<EMagicalElement, int>.GetDictionaryFromArray(
                    armorPiece.MagicalResistances
                );
                foreach (KeyValuePair<EMagicalElement, int> magicalResistance in armorPieceMagicalResistances)
                {
                    armorMagicalResistances[magicalResistance.Key] += magicalResistance.Value;
                }
            }
            return armorMagicalResistances;
        }
        #endregion

        #region Movement handling
        private void MakeNextMove()
        {
            // Trigger exit trap of current cell before moving (if any)
            cell.onTrapTriggered += OnExitCellTrapTriggered;
            cell.TriggerTrapIfAny(ETrapTriggerTime.OnExit);
        }

        private void OnExitCellTrapTriggered()
        {
            cell.onTrapTriggered -= OnExitCellTrapTriggered;

            FightCell cellToMoveTo = (FightCell)_currentMovePath.GetNextCellInPath();
            MovementController.onMoveEnded += OnFighterArrivedAtCell;
            MovementController.Move(cell, cellToMoveTo, _currentMovePath.IsLastMove);
        }

        private void OnFighterArrivedAtCell(Cell destinationCell)
        {
            MovementController.onMoveEnded -= OnFighterArrivedAtCell;

            // Update the leaved cell
            cell.SetFighter(null);

            // Update the cell the fighter is on
            FightCell destinationFightCell = (FightCell)destinationCell;
            destinationFightCell.SetFighter(this);
            cell = destinationFightCell;

            // Decrease move points
            _stats.movePoints -= 1;

            // Trigger trap of new entered cell if any
            cell.onTrapTriggered += OnEnteredCellTrapTriggered;
            cell.TriggerTrapIfAny(ETrapTriggerTime.OnEnter);
        }

        private void OnEnteredCellTrapTriggered()
        {
            // Unsubscribe to the trap event
            cell.onTrapTriggered -= OnEnteredCellTrapTriggered;

            // If there are still moves to make, make the next one
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
            if (MovementController != null)
            {
                return true;
            }
            MovementController = GetComponent<EntityVisualMovementController>();
            if (MovementController == null)
            {
                return false;
            }
            return true;
        }

        private bool TrySetupEntitiyVisualAnimationController()
        {
            if (AnimationController != null)
            {
                return true;
            }
            AnimationController = GetComponent<EntityVisualAnimationController>();
            if (AnimationController == null)
            {
                return false;
            }
            return true;
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