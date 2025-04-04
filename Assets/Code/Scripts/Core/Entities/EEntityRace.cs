namespace FrostfallSaga.Core.Entities
{
    public enum EEntityRace
    {
        HUMAN,
        SKELETON
    }

    public static class EEntityRaceMethods
    {
        public static string ToUIString(this EEntityRace entityRace)
        {
            return entityRace switch
            {
                EEntityRace.HUMAN => "<b>Human</b>",
                EEntityRace.SKELETON => "<b>Skeleton</b>",
                _ => "Unknown",
            };
        }
    }
}