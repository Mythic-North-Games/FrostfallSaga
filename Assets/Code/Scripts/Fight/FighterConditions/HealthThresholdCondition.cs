using System;
using System.Collections.Generic;
using UnityEngine;
using FrostfallSaga.Grid;
using FrostfallSaga.Fight.Fighters;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    /// Check if the fighter's health is [StatConditionType] compared to the given Threshold.
    /// </summary>
    [Serializable]
    public class HealthThresholdCondition : AFighterCondition
    {
        [SerializeField] public EStatConditionType StatConditionType = EStatConditionType.LESS_OR_EQUAL;
        [SerializeField] public int Threshold;
        [SerializeField] public bool UsePercentage;

        public override bool CheckCondition(Fighter fighter, HexGrid fightGrid, Dictionary<Fighter, bool> fightersTeams)
        {
            int finalThreshold = Threshold;
            if (UsePercentage)
            {
                finalThreshold = (int) (fighter.GetMaxHealth() * Threshold / 100f);
            }
            return StatConditionType.CompareIntegers(fighter.GetHealth(), finalThreshold);
        }

        public override string GetName()
        {
            return "Health threshold";
        }

        public override string GetDescription()
        {
            return $"Check if the fighter's health is {StatConditionType.GetAsString()} {Threshold}{(UsePercentage ? "%" : "")}.";
        }
    }
}