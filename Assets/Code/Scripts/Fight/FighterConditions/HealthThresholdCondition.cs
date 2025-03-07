using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using FrostfallSaga.Grid;
using UnityEngine;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    ///     Check if the fighter's health is [StatConditionType] compared to the given Threshold.
    /// </summary>
    [Serializable]
    public class HealthThresholdCondition : AFighterCondition
    {
        [SerializeField] public EStatConditionType StatConditionType = EStatConditionType.LESS_OR_EQUAL;
        [SerializeField] public int Threshold;
        [SerializeField] public bool UsePercentage;

        public override bool CheckCondition(Fighter fighter, AHexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams)
        {
            int finalThreshold = Threshold;
            if (UsePercentage) finalThreshold = (int)(fighter.GetMaxHealth() * Threshold / 100f);
            return fighter.GetHealth() > 0 && StatConditionType.CompareIntegers(fighter.GetHealth(), finalThreshold);
        }

        public override string GetName()
        {
            return "Health threshold";
        }

        public override string GetDescription()
        {
            return
                $"Check if the fighter's health is {StatConditionType.GetAsString()} {Threshold}{(UsePercentage ? "%" : "")}.";
        }
    }
}