using System;
using System.Collections.Generic;
using FrostfallSaga.Fight.Fighters;
using UnityEngine;

namespace FrostfallSaga.Fight.FightConditions
{
    /// <summary>
    ///     Check if the fighter's move points is [StatConditionType] compared to the given Threshold.
    /// </summary>
    [Serializable]
    public class MovePointsThresholdCondition : AFighterCondition
    {
        [SerializeField] public EStatConditionType StatConditionType = EStatConditionType.LESS_OR_EQUAL;
        [SerializeField] public int Threshold;
        [SerializeField] public bool UsePercentage;

        public override bool CheckCondition(Fighter fighter, FightHexGrid fightGrid,
            Dictionary<Fighter, bool> fightersTeams)
        {
            int finalThreshold = Threshold;
            if (UsePercentage) finalThreshold = (int)(fighter.GetMaxMovePoints() * Threshold / 100f);
            return StatConditionType.CompareIntegers(fighter.GetMovePoints(), finalThreshold);
        }

        public override string GetName()
        {
            return "Move points threshold";
        }

        public override string GetDescription()
        {
            return
                $"Check if the fighter's move points are {StatConditionType.GetAsString()} {Threshold}{(UsePercentage ? "%" : "")}.";
        }
    }
}