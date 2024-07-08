namespace SimpleBroker
{
    /// <summary>
    /// Represents a historical midprice tick
    /// </summary>
    public record HistoricalTick
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
        /// The midprice
        /// </summary>
        public double Price { get; init; }

        /// <summary>
        /// The size of the tick
        /// </summary>
        public decimal Size { get; init; }
    }
}
