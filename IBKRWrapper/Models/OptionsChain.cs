using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class OptionsChain
    {
        public int ReqId { get; init; }
        public required string Symbol { get; init; }
        public int UnderlyingConId { get; private set; }
        public string? TradingClass { get; private set; }
        public string? Multiplier { get; private set; }
        public string? Exchange { get; private set; }
        public HashSet<string>? Expirations { get; private set; }
        public HashSet<double>? Strikes { get; private set; }

        public void HandleOptionsChainData(object? sender, OptionsChainEventArgs e)
        {
            if (e.ReqId == ReqId)
            {
                UnderlyingConId = e.UnderlyingConId;
                TradingClass = e.TradingClass;
                Multiplier = e.Multiplier;
                Exchange = e.Exchange;

                if (Expirations == null)
                {
                    Expirations = e.Expirations;
                }
                else
                {
                    Expirations.UnionWith(e.Expirations);
                }

                if (Strikes == null)
                {
                    Strikes = e.Strikes;
                }
                else
                {
                    Strikes.UnionWith(e.Strikes);
                }
            }
        }
    }
}
