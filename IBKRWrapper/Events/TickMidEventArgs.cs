namespace IBKRWrapper.Events
{
    public class TickMidEventArgs(int reqId, long time, double midPoint) : EventArgs
    {
        public int ReqId { get; } = reqId;
        public DateTimeOffset Time { get; } = DateTimeOffset.FromUnixTimeSeconds(time);
        public double MidPoint { get; } = midPoint;
    }
}
