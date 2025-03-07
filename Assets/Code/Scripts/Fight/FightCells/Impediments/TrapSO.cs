using System;
using FrostfallSaga.Fight.Effects;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.FightCells.Impediments
{
    /// <summary>
    ///     Represents a trap that can be placed on a fight cell.
    ///     Traps apply a list of effects when a fighter steps on them at the specified trigger times.
    ///     Traps effects can't be dodged or masterstroked.
    /// </summary>
    [CreateAssetMenu(fileName = "Trap", menuName = "ScriptableObjects/Fight/Impediments/Trap", order = 0)]
    public class TrapSO : AImpedimentSO
    {
        [field: SerializeField]
        [field: Header("Trap definition")]
        public ETrapTriggerTime[] TriggerTimes { get; private set; }

        [SerializeReference] public AEffect[] Effects = { };

        public Action onTrapTriggered;

        public void Trigger(Fighter receiver)
        {
            foreach (AEffect effect in Effects)
                effect.ApplyEffect(
                    receiver,
                    false
                );

            onTrapTriggered?.Invoke();
        }
    }
}