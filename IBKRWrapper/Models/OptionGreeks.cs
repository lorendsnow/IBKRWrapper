using IBKRWrapper.Constants;

namespace IBKRWrapper.Models
{
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
        public int ReqId { get; init; } = ReqId;
        public StandardTickIds Field { get; init; } = (StandardTickIds)FieldCode;
        public OptionTickAttribute TickAttribute { get; init; } = (OptionTickAttribute)TickAttrib;
        public double ImpliedVolatility { get; init; } = ImpliedVolatility;
        public double Delta { get; init; } = Delta;
        public double OptionPrice { get; init; } = OptPrice;
        public double PresentValueDividend { get; init; } = PvDividend;
        public double Gamma { get; init; } = Gamma;
        public double Vega { get; init; } = Vega;
        public double Theta { get; init; } = Theta;
        public double UnderlyingPrice { get; init; } = UndPrice;
    }
}
