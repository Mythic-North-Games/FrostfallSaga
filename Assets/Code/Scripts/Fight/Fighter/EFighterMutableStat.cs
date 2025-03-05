namespace FrostfallSaga.Fight.Fighters
{
    public enum EFighterMutableStat
    {
        MaxHealth,
        MaxActionPoints,
        MaxMovePoints,
        Strength,
        Dexterity,
        Tenacity,
        PhysicalResistance,
        MagicalResistances,
        MagicalStrengths,
        DodgeChance,
        MasterstrokeChance,
        Initiative
    }

    public static class EFighterMutableStatExtensions
    {
        public static string ToUIString(this EFighterMutableStat stat)
        {
            return stat switch
            {
                EFighterMutableStat.MaxHealth => "<b>Max Health</b>",
                EFighterMutableStat.MaxActionPoints => "<b>Max Action Points</b>",
                EFighterMutableStat.MaxMovePoints => "<b>Max Move Points</b>",
                EFighterMutableStat.Strength => "<b>Strength</b>",
                EFighterMutableStat.Dexterity => "<b>Dexterity</b>",
                EFighterMutableStat.Tenacity => "<b>Tenacity</b>",
                EFighterMutableStat.PhysicalResistance => "<b>Physical Resistance</b>",
                EFighterMutableStat.MagicalResistances => "<b>Magical Resistances</b>",
                EFighterMutableStat.MagicalStrengths => "<b>Magical Strengths</b>",
                EFighterMutableStat.DodgeChance => "<b>Dodge Chance</b>",
                EFighterMutableStat.MasterstrokeChance => "<b>Masterstroke Chance</b>",
                EFighterMutableStat.Initiative => "<b>Initiative</b>",
                _ => "Unknown stat"
            };
        }

        public static string GetIconResourceName(this EFighterMutableStat stat)
        {
            return stat switch
            {
                EFighterMutableStat.MaxHealth => "maxHealth",
                EFighterMutableStat.MaxActionPoints => "maxActionPoints",
                EFighterMutableStat.MaxMovePoints => "maxMovePoints",
                EFighterMutableStat.Strength => "strength",
                EFighterMutableStat.Dexterity => "dexterity",
                EFighterMutableStat.Tenacity => "tenacity",
                EFighterMutableStat.PhysicalResistance => "physicalResistance",
                EFighterMutableStat.MagicalResistances => "magic",
                EFighterMutableStat.MagicalStrengths => "magic",
                EFighterMutableStat.DodgeChance => "dodgeChance",
                EFighterMutableStat.MasterstrokeChance => "masterstrokeChance",
                EFighterMutableStat.Initiative => "initiative",
                _ => string.Empty
            };
        }
    }
}