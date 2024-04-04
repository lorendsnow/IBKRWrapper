using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class TickDataList
    {
        public int ReqId { get; set; }
        public required Contract Contract { get; set; }
        public required string TickType { get; set; }

        public required List<TickData> Data { get; set; }

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

    public class NewTickEventArgs(TickData tick) : EventArgs
    {
        public TickData Tick { get; set; } = tick;
    }
}
