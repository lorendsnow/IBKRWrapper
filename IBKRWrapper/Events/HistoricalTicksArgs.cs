using IBApi;

namespace IBKRWrapper.Events
{
    public class HistoricalTicksMidEventArgs(int reqId, HistoricalTick[] ticks, bool done)
        : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public HistoricalTick[] Ticks { get; private set; } = ticks;
        public bool Done { get; private set; } = done;
    }

    public class HistoricalTicksBidAskEventArgs(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public HistoricalTickBidAsk[] Ticks { get; private set; } = ticks;
        public bool Done { get; private set; } = done;
    }

    public class HistoricalTicksLastEventArgs(int reqId, HistoricalTickLast[] ticks, bool done)
        : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public HistoricalTickLast[] Ticks { get; private set; } = ticks;
        public bool Done { get; private set; } = done;
    }
}
