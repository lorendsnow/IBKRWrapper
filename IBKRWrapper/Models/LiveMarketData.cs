namespace IBKRWrapper.Models
{
    public abstract class LiveMarketData
    {
        public int ReqId { get; init; } 
        public Contract Contract { get; init; }
        public List<double> BidPrices { get; private set; } = [];
        public List<decimal> BidSizes { get; private set; } = [];
        public List<double> AskPrices { get; private set; } = [];
        public List<decimal> AskSizes { get; private set; } = [];
        public List<double> LastPrices { get; private set; } = [];
        public List<decimal> LastSizes { get; private set; } = [];
        public List<double> Highs { get; private set; } = [];
        public List<double> Lows { get; private set; } = [];
        public List<decimal> Volumes { get; private set; } = [];
        public double? OpenTick { get; private set; }
        public List<DateTimeOffset> TimeStamps { get; private set; } = [];

        protected LiveMarketData(int reqId, Contract contract)
        {
            ReqId = reqId;
            Contract = contract;
        }

        public abstract void UpdateMarketData<T>(object? sender, MarketDataEventArgs<T> e);
    }
}
