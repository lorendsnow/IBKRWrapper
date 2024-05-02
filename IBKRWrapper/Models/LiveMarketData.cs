using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper.Models
{
    public class LiveMarketData(int reqId, Contract contract)
    {
        public int ReqId { get; init; } = reqId;
        public Contract Contract { get; init; } = contract;
        public List<double> BidPrices { get; private set; } = [];
        public List<decimal> BidSizes { get; private set; } = [];
        public List<double> AskPrices { get; private set; } = [];
        public List<decimal> AskSizes { get; private set; } = [];
        public List<double> LastPrices { get; private set; } = [];
        public List<decimal> LastSizes { get; private set; } = [];
        public List<double> Highs { get; private set; } = [];
        public List<double> Lows { get; private set; } = [];
        public List<decimal> Volumes { get; private set; } = [];
        public double? CloseTick { get; private set; }
        public double? OpenTick { get; private set; }
        public List<DateTimeOffset> TimeStamps { get; private set; } = [];
        public List<OptionGreeks> BidGreeks { get; private set; } = [];
        public List<OptionGreeks> AskGreeks { get; private set; } = [];
        public List<OptionGreeks> LastGreeks { get; private set; } = [];
        public List<OptionGreeks> ModelGreeks { get; private set; } = [];
        public bool Halted { get; private set; }

        public void UpdateMarketData(object? sender, MarketDataEventArgs<double> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            switch ((StandardTickIds)e.Data.Field)
            {
                case StandardTickIds.BidPrice:
                    BidPrices.Add(e.Data.Value);
                    break;

                case StandardTickIds.AskPrice:
                    AskPrices.Add(e.Data.Value);
                    break;

                case StandardTickIds.LastPrice:
                    LastPrices.Add(e.Data.Value);
                    break;

                case StandardTickIds.High:
                    Highs.Add(e.Data.Value);
                    break;

                case StandardTickIds.Low:
                    Lows.Add(e.Data.Value);
                    break;

                case StandardTickIds.ClosePrice:
                    CloseTick ??= e.Data.Value;
                    break;

                case StandardTickIds.OpenTick:
                    OpenTick ??= e.Data.Value;
                    break;

                case StandardTickIds.Halted:
                    Halted = e.Data.Value == 1 || e.Data.Value == 2;
                    break;

                default:
                    break;
            }
        }

        public void UpdateMarketData(object? sender, MarketDataEventArgs<decimal> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            switch ((StandardTickIds)e.Data.Field)
            {
                case StandardTickIds.BidSize:
                    BidSizes.Add(e.Data.Value);
                    break;

                case StandardTickIds.AskSize:
                    AskSizes.Add(e.Data.Value);
                    break;

                case StandardTickIds.LastSize:
                    LastSizes.Add(e.Data.Value);
                    break;

                case StandardTickIds.Volume:
                    Volumes.Add(e.Data.Value);
                    break;

                default:
                    break;
            }
        }

        public void UpdateMarketData(object? sender, MarketDataEventArgs<OptionGreeks> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            switch ((StandardTickIds)e.Data.Field)
            {
                case StandardTickIds.BidOptionComputation:
                    BidGreeks.Add(e.Data.Value);
                    break;

                case StandardTickIds.AskOptionComputation:
                    AskGreeks.Add(e.Data.Value);
                    break;

                case StandardTickIds.LastOptionComputation:
                    LastGreeks.Add(e.Data.Value);
                    break;

                case StandardTickIds.ModelOptionComputation:
                    ModelGreeks.Add(e.Data.Value);
                    break;

                default:
                    break;
            }
        }

        public void UpdateMarketData(object? sender, MarketDataEventArgs<string> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            if ((StandardTickIds)e.Data.Field == StandardTickIds.LastTimestamp)
            {
                TimeStamps.Add(IbDateParser.ParseIBDateTime(e.Data.Value));
            }
        }
    }
}
