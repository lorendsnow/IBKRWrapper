using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Object which holds real-time bars from IBKR, together with details of the initial request.
    /// </summary>
    public class RealTimeBarList
    {
        /// <summary>
        /// List of real-time bars received from IBKR.
        /// </summary>
        public List<RealTimeBar> Bars { get; set; } = [];
        public int ReqId { get; init; }
        public required Contract Contract { get; init; }
        public int BarSize { get; init; } = 5;
        public required string WhatToShow { get; init; }
        public bool UseRTH { get; init; }

        /// <summary>
        /// Emits a bar when a new real-time bar is received from IBKR.
        /// </summary>
        public event EventHandler<NewBarEventArgs>? NewBarEvent;

        public void HandleNewBar(object? sender, IBKRrtBarEventArgs e)
        {
            if (e.ReqId != ReqId)
            {
                return;
            }
            OnNewBar(e.RealTimeBar);
            Bars.Add(e.RealTimeBar);
        }

        public void OnNewBar(RealTimeBar bar)
        {
            NewBarEvent?.Invoke(this, new NewBarEventArgs(bar));
        }
    }

    /// <summary>
    /// Bar object representing a real-time bar from IBKR.
    /// </summary>
    public record RealTimeBar
    {
        public DateTimeOffset TimeOffset { get; init; }
        public double Open { get; init; }
        public double High { get; init; }
        public double Low { get; init; }
        public double Close { get; init; }
        public decimal? Volume { get; init; }
        public decimal? WAP { get; init; }
        public int? Count { get; init; }
    }

    /// <summary>
    /// Contains a <see cref="RealTimeBar"/> received from IBKR.
    /// </summary>
    public class NewBarEventArgs : EventArgs
    {
        public NewBarEventArgs(RealTimeBar bar)
        {
            Bar = bar;
        }

        public RealTimeBar Bar { get; set; }
    }
}
