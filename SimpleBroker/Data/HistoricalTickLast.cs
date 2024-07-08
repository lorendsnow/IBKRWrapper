namespace SimpleBroker
{
    /// <summary>
    /// Represents a historical tick based on the last trade
    /// </summary>
    public record HistoricalTickLast
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
        /// The tick's price
        /// </summary>
        public double Price { get; init; }

        /// <summary>
        /// The tick's size
        /// </summary>
        public decimal Size { get; init; }

        /// <summary>
        /// The source exchange of the tick
        /// </summary>
        public required string Exchange { get; init; }

        /// <summary>
        /// The conditions of the historical tick. Refer to IBKR's Trade Conditions page for more
        /// details: <see href="https://www.interactivebrokers.com/en/index.php?f=7235"/>
        /// </summary>
        public required string SpecialConditions { get; init; }
    }
}
