namespace FrostfallSaga.Fight.Statuses
{
    public enum EStatusTriggerTime
    {
        StartOfTurn,
        EndOfTurn
    }

    public static class EStatusTriggerTimeExtensions
    {
        public static string ToUIString(this EStatusTriggerTime triggerTime)
        {
            switch (triggerTime)
            {
                case EStatusTriggerTime.StartOfTurn:
                    return "start of the turn";
                case EStatusTriggerTime.EndOfTurn:
                    return "end of the turn";
                default:
                    return "Unknown";
            }
        }
    }
}