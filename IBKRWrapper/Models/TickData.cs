using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a list of tick data received from IBKR, and holds the initial request details.
    /// </summary>
    public class TickDataList
    {
        public int ReqId { get; set; }
        public required Contract Contract { get; set; }
        public required string TickType { get; set; }

        /// <summary>
        /// List of tick data received from IBKR.
        /// </summary>
        public required List<TickData> Data { get; set; }

        /// <summary>
        /// Emits a tick upon receipt from IBKR.
        /// </summary>
        public event EventHandler<NewTickEventArgs>? NewTickEvent;

        public void HandleNewTick(object? sender, IBKRTickEventArgs e)
        {
            if (e.ReqId != ReqId)
            {
                return;
            }
            OnNewTick(e.TickData);
            Data.Add(e.TickData);
        }

        public void OnNewTick(TickData tick)
        {
            NewTickEvent?.Invoke(this, new NewTickEventArgs(tick));
        }

        public void CancelTickData(Wrapper wrapper)
        {
            wrapper.ClientSocket.cancelTickByTickData(ReqId);
        }
    }

    /// <summary>
    /// Object representing a tick from IBKR.
    /// </summary>
    public record TickData
    {
        public DateTimeOffset Time { get; init; }
        public double? Last { get; init; }
        public double? Bid { get; init; }
        public double? Ask { get; init; }
        public double? Mid { get; init; }
        public decimal? LastSize { get; init; }
        public decimal? BidSize { get; init; }
        public decimal? AskSize { get; init; }
        public string? Exchange { get; init; }
        public string? SpecialConditions { get; init; }
    }

    /// <summary>
    /// Contains a <see cref="TickData"/> object from IBKR.
    /// </summary>
    public class NewTickEventArgs : EventArgs
    {
        public NewTickEventArgs(TickData tick)
        {
            Tick = tick;
        }

        public TickData Tick { get; set; }
    }
}
