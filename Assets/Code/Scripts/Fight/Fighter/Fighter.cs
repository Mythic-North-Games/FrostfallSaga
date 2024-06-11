using UnityEngine;

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
            _stats.magicalResistance = FighterConfiguration.MagicalResistance;
            _stats.physicalResistance = FighterConfiguration.PhysicalResistance;
            _stats.initiative = FighterConfiguration.Initiative;
        }

         public void PhysicalWhistand(int physicalDamageAmount){
            _stats.physicalResistance -= physicalDamageAmount;
            Debug.Log("Nouvelle resistance physique : "+_stats.physicalResistance);
        }

        public void MagicalWhistand(int magicalDamageAmount){
            _stats.magicalResistance -= magicalDamageAmount;
            Debug.Log("Nouvelle resistance Magique : "+_stats.magicalResistance);

        }

        public void Heal(int healAmount){
            _stats.health += healAmount;
            Debug.Log("Nouveaux points de vie : "+_stats.health);

        }

        private void PlayDamageAnimationIfAny(){
        //TODO checks if such an animation is available in the given animator before playing it
        }

        private void PlayHealAnimationIfAny(){
        //TODO checks if such an animation is available in the given animator before playing it
        }
    }
}