using System;

namespace FrostfallSaga.Core.GameState.Fight
{
    /// <summary>
    ///     From where the fight is launched.
    /// </summary>
    public enum EFightOrigin
    {
        FIGHT, // Fight launched from the fight scene (dev only)
        DUNGEON,
        KINGDOM
    }

    public static class EFightOriginExtensions
    {
        public static EScenesName ToEScenesName(this EFightOrigin fightOrigin)
        {
            switch (fightOrigin)
            {
                case EFightOrigin.FIGHT:
                    return
                        EScenesName
                            .KINGDOM; // If the fight is launched from the fight scene, we go back to the kingdom (dev only)
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