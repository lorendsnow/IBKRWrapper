using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper.Models
{
    public class FrozenMarketData(int reqId, Contract contract)
    {
        public int ReqId { get; init; } = reqId;
        public Contract Contract { get; init; } = contract;
        public double? BidPrice { get; private set; }
        public decimal? BidSize { get; private set; }
        public double? AskPrice { get; private set; }
        public decimal? AskSize { get; private set; }
        public double? LastPrice { get; private set; }
        public decimal? LastSize { get; private set; }
        public double? High { get; private set; }
        public double? Low { get; private set; }
        public decimal? Volume { get; private set; }
        public double? CloseTick { get; private set; }
        public double? OpenTick { get; private set; }
        public DateTimeOffset? TimeStamp { get; private set; }
        public OptionGreeks? BidGreeks { get; private set; }
        public OptionGreeks? AskGreeks { get; private set; }
        public OptionGreeks? LastGreeks { get; private set; }
        public OptionGreeks? ModelGreeks { get; private set; }
        public bool Halted { get; private set; }

        public void UpdateMarketData(object? sender, MarketDataEventArgs<double> e)
        {
            if (e.Data.ReqId != ReqId)
                return;

            switch ((StandardTickIds)e.Data.Field)
            {
                case StandardTickIds.BidPrice:
                    BidPrice = e.Data.Value;
                    break;

                case StandardTickIds.AskPrice:
                    AskPrice = e.Data.Value;
                    break;

                case StandardTickIds.LastPrice:
                    LastPrice = e.Data.Value;
                    break;

                case StandardTickIds.High:
                    High = e.Data.Value;
                    break;

                case StandardTickIds.Low:
                    Low = e.Data.Value;
                    break;

                case StandardTickIds.ClosePrice:
                    CloseTick = e.Data.Value;
                    break;

                case StandardTickIds.OpenTick:
                    OpenTick = e.Data.Value;
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
                    BidSize = e.Data.Value;
                    break;

                case StandardTickIds.AskSize:
                    AskSize = e.Data.Value;
                    break;

                case StandardTickIds.LastSize:
                    LastSize = e.Data.Value;
                    break;

                case StandardTickIds.Volume:
                    Volume = e.Data.Value;
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
                    BidGreeks = e.Data.Value;
                    break;

                case StandardTickIds.AskOptionComputation:
                    AskGreeks = e.Data.Value;
                    break;

                case StandardTickIds.LastOptionComputation:
                    LastGreeks = e.Data.Value;
                    break;

                case StandardTickIds.ModelOptionComputation:
                    ModelGreeks = e.Data.Value;
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
                TimeStamp = IbDateParser.ParseIBDateTime(e.Data.Value);
            }
        }
    }
}
