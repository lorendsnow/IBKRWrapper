using IBApi;

namespace IBKRWrapper.Events
{
    public class PositionEventArgs(string account, Contract contract, decimal pos, double avgCost)
        : EventArgs
    {
        public string Account { get; } = account;
        public Contract Contract { get; } = contract;
        public decimal Position { get; } = pos;
        public double AverageCost { get; } = avgCost;
    }
}
