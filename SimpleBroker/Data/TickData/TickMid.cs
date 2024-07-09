namespace SimpleBroker
{
    /// <summary>
    /// Tick data for mid ticks.
    /// </summary>
    public record TickMid : Tick
    {
        /// <summary>
        /// Mid price
        /// </summary>
        public double Price { get; init; }
    }
}
