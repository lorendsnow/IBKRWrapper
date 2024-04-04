using IBApi;

namespace IBKRWrapper.Models
{
    public record Position
    {
        public Position(string account, Contract contract, decimal pos, double avgCost)
        {
            ConId = contract.ConId;
            Symbol = contract.Symbol;
            SecType = contract.SecType;
            Exchange = contract.Exchange;
            Currency = contract.Currency;
            PrimaryExch = contract.PrimaryExch;
            Positions = pos;
            AvgCost = avgCost;
            Account = account;
        }

        public int ConId { get; init; }
        public string Symbol { get; init; }
        public string SecType { get; init; }
        public string Exchange { get; init; }
        public string Currency { get; init; }
        public string PrimaryExch { get; init; }
        public decimal Positions { get; init; }
        public double AvgCost { get; init; }
        public string Account { get; init; }
    }
}
