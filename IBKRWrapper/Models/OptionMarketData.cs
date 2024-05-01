using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper.Models
{
    public class OptionMarketData : LiveMarketData
    {
        public OptionMarketData(int reqId, Contract contract)
            : base(reqId, contract) { }

        public List<OptionGreeks> BidGreeks { get; private set; } = [];
        public List<OptionGreeks> AskGreeks { get; private set; } = [];
        public List<OptionGreeks> LastGreeks { get; private set; } = [];
        public List<OptionGreeks> ModelGreeks { get; private set; } = [];

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<double> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

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
                    if (OpenTick is null)
                        OpenTick = e.Data.Value;
                    break;

                default:
                    throw new NotImplementedException(
                        $"Field {(OptionTickIds)e.Data.Field} as double isn't a valid data "
                            + "type for option market data"
                    );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<decimal> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

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
                    );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<OptionGreeks> e)
        {
            switch ((OptionTickIds)e.Data.Field)
            {
                case OptionTickIds.BidOptionComputation:
                    BidGreeks.Add(e.Data.Value);
                    break;

                case OptionTickIds.AskOptionComputation:
                    AskGreeks.Add(e.Data.Value);
                    break;

                case OptionTickIds.LastOptionComputation:
                    LastGreeks.Add(e.Data.Value);
                    break;

                case OptionTickIds.ModelOptionComputation:
                    ModelGreeks.Add(e.Data.Value);
                    break;

                default:
                    throw new NotImplementedException(
                        $"Field {(OptionTickIds)e.Data.Field} as OptionGreeks isn't a valid "
                            + "data type for option market data"
                    );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<string> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            if ((OptionTickIds)e.Data.Field == OptionTickIds.LastTimestamp)
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
