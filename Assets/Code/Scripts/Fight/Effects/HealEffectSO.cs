using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Utilities;

namespace FrostfallSaga.Fight.Effects
{
    /// <summary>
    /// Effect that applies heal to the target fighter.
    /// </summary>
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/Fight/Effects/HealEffect", order = 0)]
    public class HealEffectSO : AEffectSO
    {
    [field: SerializeField, Range(0, 9999)] public int HealAmount { get; private set; }

    public override void ApplyEffect(Fighter attacker, Fighter defender)  
        {
            // Calculate critical heal
            int finalHealAmount = DamageUtils.TryCriticalHit(attacker, HealAmount);

            // Apply the heal
            defender.Heal(finalHealAmount);
            Debug.Log($"Healed {defender.name} for {finalHealAmount} health.");
        }
         
    }
    
}
