using IBKRWrapper.Events;
using IBKRWrapper.Utils;

namespace SimpleBroker
{
    /// <summary>
    /// Represents a general live market data subscription
    /// </summary>
    /// <param name="reqId"></param>
    /// <param name="contract"></param>
    /// <remarks>Receives and stores bid, ask, last, low, high, volume, close and open ticks. If the underlying security is an option, it also holds bid, ask and last greek values</remarks>
    public class LiveMarketData(int reqId, Contract contract)
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
        /// List of bid prices received from IBKR
        /// </summary>
        public List<double> BidPrices { get; private set; } = [];

        /// <summary>
        /// List of bid sizes received from IBKR
        /// </summary>
        public List<decimal> BidSizes { get; private set; } = [];

        /// <summary>
        /// List of ask prices received from IBKR
        /// </summary>
        public List<double> AskPrices { get; private set; } = [];

        /// <summary>
        /// List of ask sizes received from IBKR
        /// </summary>
        public List<decimal> AskSizes { get; private set; } = [];

        /// <summary>
        /// List of last trade prices received from IBKR
        /// </summary>
        public List<double> LastPrices { get; private set; } = [];

        /// <summary>
        /// List of last trade sizes received from IBKR
        /// </summary>
        public List<decimal> LastSizes { get; private set; } = [];

        /// <summary>
        /// High price for the day
        /// </summary>
        public List<double> Highs { get; private set; } = [];

        /// <summary>
        /// Low price for the day
        /// </summary>
        public List<double> Lows { get; private set; } = [];

        /// <summary>
        /// Trading volume for the day
        /// </summary>
        public List<decimal> Volumes { get; private set; } = [];

        /// <summary>
        /// The closing price for the day
        /// </summary>
        public double? CloseTick { get; private set; }

        /// <summary>
        /// The opening price for the day
        /// </summary>
        public double? OpenTick { get; private set; }

        /// <summary>
        /// Timestamps for time of last trade
        /// </summary>
        public List<DateTimeOffset> TimeStamps { get; private set; } = [];

        /// <summary>
        /// List of bid greeks received from IBKR
        /// </summary>
        public List<OptionGreeks> BidGreeks { get; private set; } = [];

        /// <summary>
        /// List of ask greeks received from IBKR
        /// </summary>
        public List<OptionGreeks> AskGreeks { get; private set; } = [];

        /// <summary>
        /// List of last trade greeks received from IBKR
        /// </summary>
        public List<OptionGreeks> LastGreeks { get; private set; } = [];

        /// <summary>
        /// List of model greeks received from IBKR
        /// </summary>
        public List<OptionGreeks> ModelGreeks { get; private set; } = [];

        /// <summary>
        /// Whether trading in the security is halted
        /// </summary>
        public bool Halted { get; private set; }

        /// <summary>
        /// Handles and stores <see cref="double"/> tick values emitted by IBKR
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

        /// <summary>
        /// Handles and stores <see cref="decimal"/> tick values emitted by IBKR
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

        /// <summary>
        /// Handles and stores <see cref="OptionGreeks"/> tick values emitted by IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateMarketData(
            object? sender,
            MarketDataEventArgs<IBKRWrapper.Models.OptionGreeks> e
        )
        {
            if (e.Data.ReqId != ReqId)
                return;

            switch ((StandardTickIds)e.Data.Field)
            {
                case StandardTickIds.BidOptionComputation:
                    BidGreeks.Add(e.Data.Value.ToBrokerOptionGreeks());
                    break;

                case StandardTickIds.AskOptionComputation:
                    AskGreeks.Add(e.Data.Value.ToBrokerOptionGreeks());
                    break;

                case StandardTickIds.LastOptionComputation:
                    LastGreeks.Add(e.Data.Value.ToBrokerOptionGreeks());
                    break;

                case StandardTickIds.ModelOptionComputation:
                    ModelGreeks.Add(e.Data.Value.ToBrokerOptionGreeks());
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handles and stores <see cref="string"/> tick values emitted by IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
