using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class UpdatePortfolioEventArgs(PortfolioPosition position) : EventArgs
    {
        public PortfolioPosition Position { get; private set; } = position;
    }
}
