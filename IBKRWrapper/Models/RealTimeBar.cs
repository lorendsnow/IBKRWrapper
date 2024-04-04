using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class RealTimeBarList
    {
        public List<RealTimeBar> Bars { get; set; } = [];
        public int ReqId { get; init; }
        public required Contract Contract { get; init; }
        public int BarSize { get; init; } = 5;
        public required string WhatToShow { get; init; }
        public bool UseRTH { get; init; }

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

    public class NewBarEventArgs(RealTimeBar bar) : EventArgs
    {
        public RealTimeBar Bar { get; set; } = bar;
    }
}
