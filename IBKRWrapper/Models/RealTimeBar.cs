using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class RealTimeBarList(
        int reqId,
        Contract contract,
        int barSize,
        string whatToShow,
        bool useRTH
    )
    {
        public List<RealTimeBar> Bars { get; private set; } = [];
        public int ReqId { get; init; } = reqId;
        public Contract Contract { get; init; } = contract;
        public int BarSize { get; init; } = barSize;
        public string WhatToShow { get; init; } = whatToShow;
        public bool UseRTH { get; init; } = useRTH;

        public event EventHandler<NewBarEventArgs>? NewBarEvent;

        public void HandleNewBar(object? sender, RealTimeBarEventArgs e)
        {
            if (e.ReqId != ReqId)
            {
                return;
            }

            NewBarEvent?.Invoke(this, new NewBarEventArgs(e.RealTimeBar));
            Bars.Add(e.RealTimeBar);
        }

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

    public class NewBarEventArgs(RealTimeBar bar) : EventArgs
    {
        public RealTimeBar Bar { get; private set; } = bar;
    }
}
