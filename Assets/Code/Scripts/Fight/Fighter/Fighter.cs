using UnityEngine;
using FrostfallSaga.Fight.Effects;
using System;
using System.Collections.Generic;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : MonoBehaviour
    {
        [field: SerializeField] public FighterConfigurationSO FighterConfiguration { get; private set; }
        private FighterStats _stats;
        private void Awake()
        {
            if (FighterConfiguration == null)
            {
                Debug.LogError("No fighter configuration for " + name);
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

        public void PhysicalWhistand(int physicalDamageAmount)
        {

            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            _stats.health = Math.Max(0, _stats.health - inflictedPhysicalDamageAmount);
            Debug.Log("New PV : " + _stats.health);
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

            _stats.health = Math.Max(0, _stats.health - inflictedMagicalDamageAmount);
            Debug.Log("New PV : " + _stats.health);

        }

        public void Heal(int healAmount)
        {
            _stats.health = Math.Min(_stats.health + healAmount, _stats.maxHealth);
            Debug.Log("New PV : " + _stats.health);

        }

        private void PlayDamageAnimationIfAny()
        {
            //TODO checks if such an animation is available in the given animator before playing it
        }

        private void PlayHealAnimationIfAny()
        {
            //TODO checks if such an animation is available in the given animator before playing it
        }
    }
}