namespace SimpleBroker
{
    /// <summary>
    /// Represents a historical bid/ask tick
    /// </summary>
    public record HistoricalTickBidAsk
    {
        /// <summary>
        /// The underlying instrument's symbol
        /// </summary>
        public required string Symbol { get; init; }

        /// <summary>
        /// The tick's timestamp
        /// </summary>
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// The bid price
        /// </summary>
        public double PriceBid { get; init; }

        /// <summary>
        /// The ask price
        /// </summary>
        public double PriceAsk { get; init; }

        /// <summary>
        /// The bid size
        /// </summary>
        public decimal SizeBid { get; init; }

        /// <summary>
        /// The ask size
        /// </summary>
        public decimal SizeAsk { get; init; }
    }
}
