using IBApi;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a position in a an account.
    /// </summary>
    public record PortfolioPosition(
        Contract Contract,
        decimal Position,
        double MarketPrice,
        double MarketValue,
        double AverageCost,
        double UnrealizedPNL,
        double RealizedPNL,
        string AccountName
    )
    {
        /// <summary>
        /// The position's ticker symbol.
        /// </summary>
        public string Ticker { get; init; } = Contract.Symbol;

        /// <summary>
        /// The position's security type.
        /// </summary>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader><description>Definitions:</description></listheader>
        ///         <item>
        ///             <term>STK</term><description> Stock or ETF</description>
        ///         </item>
        ///         <item>
        ///             <term>OPT</term><description> Option</description>
        ///         </item>
        ///         <item>
        ///             <term>FUT</term><description> Future</description>
        ///         </item>
        ///         <item>
        ///             <term>IND</term><description> Index</description>
        ///         </item>
        ///         <item>
        ///             <term>FOP</term><description> Futures Option</description>
        ///         </item>
        ///         <item>
        ///             <term>CASH</term><description> Forex Pair</description>
        ///         </item>
        ///         <item>
        ///             <term>BAG</term><description> Combo</description>
        ///         </item>
        ///         <item>
        ///             <term>WAR</term><description> Warrant</description>
        ///         </item>
        ///         <item>
        ///             <term>BOND</term><description> Bond</description>
        ///         </item>
        ///         <item>
        ///             <term>CMDTY</term><description> Commodity</description>
        ///         </item>
        ///         <item>
        ///             <term>NEWS</term><description> News</description>
        ///         </item>
        ///         <item>
        ///             <term>FUND</term><description> Mutual Fund</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public string SecType { get; init; } = Contract.SecType;

        /// <summary>
        /// The destination exchange on the underlying IBKR <see cref="IBApi.Contract"/>
        /// </summary>
        public string Exchange { get; init; } = Contract.Exchange;

        /// <summary>
        /// The primary exchange on the underlying IBKR <see cref="IBApi.Contract"/>
        /// </summary>
        public string PrimaryExch { get; init; } = Contract.PrimaryExch;

        /// <summary>
        /// The number of shares/contracts/etc. in the position.
        /// </summary>
        public decimal Quantity { get; init; } = Position;

        /// <summary>
        /// The position's market price.
        /// </summary>
        public double MarketPrice { get; init; } = MarketPrice;

        /// <summary>
        /// The position's market value.
        /// </summary>
        public double MarketValue { get; init; } = MarketValue;

        /// <summary>
        /// The position's average cost.
        /// </summary>
        public double AverageCost { get; init; } = AverageCost;

        /// <summary>
        /// The position's unrealized profit and loss.
        /// </summary>
        public double UnrealizedPNL { get; init; } = UnrealizedPNL;

        /// <summary>
        /// The position's realized profit and loss.
        /// </summary>
        public double RealizedPNL { get; init; } = RealizedPNL;

        /// <summary>
        /// The account name associated with the position.
        /// </summary>
        public string AccountName { get; init; } = AccountName;
    }
}
