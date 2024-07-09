using IBApi;

namespace IBKRWrapper.Events
{
    public class TickBidAskEventArgs(
        int reqId,
        long time,
        double bidPrice,
        double askPrice,
        decimal bidSize,
        decimal askSize,
        TickAttribBidAsk tickAttribBidAsk
    ) : EventArgs
    {
        public int ReqId { get; } = reqId;
        public DateTimeOffset Time { get; } = DateTimeOffset.FromUnixTimeSeconds(time);
        public double BidPrice { get; } = bidPrice;
        public double AskPrice { get; } = askPrice;
        public decimal BidSize { get; } = bidSize;
        public decimal AskSize { get; } = askSize;
        public TickAttribBidAsk? TickAttribBidAsk { get; } = tickAttribBidAsk;
    }
}
