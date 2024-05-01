using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper.Models
{
    public class EquityMarketData(int reqId, Contract contract) : LiveMarketData(reqId, contract)
    {
        public override void UpdateMarketData(object? sender, MarketDataEventArgs<double> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

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
                    OpenTick ??= e.Data.Value;
                    break;

                default:
                    throw new NotImplementedException(
                        $"Field {(EquityTickIds)e.Data.Field} as double isn't a valid data "
                            + "type for equities market data"
                    );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<decimal> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

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

                default:
                    throw new NotImplementedException(
                        $"Field {(EquityTickIds)e.Data.Field} as int isn't a valid data type "
                            + "for equities market data"
                    );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<string> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            if ((EquityTickIds)e.Data.Field == EquityTickIds.LastTimestamp)
            {
                TimeStamps.Add(IbDateParser.ParseIBDateTime(e.Data.Value));
            }
            else
            {
                throw new NotImplementedException(
                    $"Field {(EquityTickIds)e.Data.Field} as string isn't a valid data type "
                        + "for equities market data"
                );
            }
        }

        public override void UpdateMarketData(object? sender, MarketDataEventArgs<OptionGreeks> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            throw new NotImplementedException(
                "Option greeks are not a valid data type for equities market data"
            );
        }
    }
}
