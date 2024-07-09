using IBApi;

namespace IBKRWrapper.Events
{
    public class UpdatePortfolioEventArgs(
        Contract contract,
        decimal position,
        double marketPrice,
        double marketValue,
        double averageCost,
        double unrealizedPNL,
        double realizedPNL,
        string accountName
    ) : EventArgs
    {
        public Contract Contract { get; } = contract;
        public decimal Position { get; } = position;
        public double MarketPrice { get; } = marketPrice;
        public double MarketValue { get; } = marketValue;
        public double AverageCost { get; } = averageCost;
        public double UnrealizedPNL { get; } = unrealizedPNL;
        public double RealizedPNL { get; } = realizedPNL;
        public string Account { get; } = accountName;
    }
}
