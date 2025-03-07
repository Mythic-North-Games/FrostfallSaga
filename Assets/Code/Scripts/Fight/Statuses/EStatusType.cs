using System;

namespace FrostfallSaga.Fight.Statuses
{
    public enum EStatusType
    {
        SLOWNESS,
        BLEED,
        PARALYSIS,
        WEAKNESS,
        STRENGTH
    }

    public static class EStatusTypeMethods
    {
        public static bool IsBuff(this EStatusType statusType)
        {
            return statusType switch
            {
                EStatusType.SLOWNESS => false,
                EStatusType.BLEED => false,
                EStatusType.PARALYSIS => false,
                EStatusType.WEAKNESS => false,
                EStatusType.STRENGTH => true,
                _ => throw new InvalidOperationException("Unknown status type.")
            };
        }
    }
}