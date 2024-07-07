namespace SimpleBroker
{
    /// <summary>
    /// Represents the greeks of an option.
    /// </summary>
    public record OptionGreeks
    {
        /// <summary>
        /// The request ID sent to IBKR with the initial request
        /// </summary>
        public int ReqId { get; init; }

        /// <summary>
        /// Indicates the field requested
        /// </summary>
        public StandardTickIds Field { get; init; }

        /// <summary>
        /// Indicates whether the tick is return-based or price-based
        /// </summary>
        public OptionTickAttribute TickAttribute { get; init; }

        /// <summary>
        /// Implied volatility of the option
        /// </summary>
        public double ImpliedVolatility { get; init; }

        /// <summary>
        /// Delta of the option
        /// </summary>
        public double Delta { get; init; }

        /// <summary>
        /// Price of the option
        /// </summary>
        public double OptionPrice { get; init; }

        /// <summary>
        /// Present value of the dividend
        /// </summary>
        public double PresentValueDividend { get; init; }

        /// <summary>
        /// Gamma of the option
        /// </summary>
        public double Gamma { get; init; }

        /// <summary>
        /// Vega of the option
        /// </summary>
        public double Vega { get; init; }

        /// <summary>
        /// Theta of the option
        /// </summary>
        public double Theta { get; init; }

        /// <summary>
        /// Price of the underlying
        /// </summary>
        public double UnderlyingPrice { get; init; }
    }
}
