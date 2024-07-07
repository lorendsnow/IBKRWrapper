namespace SimpleBroker
{
    /// <summary>
    /// Indicates whether the tick is return-based or price-based
    /// </summary>
    public enum OptionTickAttribute
    {
        /// <summary>
        /// Return based
        /// </summary>
        ReturnBased = 0,

        /// <summary>
        /// Price based
        /// </summary>
        PriceBased = 1,
    }
}
