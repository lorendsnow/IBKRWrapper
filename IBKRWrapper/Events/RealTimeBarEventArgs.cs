namespace IBKRWrapper.Events
{
    public class RealTimeBarEventArgs(
        int reqId,
        long time,
        double open,
        double high,
        double low,
        double close,
        decimal volume,
        decimal WAP,
        int count
    ) : EventArgs
    {
        public int ReqId { get; } = reqId;
        public DateTimeOffset Time { get; } = DateTimeOffset.FromUnixTimeSeconds(time);
        public double Open { get; } = open;
        public double High { get; } = high;
        public double Low { get; } = low;
        public double Close { get; } = close;
        public decimal Volume { get; } = volume;
        public decimal WAP { get; } = WAP;
        public int Count { get; } = count;
    }
}
