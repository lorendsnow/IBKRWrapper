namespace SimpleBroker
{
    /// <summary>
    /// Represents a last tick.
    /// </summary>
    public record TickLast : Tick
    {
        /// <summary>
        /// Whether "Last" (0) or "Alllast" (1). AllLast has additional trade types such as
        /// combos, derivatives, and average price trades which are not included in Last.
        /// </summary>
        public int TickType { get; init; }

        /// <summary>
        /// Last traded price
        /// </summary>
        public double Price { get; init; }

        /// <summary>
        /// Last traded size
        /// </summary>
        public decimal Size { get; init; }

        /// <summary>
        /// Tick attributes that describes additional information for last price ticks
        /// </summary>
        public required TickAttribLast TickAttribLast { get; init; }

        /// <summary>
        /// Exchange the trade was executed on
        /// </summary>
        public required string Exchange { get; init; }

        /// <summary>
        /// Conditions under which the operation took place (refer to
        /// <see href="https://www.interactivebrokers.com/en/index.php?f=7235"/> for more
        /// information)
        /// </summary>
        public required string SpecialConditions { get; init; }
    }
}
