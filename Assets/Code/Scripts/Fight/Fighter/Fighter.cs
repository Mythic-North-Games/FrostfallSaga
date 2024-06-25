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
        [field: SerializeField] public Cell Cell { get; private set; }
        public Action<Fighter> OnFighterMoved;

        [SerializeField] private EntityVisualMovementController _movementController;
        private MovePath _currentMovePath;
        private FighterStats _stats;

        private void Awake()
        {
            if (FighterConfiguration == null)
            {
                Debug.LogError("No fighter configuration for " + name);
            }
            if (!TrySetupEntitiyVisyualMoveController())
            {
                Debug.LogError("No entity visual move controller found for fighter " + name);
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

        public void PhysicalWhistand(int physicalDamageAmount)
        {

            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            PlayDamageAnimationIfAny();
            _stats.health = Math.Max(0, _stats.health - inflictedPhysicalDamageAmount);
        }

        public void MagicalWhistand(int magicalDamageAmount, EMagicalElement magicalElement)
        {
            int inflictedMagicalDamageAmount = 0;

            if (_stats.magicalResistances.TryGetValue(magicalElement, out int value))
            {
                inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - value);
            }
            else
            {
                Debug.LogError("Magical element is not bind to any value!");
            }

            PlayDamageAnimationIfAny();
            _stats.health = Math.Max(0, _stats.health - inflictedMagicalDamageAmount);
        }

        public void Heal(int healAmount)
        {
            PlayHealAnimationIfAny();
            _stats.health = Math.Min(_stats.health + healAmount, _stats.maxHealth);

        }

        private void PlayDamageAnimationIfAny()
        {
            
        }

        private void PlayHealAnimationIfAny()
        {
            
        }

        #region Movement handling
        private void MakeNextMove()
        {
            Cell cellToMoveTo = _currentMovePath.GetNextCellInPath();
            _movementController.Move(Cell, cellToMoveTo, _currentMovePath.IsLastMove);
        }

        private void OnFighterArrivedAtCell(Cell destinationCell)
        {
            _stats.movePoints -= 1;
            Cell = destinationCell;
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
        private bool TrySetupEntitiyVisyualMoveController()
        {
            _movementController = GetComponentInChildren<EntityVisualMovementController>();
            if (_movementController == null)
            {
                return false;
            }
            
            _movementController.onMoveEnded += OnFighterArrivedAtCell;
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
    }
}