namespace IBKRWrapper.Events
{
    public class OptionsChainEventArgs(
        int reqId,
        string exchange,
        int underlyingConId,
        string tradingClass,
        string multiplier,
        HashSet<string> expirations,
        HashSet<double> strikes
    ) : EventArgs
    {
        public int ReqId { get; init; } = reqId;
        public string Exchange { get; init; } = exchange;
        public int UnderlyingConId { get; init; } = underlyingConId;
        public string TradingClass { get; init; } = tradingClass;
        public string Multiplier { get; init; } = multiplier;
        public HashSet<string> Expirations { get; init; } = expirations;
        public HashSet<double> Strikes { get; init; } = strikes;
    }
}
