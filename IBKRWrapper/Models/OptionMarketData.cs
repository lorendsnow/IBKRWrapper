using IBKRWrapper.Events;
using IBKRWrapper.Constants;
using IBKRWrapper.Utils;
using IBApi;

namespace IBKRWrapper.Models
{
    public class OptionMarketData(int reqId, Contract contract)
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
        public List<OptionGreeks> BidGreeks { get; private set; } = [];
        public List<OptionGreeks> AskGreeks { get; private set; } = [];
        public List<OptionGreeks> LastGreeks { get; private set; } = [];
        public List<OptionGreeks> ModelGreeks { get; private set; } = [];
        public double? OpenTick { get; private set; }
        public List<DateTimeOffset> TimeStamps { get; private set; } = [];

        public void UpdateMarketData<T>(object? sender, MarketDataEventArgs<T> e)
        {
            if (e.Data.RequestId != ReqId) return;

            if (e.Data.Value is double)
            {
                switch ((OptionTickIds)e.Data.Field)
                {
                    case OptionTickIds.BidPrice:
                        BidPrices.Add(e.Data.Value);
                        break;

                    case OptionTickIds.AskPrice:
                        AskPrices.Add(e.Data.Value);
                        break;

                    case OptionTickIds.LastPrice:
                        LastPrices.Add(e.Data.Value);
                        break;
                    
                    case OptionTickIds.High:
                        Highs.Add(e.Data.Value);
                        break;

                    case OptionTickIds.Low:
                        Lows.Add(e.Data.Value);
                        break;
                    
                    case OptionTickIds.OpenTick:
                        if (OpenTick is null) OpenTick = e.Data.Value;
                        break;

                    default:
                        throw new NotImplementedException(
                            $"Field {(OptionTickIds)e.Data.Field} as double isn't a valid data "
                            + "type for option market data"
                            )
                }
            }
            else if (e.Data.Value is int)
            {
                switch ((OptionTickIds)e.Data.Field)
                {
                    case OptionTickIds.BidSize:
                        BidSizes.Add(e.Data.Value);
                        break;
                    
                    case OptionTickIds.AskSize:
                        AskSizes.Add(e.Data.Value);
                        break;

                    case OptionTickIds.LastSize:
                        LastSizes.Add(e.Data.Value);
                        break;

                    case OptionTickIds.Volume:
                        Volumes.Add(e.Data.Value);
                        break;
                    
                    default:
                        throw new NotImplementedException(
                            $"Field {(OptionTickIds)e.Data.Field} as int isn't a valid data type "
                            + "for option market data"
                            )
                }
            }
            else if (e.Data.Value is OptionGreeks)
            {
                switch ((OptionTickIds)e.Data.Field)
                {
                    case OptionTickIds.BidGreeks:
                        BidGreeks.Add(e.Data.Value);
                        break;

                    case OptionTickIds.AskGreeks:
                        AskGreeks.Add(e.Data.Value);
                        break;

                    case OptionTickIds.LastGreeks:
                        LastGreeks.Add(e.Data.Value);
                        break;

                    case OptionTickIds.ModelGreeks:
                        ModelGreeks.Add(e.Data.Value);
                        break;

                    default:
                        throw new NotImplementedException(
                            $"Field {(OptionTickIds)e.Data.Field} as OptionGreeks isn't a valid "
                            + "data type for option market data"
                            )
                }
            }
            else if (e.Data.Value is string && (OptionTickIds)e.Data.Field == OptionTickIds.TimeStamp)
            {
                TimeStamps.Add(IbDateParser.ParseIBDateTime(e.Data.Value));
                throw new 
            }
            else
            {
                throw new ArgumentException("Invalid data type");
            }
        }
    }
}
