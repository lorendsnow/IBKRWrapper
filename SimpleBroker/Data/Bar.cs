namespace SimpleBroker
{
    /// <summary>
    /// Represents a historical OHLC bar. If TRADES data was requested, will also include volume,
    /// weighted average price, and the number of trades during the bar's time period
    /// </summary>
    public record Bar
    {
        /// <summary>
        /// Symbol of the underlying instrument
        /// </summary>
        public required string Symbol { get; init; }

        /// <summary>
        /// The bar's timestamp
        /// </summary>
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// The bar's open price
        /// </summary>
        public double Open { get; init; }

        /// <summary>
        /// The bar's high price
        /// </summary>
        public double High { get; init; }

        /// <summary>
        /// The bar's low price
        /// </summary>
        public double Low { get; init; }

        /// <summary>
        /// The bar's close price
        /// </summary>
        public double Close { get; init; }

        /// <summary>
        /// The bar's volume (only available for TRADES data)
        /// </summary>
        public decimal Volume { get; init; }

        /// <summary>
        /// The bar's weighted average price (only available for TRADES data)
        /// </summary>
        public decimal WAP { get; init; }

        /// <summary>
        /// The number of trades during the bar's timespan (only available for TRADES data)
        /// </summary>
        public int Count { get; init; }
    }
}
