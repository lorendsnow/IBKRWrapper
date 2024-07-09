namespace SimpleBroker
{
    /// <summary>
    /// Tick attributes that describe additional information for last price ticks.
    /// </summary>
    public record TickAttribLast
    {
        /// <summary>
        /// Not currently used with trade data; only applies to bid/ask data.
        /// </summary>
        public bool PastLimit { get; init; }

        /// <summary>
        /// Used with tick-by-tick last data or historical ticks last to indicate if a trade is
        /// classified as 'unreportable' (odd lots, combos, derivative trades, etc)
        /// </summary>
        public bool Unreported { get; init; }
    }
}
