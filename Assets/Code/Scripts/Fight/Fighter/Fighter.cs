using UnityEngine;
using FrostfallSaga.Fight.Effects;
using System;

namespace FrostfallSaga.Fight.Fighters
{
    public class Fighter : MonoBehaviour
    {
        [field: SerializeField] public FighterConfigurationSO FighterConfiguration { get; private set; }
        private FighterStats _stats;
        private Dict<EMagicalElement, int> magicElements = new Dictionary<EMagicalElement, int>
        {
            { EMagicalElement.FIRE, _stats.fireResistance },
            { EMagicalElement.WATER, _stats.waterResistance },
            { EMagicalElement.ICE, _stats.iceResistance },
            { EMagicalElement.WIND, _stats.windResistance },
            { EMagicalElement.LIGHTNING, _stats.lightningResistance },
            { EMagicalElement.EARTH, _stats.earthResistance }
        };

        private void Awake()
        {
            if (FighterConfiguration == null)
            {
                Debug.LogError("No fighter configuration for " + name);
            }
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
            _stats.magicalStrength = FighterConfiguration.MagicalStrength;
            _stats.fireResistance = FighterConfiguration.FireResistance;
            _stats.waterResistance = FighterConfiguration.WaterResistance;
            _stats.iceResistance = FighterConfiguration.IceResistance;
            _stats.windResistance = FighterConfiguration.WindResistance;
            _stats.lightningResistance = FighterConfiguration.LightningResistance;
            _stats.earthResistance = FighterConfiguration.EarthResistance;
            _stats.physicalResistance = FighterConfiguration.PhysicalResistance;
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

            if (myDictionary.TryGetValue(magicalElement, out int value))
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