using UnityEngine;
using FrostfallSaga.Fight.Effects;
using System;

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

         public void PhysicalWhistand(int physicalDamageAmount){

            int inflictedPhysicalDamageAmount = Math.Max(0, physicalDamageAmount - _stats.physicalResistance);
            _stats.health = Math.Max(0, _stats.health - inflictedPhysicalDamageAmount);
            Debug.Log("New PV : "+_stats.health);
        }

        public void MagicalWhistand(int magicalDamageAmount, EMagicalElement magicalElement){
            int inflictedMagicalDamageAmount = 0;

            switch ( magicalElement) {
                case EMagicalElement.FIRE :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.fireResistance);
                    break;
                case EMagicalElement.WATER :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.waterResistance);
                    break;
                case EMagicalElement.ICE :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.iceResistance);
                    break;
                case EMagicalElement.WIND :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.windResistance);
                    break;
                case EMagicalElement.LIGHTNING :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.lightningResistance);
                    break;
                case EMagicalElement.EARTH :
                    inflictedMagicalDamageAmount = Math.Max(0, magicalDamageAmount - _stats.earthResistance);
                    break;
            }
            _stats.health = Math.Max(0, _stats.health - inflictedMagicalDamageAmount);
            Debug.Log("New PV : "+_stats.health);

        }

        public void Heal(int healAmount){
            _stats.health = Math.Min(_stats.health + healAmount, _stats.maxHealth);
            Debug.Log("New PV : "+_stats.health);

        }

        private void PlayDamageAnimationIfAny(){
        //TODO checks if such an animation is available in the given animator before playing it
        }

        private void PlayHealAnimationIfAny(){
        //TODO checks if such an animation is available in the given animator before playing it
        }
    }
}