using IBApi;

namespace IBKRWrapper.Events
{
    public class TickLastEventArgs(
        int reqId,
        int tickType,
        long time,
        double price,
        decimal size,
        TickAttribLast tickAttribLast,
        string exchange,
        string specialConditions
    ) : EventArgs
    {
        public int ReqId { get; } = reqId;
        public int TickType { get; } = tickType;
        public DateTimeOffset Time { get; } = DateTimeOffset.FromUnixTimeMilliseconds(time);
        public double LastPrice { get; } = price;
        public decimal LastSize { get; } = size;
        public TickAttribLast? TickAttribLast { get; } = tickAttribLast;
        public string Exchange { get; } = exchange;
        public string SpecialConditions { get; } = specialConditions;
    }
}
