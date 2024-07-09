namespace SimpleBroker
{
    /// <summary>
    /// Tick data for bid/ask ticks.
    /// </summary>
    public record TickBidAsk : Tick
    {
        /// <summary>
        /// Bid price
        /// </summary>
        public double BidPrice { get; init; }

        /// <summary>
        /// Ask price
        /// </summary>
        public double AskPrice { get; init; }

        /// <summary>
        /// Bid size
        /// </summary>
        public decimal BidSize { get; init; }

        /// <summary>
        /// Ask size
        /// </summary>
        public decimal AskSize { get; init; }

        /// <summary>
        /// Tick attribute that describes additional information for bid/ask ticks
        /// </summary>
        public required TickAttribBidAsk TickAttribBidAsk { get; init; }
    }
}
