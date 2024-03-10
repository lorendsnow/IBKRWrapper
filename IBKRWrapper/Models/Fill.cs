using IBApi;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a fill received from IBKR.
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="execution"></param>
    public class Fill(Contract contract, Execution execution)
    {
        public string Ticker { get; set; } = contract.Symbol;
        public decimal Shares { get; set; } = execution.Shares;
        public string Side { get; set; } = execution.Side;
        public double Price { get; set; } = execution.Price;
        public decimal CumQty { get; set; } = execution.CumQty;
        public string Time { get; set; } = execution.Time;
        public string Exchange { get; set; } = execution.Exchange;
        public int OrderId { get; set; } = execution.OrderId;
        public string ExecId { get; set; } = execution.ExecId;
        public string OrderRef { get; set; } = execution.OrderRef;
        public double? Commission { get; set; }
    }
}
