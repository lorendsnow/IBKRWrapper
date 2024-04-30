using IBKRWrapper.Constants;

namespace IBKRWrapper.Models
{
    public record OptionGreeks(
        int reqId,
        int field,
        int tickAttrib,
        double impliedVolatility,
        double delta,
        double optPrice,
        double pvDividend,
        double gamma,
        double vega,
        double theta,
        double undPrice
    )
    {
        public int ReqId { get; init; } = reqId;
        public OptionTickIds Field { get; init; } = (OptionTickIds)field;
        public OptionTickAttribute TickAttribute { get; init; } = (OptionTickAttribute)tickAttrib;
        public double ImpliedVolatility { get; init; } = impliedVolatility;
        public double Delta { get; init; } = delta;
        public double OptionPrice { get; init; } = optPrice;
        public double PresentValueDividend { get; init; } = pvDividend;
        public double Gamma { get; init; } = gamma;
        public double Vega { get; init; } = vega;
        public double Theta { get; init; } = theta;
        public double UnderlyingPrice { get; init; } = undPrice;
    }
}
