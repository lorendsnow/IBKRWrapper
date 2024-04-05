using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class TickDataList(int reqId, Contract contract, string tickType)
    {
        public int ReqId { get; init; } = reqId;
        public Contract Contract { get; init; } = contract;
        public string TickType { get; init; } = tickType;

        public List<TickData> Data { get; set; } = [];

        public event EventHandler<NewTickEventArgs>? NewTickEvent;

        public void HandleNewTick(object? sender, TickByTickEventArgs e)
        {
            if (e.ReqId != ReqId)
            {
                return;
            }

            NewTickEvent?.Invoke(this, new NewTickEventArgs(e.TickData));
            Data.Add(e.TickData);
        }

        public void CancelTickData(Wrapper wrapper)
        {
            wrapper.ClientSocket.cancelTickByTickData(ReqId);
        }

        public override string ToString()
        {
            return (
                $"TickDataList(ReqId={ReqId}, "
                + $"Contract={ContractString(Contract)}, "
                + $"TickType={TickType}, "
                + $"Data=[{string.Join(", ", Data.Select(x => x.ToString()).ToArray())}])"
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
        public TickData Tick { get; private set; } = tick;
    }
}
