using IBKRWrapper.Events;

namespace SimpleBroker
{
    /// <summary>
    /// Represents an options chain for a security.
    /// </summary>
    public class OptionsChain
    {
        /// <summary>
        /// The original request ID sent to the TWS API.
        /// </summary>
        public int ReqId { get; init; }

        /// <summary>
        /// The symbol of the security.
        /// </summary>
        public required string Symbol { get; init; }

        /// <summary>
        /// The IBKR contract ID of the underlying security.
        /// </summary>
        public int UnderlyingConId { get; private set; }

        /// <summary>
        /// The trading class of the security.
        /// </summary>
        public string? TradingClass { get; private set; }

        /// <summary>
        /// The derivative's multiplier.
        /// </summary>
        public string? Multiplier { get; private set; }

        /// <summary>
        /// The exchange on which the security is traded.
        /// </summary>
        public string? Exchange { get; private set; }

        /// <summary>
        /// All expiration dates for the options chain.
        /// </summary>
        public HashSet<string>? Expirations { get; private set; }

        /// <summary>
        /// All strike prices for the options chain (across all dates).
        /// </summary>
        public HashSet<double>? Strikes { get; private set; }

        /// <summary>
        /// Event handler to process and save incoming options chain data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleOptionsChainData(object? sender, OptionsChainEventArgs e)
        {
            if (e.ReqId == ReqId)
            {
                UnderlyingConId = e.UnderlyingConId;
                TradingClass = e.TradingClass;
                Multiplier = e.Multiplier;
                Exchange = e.Exchange;

                if (Expirations is null)
                {
                    Expirations = e.Expirations;
                }
                else
                {
                    Expirations.UnionWith(e.Expirations);
                }

                if (Strikes is null)
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
