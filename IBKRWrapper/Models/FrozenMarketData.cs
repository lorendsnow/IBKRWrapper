using IBApi;
using IBKRWrapper.Constants;
using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents "frozen" market data, which is a snapshot of the market data at the time of the request (or at the close if the request was made after hours)
    /// </summary>
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    public class FrozenMarketData(int reqId, Contract contract)
    {
        /// <summary>
        /// The request ID sent to IBKR when requesting market data
        /// </summary>
        public int ReqId { get; init; } = reqId;

        /// <summary>
        /// The underlying contract
        /// </summary>
        public Contract Contract { get; init; } = contract;

        /// <summary>
        /// The bid price
        /// </summary>
        public double? BidPrice { get; private set; }

        /// <summary>
        /// The bid size
        /// </summary>
        public decimal? BidSize { get; private set; }

        /// <summary>
        /// The ask price
        /// </summary>
        public double? AskPrice { get; private set; }

        /// <summary>
        /// The ask size
        /// </summary>
        public decimal? AskSize { get; private set; }

        /// <summary>
        /// The last traded price
        /// </summary>
        public double? LastPrice { get; private set; }

        /// <summary>
        /// The last traded size
        /// </summary>
        public decimal? LastSize { get; private set; }

        /// <summary>
        /// The high price for the day
        /// </summary>
        public double? High { get; private set; }

        /// <summary>
        /// The low price for the day
        /// </summary>
        public double? Low { get; private set; }

        /// <summary>
        /// The volume for the day
        /// </summary>
        public decimal? Volume { get; private set; }

        /// <summary>
        /// The close price for the day
        /// </summary>
        public double? CloseTick { get; private set; }

        /// <summary>
        /// The open price for the day
        /// </summary>
        public double? OpenTick { get; private set; }

        /// <summary>
        /// The time of the last tick
        /// </summary>
        public DateTimeOffset? TimeStamp { get; private set; }

        /// <summary>
        /// The greeks for the bid price
        /// </summary>
        public OptionGreeks? BidGreeks { get; private set; }

        /// <summary>
        /// The greeks for the ask price
        /// </summary>
        public OptionGreeks? AskGreeks { get; private set; }

        /// <summary>
        /// The greeks for the last price
        /// </summary>
        public OptionGreeks? LastGreeks { get; private set; }

        /// <summary>
        /// The greeks for the model price, based on IBKR's internal model
        /// </summary>
        public OptionGreeks? ModelGreeks { get; private set; }

        /// <summary>
        /// Whether trading in the security is halted
        /// </summary>
        public bool Halted { get; private set; }

        /// <summary>
        /// Receives and handles <see cref="double"/> market data from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Receives and handles <see cref="decimal"/> market data from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Receives and handles <see cref="OptionGreeks"/> market data from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Receives and handles <see cref="string"/> market data from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
