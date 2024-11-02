using UnityEngine;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Fight.Effects;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    /// <summary>
    /// Represents a trap that can be placed on a fight cell.
    /// Traps apply a list of effects when a fighter steps on them at the specified trigger times.
    /// Traps effects can't be dodged or masterstroked.
    /// </summary>
    [CreateAssetMenu(fileName = "Trap", menuName = "ScriptableObjects/Fight/Impediments/Trap", order = 0)]
    public class TrapSO : AImpedimentSO
    {
        [field: SerializeField, Header("Trap definition")] public AEffectSO[] Effects { get; private set; }
        [field: SerializeField] public ETrapTriggerTime[] TriggerTimes { get; private set; }

        public void Trigger(Fighter receiver)
        {
            foreach (var effect in Effects)
            {
                effect.ApplyEffect(receiver, initator: null, canMasterstroke: false, canDodge: false);
            }
        }
    }
}