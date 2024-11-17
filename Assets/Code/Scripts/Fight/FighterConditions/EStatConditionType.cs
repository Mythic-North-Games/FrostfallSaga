using System;

namespace FrostfallSaga.Fight.FightConditions
{
    public enum EStatConditionType
    {
        EQUAL,
        GREATER,
        GREATER_OR_EQUAL,
        LESS,
        LESS_OR_EQUAL,

    }

    public static class EStatConditionTypeMethods
    {
        public static bool CompareIntegers(this EStatConditionType conditionType, int value1, int value2)
        {
            return conditionType switch
            {
                EStatConditionType.EQUAL => value1 == value2,
                EStatConditionType.GREATER => value1 > value2,
                EStatConditionType.GREATER_OR_EQUAL => value1 >= value2,
                EStatConditionType.LESS => value1 < value2,
                EStatConditionType.LESS_OR_EQUAL => value1 <= value2,
                _ => throw new Exception("Unknown EStatConditionType"),
            };
        }

        public static string GetAsString(this EStatConditionType conditionType)
        {
            return conditionType switch
            {
                EStatConditionType.EQUAL => "equal to",
                EStatConditionType.GREATER => "greater than",
                EStatConditionType.GREATER_OR_EQUAL => "greater or equal to",
                EStatConditionType.LESS => "less than",
                EStatConditionType.LESS_OR_EQUAL => "less or equal to",
                _ => throw new Exception("Unknown EStatConditionType"),
            };
        }
    }
}
