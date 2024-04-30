using IBKRWrapper.Events;
using IBKRWrapper.Constants;
using IBKRWrapper.Utils;
using IBApi;

namespace IBKRWrapper.Models
{
    public class EquityMarketData(int reqId, Contract contract): LiveMarketData(reqId, contract)
    {
        public override void UpdateMarketData<T>(object? sender, MarketDataEventArgs<T> e)
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

                    default:
                        throw new NotImplementedException(
                            $"Field {(EquityTickIds)e.Data.Field} as double isn't a valid data "
                            + "type for equities market data"
                            )
                }
            }
            else if (e.Data.Value is decimal)
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
                    
                    default:
                        throw new NotImplementedException(
                            $"Field {(EquityTickIds)e.Data.Field} as int isn't a valid data type "
                            + "for equities market data"
                            )
                }
            }
            else if (e.Data.Value is string && (EquityTickIds)e.Data.Field == EquityTickIds.TimeStamp)
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
