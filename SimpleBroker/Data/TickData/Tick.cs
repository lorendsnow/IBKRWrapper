namespace SimpleBroker
{
    /// <summary>
    /// Base record class for tick data.
    /// </summary>
    public abstract record Tick
    {
        /// <summary>
        /// The request ID sent to IBKR when requesting tick data.
        /// </summary>
        public int ReqId { get; init; }

        /// <summary>
        /// The tick's timestamp
        /// </summary>
        public DateTimeOffset Time { get; init; }
    }
}
