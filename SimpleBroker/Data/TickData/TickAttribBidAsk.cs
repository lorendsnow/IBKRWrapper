namespace SimpleBroker
{
    /// <summary>
    /// Attribute that describes additional information for bid/ask ticks
    /// </summary>
    public record TickAttribBidAsk
    {
        /// <summary>
        /// Used with real time tick-by-tick. Indicates if bid is lower than day's lowest low.
        /// </summary>
        public bool BidPastLow { get; init; }

        /// <summary>
        /// Used with real time tick-by-tick. Indivates if ask is higher than day's highest high.
        /// </summary>
        public bool AskPastHigh { get; init; }
    }
}
