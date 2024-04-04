using IBApi;

namespace IBKRWrapper.Models
{
    public record PortfolioPosition(
        Contract Contract,
        decimal Position,
        double MarketPrice,
        double MarketValue,
        double AverageCost,
        double UnrealizedPNL,
        double RealizedPNL,
        string AccountName
    )
    {
        public string Ticker { get; init; } = Contract.Symbol;
        public string SecType { get; init; } = Contract.SecType;
        public string Exchange { get; init; } = Contract.Exchange;
        public string PrimaryExch { get; init; } = Contract.PrimaryExch;
        public decimal Shares { get; init; } = Position;
        public double MarketPrice { get; init; } = MarketPrice;
        public double MarketValue { get; init; } = MarketValue;
        public double AverageCost { get; init; } = AverageCost;
        public double UnrealizedPNL { get; init; } = UnrealizedPNL;
        public double RealizedPNL { get; init; } = RealizedPNL;
        public string AccountName { get; init; } = AccountName;
    }
}
