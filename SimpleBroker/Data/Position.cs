namespace SimpleBroker
{
    /// <summary>
    /// Represents a position in an account
    /// </summary>
    public record Position
    {
        /// <summary>
        /// The contract ID for the underlying IBKR <see cref="Contract"/>
        /// </summary>
        public int ConId { get; init; }

        /// <summary>
        /// The position's symbol.
        /// </summary>
        public required string Symbol { get; init; }

        /// <summary>
        /// The position's security type
        /// </summary>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader><description>Security Type Definitions:</description></listheader>
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
        public required string SecType { get; init; }

        /// <summary>
        /// The position's exchange
        /// </summary>
        public required string Exchange { get; init; }

        /// <summary>
        /// The position's currency
        /// </summary>
        public required string Currency { get; init; }

        /// <summary>
        /// The primary exchange for the position
        /// </summary>
        public required string PrimaryExch { get; init; }

        /// <summary>
        /// The quantity of positions held (e.g., shares, contracts, etc.)
        /// </summary>
        public decimal Positions { get; init; }

        /// <summary>
        /// The average cost of all trades for the position
        /// </summary>
        public double AvgCost { get; init; }

        /// <summary>
        /// The account number for the position
        /// </summary>
        public required string Account { get; init; }
    }
}
