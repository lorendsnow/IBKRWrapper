using IBKRWrapper.Constants;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents the greeks of an option.
    /// </summary>
    /// <param name="ReqId"></param>
    /// <param name="FieldCode"></param>
    /// <param name="TickAttrib"></param>
    /// <param name="ImpliedVolatility"></param>
    /// <param name="Delta"></param>
    /// <param name="OptPrice"></param>
    /// <param name="PvDividend"></param>
    /// <param name="Gamma"></param>
    /// <param name="Vega"></param>
    /// <param name="Theta"></param>
    /// <param name="UndPrice"></param>
    public record OptionGreeks(
        int ReqId,
        int FieldCode,
        int TickAttrib,
        double ImpliedVolatility,
        double Delta,
        double OptPrice,
        double PvDividend,
        double Gamma,
        double Vega,
        double Theta,
        double UndPrice
    )
    {
        /// <summary>
        /// The request ID sent to IBKR with the initial request
        /// </summary>
        public int ReqId { get; init; } = ReqId;

        /// <summary>
        /// Indicates the field requested
        /// </summary>
        public StandardTickIds Field { get; init; } = (StandardTickIds)FieldCode;

        /// <summary>
        /// Indicates whether the tick is return-based or price-based
        /// </summary>
        public OptionTickAttribute TickAttribute { get; init; } = (OptionTickAttribute)TickAttrib;

        /// <summary>
        /// Implied volatility of the option
        /// </summary>
        public double ImpliedVolatility { get; init; } = ImpliedVolatility;

        /// <summary>
        /// Delta of the option
        /// </summary>
        public double Delta { get; init; } = Delta;

        /// <summary>
        /// Price of the option
        /// </summary>
        public double OptionPrice { get; init; } = OptPrice;

        /// <summary>
        /// Present value of the dividend
        /// </summary>
        public double PresentValueDividend { get; init; } = PvDividend;

        /// <summary>
        /// Gamma of the option
        /// </summary>
        public double Gamma { get; init; } = Gamma;

        /// <summary>
        /// Vega of the option
        /// </summary>
        public double Vega { get; init; } = Vega;

        /// <summary>
        /// Theta of the option
        /// </summary>
        public double Theta { get; init; } = Theta;

        /// <summary>
        /// Price of the underlying
        /// </summary>
        public double UnderlyingPrice { get; init; } = UndPrice;
    }
}
