using UnityEngine;
using System.Collections.Generic;
using System;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.StatusEffects
{
    public class StatusEffectManager : MonoBehaviour
    {

        private Dictionary<StatusEffect, (bool isActive, int duration)> statusEffects = new Dictionary<StatusEffect, (bool, int)>();

        public void ApplyEffect(StatusEffect status)
        {
            statusEffects[status] = (true, status.Duration);
        }

        public void UpdateStatusEffects()
        {
            List<StatusEffect> statusesToRemove = new List<StatusEffect>();
            Dictionary<StatusEffect, (bool isActive, int duration)> tempStatusEffects = new Dictionary<StatusEffect, (bool, int)>();

            foreach (var status in statusEffects)
            {
                tempStatusEffects[status.Key] = (status.Value.isActive, status.Value.duration);
            }
            foreach (var status in statusEffects.Keys)
            {
                var (isActive, duration) = tempStatusEffects[status];
                duration--;
                if (isActive)
                {
                    status.ApplyEffect(GetComponent<Fighter>());
                    if (!status.IsRecurring) isActive = false;
                }
                tempStatusEffects[status] = (isActive, duration);
                if (duration <= 0)
                {
                    statusesToRemove.Add(status);
                }

            }

            statusEffects = tempStatusEffects;
            foreach (var status in statusesToRemove)
            {
                RemoveEffect(status);
                status.RemoveEffect(GetComponent<Fighter>());

            }
        }

        public void RemoveEffect(StatusEffect status)
        {
            if (statusEffects.ContainsKey(status))
            {
                statusEffects.Remove(status);
            }
        }

        public Dictionary<StatusEffect, (bool isActive, int duration)> getStatusEffects()
        {
            return statusEffects;
        }
    }
}