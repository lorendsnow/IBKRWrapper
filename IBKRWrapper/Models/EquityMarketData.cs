using IBKRWrapper.Events;
using IBKRWrapper.Constants;
using IBKRWrapper.Utils;
using IBApi;

namespace IBKRWrapper.Models
{
    public class EquityMarketData(int reqId, Contract contract)
    {
        public int ReqId { get; init; } = reqId;
        public Contract Contract { get; init; } = contract;
        public List<double> BidPrices { get; private set; } = [];
        public List<int> BidSizes { get; private set; } = [];
        public List<double> AskPrices { get; private set; } = [];
        public List<int> AskSizes { get; private set; } = [];
        public List<double> LastPrices { get; private set; } = [];
        public List<int> LastSizes { get; private set; } = [];
        public List<double> Highs { get; private set; } = [];
        public List<double> Lows { get; private set; } = [];
        public List<int> Volumes { get; private set; } = [];
        public double? OpenTick { get; private set; }
        public List<DateTimeOffset> TimeStamps { get; private set; } = [];

        public void UpdateMarketData<T>(object? sender, MarketDataEventArgs<T> e)
        {
            if (e.Data.RequestId != ReqId) return;

            if (e.Data.Value is double)
            {
                switch ((EquityTickIds)e.Data.Field)
                {
                    case EquityTickIds.BidPrice:
                        BidPrices.Add(e.Data.Value);
                        break;

                    case EquityTickIds.AskPrice:
                        AskPrices.Add(e.Data.Value);
                        break;

                    case EquityTickIds.LastPrice:
                        LastPrices.Add(e.Data.Value);
                        break;
                    
                    case EquityTickIds.High:
                        Highs.Add(e.Data.Value);
                        break;

                    case EquityTickIds.Low:
                        Lows.Add(e.Data.Value);
                        break;
                    
                    case EquityTickIds.OpenTick:
                        if (OpenTick is null) OpenTick = e.Data.Value;
                        break;
                }
            }
            else if (e.Data.Value is int)
            {
                switch ((EquityTickIds)e.Data.Field)
                {
                    case EquityTickIds.BidSize:
                        BidSizes.Add(e.Data.Value);
                        break;
                    
                    case EquityTickIds.AskSize:
                        AskSizes.Add(e.Data.Value);
                        break;

                    case EquityTickIds.LastSize:
                        LastSizes.Add(e.Data.Value);
                        break;

                    case EquityTickIds.Volume:
                        Volumes.Add(e.Data.Value);
                        break;
                }
            }
            else if (e.Data.Value is string && (EquityTickIds)e.Data.Field == EquityTickIds.TimeStamp)
            {
                TimeStamps.Add(IbDateParser.ParseIBDateTime(e.Data.Value));
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }
    }
}
