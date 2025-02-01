using System;

namespace FrostfallSaga.Core.GameState.Fight
{
    /// <summary>
    /// From where the fight is launched.
    /// </summary>
    public enum EFightOrigin
    {
        DUNGEON,
        KINGDOM
    }

    public static class EFightOriginExtensions
    {
        public static EScenesName ToEScenesName(this EFightOrigin fightOrigin)
        {
            switch (fightOrigin)
            {
                case EFightOrigin.DUNGEON:
                    return EScenesName.DUNGEON;
                case EFightOrigin.KINGDOM:
                    return EScenesName.KINGDOM;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fightOrigin), fightOrigin, null);
            }
        }
    }
}