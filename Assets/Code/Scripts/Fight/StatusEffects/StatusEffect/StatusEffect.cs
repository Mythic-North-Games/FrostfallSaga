using UnityEngine;
using System.Collections.Generic;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{
public abstract class StatusEffect {
    public StatusType StatusType { get; protected set; }
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public int Duration { get; protected set; }  
    public bool IsRecurring { get; protected set; } 
    public string animationStateName { get; protected set; }

    public abstract void ApplyEffect(Fighter fighter);
    public abstract void RemoveEffect(Fighter fighter);


}

}