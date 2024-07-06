using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Receives and holds a list of real-time bar data.
    /// </summary>
    /// <param name="reqId">The original request ID sent to IBKR</param>
    /// <param name="contract">The underlying contract</param>
    /// <param name="barSize">The bar duration</param>
    /// <param name="whatToShow">The type of data requested</param>
    /// <param name="useRTH">Whether data is restricted to real-time hours or not</param>
    /// <remarks>
    ///     Includes an event that is triggered and emits the new bar when received from IBKR. Subscribe to <see cref="NewBarEvent"/> to receive new bars.
    /// </remarks>
    public class RealTimeBarList(
        int reqId,
        Contract contract,
        int barSize,
        string whatToShow,
        bool useRTH
    )
    {
        /// <summary>
        /// The list of real-time bars.
        /// </summary>
        public List<RealTimeBar> Bars { get; private set; } = [];

        /// <summary>
        /// The original request ID sent to IBKR.
        /// </summary>
        public int ReqId { get; init; } = reqId;

        /// <summary>
        /// The underlying contract.
        /// </summary>
        public Contract Contract { get; init; } = contract;

        /// <summary>
        /// The bar duration.
        /// </summary>
        public int BarSize { get; init; } = barSize;

        /// <summary>
        /// The type of data requested.
        /// </summary>
        public string WhatToShow { get; init; } = whatToShow;

        /// <summary>
        /// Whether data is restricted to real-time hours or not.
        /// </summary>
        public bool UseRTH { get; init; } = useRTH;

        /// <summary>
        /// An event that is triggered when a new bar is received.
        /// </summary>
        public event EventHandler<NewBarEventArgs>? NewBarEvent;

        /// <summary>
        /// Handles and stores new bars sent by IBKR.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleNewBar(object? sender, RealTimeBarEventArgs e)
        {
            if (e.ReqId != ReqId)
            {
                return;
            }

            NewBarEvent?.Invoke(this, new NewBarEventArgs(e.RealTimeBar));
            Bars.Add(e.RealTimeBar);
        }

        /// <summary>
        /// Returns a string representation of the RealTimeBarList.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return (
                $"RealTimeBarList(ReqId={ReqId}, "
                + $"Contract={ContractString(Contract)}, "
                + $"BarSize={BarSize}, "
                + $"WhatToShow={WhatToShow}, "
                + $"UseRTH={UseRTH}, "
                + $"Bars=[{string.Join(", ", Bars.Select(x => x.ToString()).ToArray())}])"
            );
        }

        private static string ContractString(Contract contract)
        {
            return (
                $"Contract(Ticker={contract.Symbol}, "
                + $"Type={contract.SecType}, "
                + $"Exchange={contract.Exchange}, "
                + $"PrimaryExchange={contract.PrimaryExch})"
            );
        }
    }

    /// <summary>
    /// Represents a single real-time bar.
    /// </summary>
    public record RealTimeBar(
        DateTimeOffset Timeoffset,
        double Open,
        double High,
        double Low,
        double Close,
        decimal? Volume,
        decimal? Wap,
        int? Count
    );

    /// <summary>
    /// Args to hold a new real-time bar for the <see cref="RealTimeBarList.NewBarEvent"/> event.
    /// </summary>
    /// <param name="bar"></param>
    public class NewBarEventArgs(RealTimeBar bar) : EventArgs
    {
        public RealTimeBar Bar { get; private set; } = bar;
    }
}
