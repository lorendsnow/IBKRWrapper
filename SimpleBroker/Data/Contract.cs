namespace SimpleBroker
{
    /// <summary>
    /// An object representing an security's contract
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="secType"></param>
    /// <param name="currency"></param>
    /// <param name="exchange"></param>
    public class Contract(string symbol, string secType, string currency, string exchange)
    {
        /// <summary>
        /// The contract's unique identifier
        /// </summary>
        public int ConId { get; set; }

        /// <summary>
        /// The instrument's symbol
        /// </summary>
        public string Symbol { get; set; } = symbol;

        /// <summary>
        /// The security's type. Possible types include:
        ///     <list type="table">
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
        /// </summary>
        public string SecType { get; set; } = secType;

        /// <summary>
        /// The contract's last trading day or contract month (for Options and Futures). Strings with format YYYYMM will be interpreted as the Contract Month whereas YYYYMMDD will be interpreted as Last Trading Day.
        /// </summary>
        public string? LastTradeDateOrContractMonth { get; set; }

        /// <summary>
        /// The contract's last trading day
        /// </summary>
        public string? LastTradeDate { get; set; }

        /// <summary>
        /// The contract's strike price
        /// </summary>
        public double Strike { get; set; }

        /// <summary>
        /// The option's right.Valid values are P, PUT, C, CALL.
        /// </summary>
        public string? Right { get; set; }

        /// <summary>
        /// The instrument's multiplier (i.e. options, futures)
        /// </summary>
        public string? Multiplier { get; set; }

        /// <summary>
        /// The destination exchange
        /// </summary>
        public string Exchange { get; set; } = exchange;

        /// <summary>
        /// The underlying's currency
        /// </summary>
        public string Currency { get; set; } = currency;

        /// <summary>
        /// The contract's symbol within its primary exchange. For options, this will be the OCC symbol.
        /// </summary>
        public string? LocalSymbol { get; set; }

        /// <summary>
        /// The contract's primary exchange.
        /// </summary>
        public string? PrimaryExch { get; set; }

        /// <summary>
        /// The trading class name for this contract.
        /// </summary>
        public string? TradingClass { get; set; }

        /// <summary>
        /// If set to true, contract details requests and historical data queries can be performed pertaining to expired futures contracts. Expired options or other instrument types are not available.
        /// </summary>
        public bool IncludeExpired { get; set; }

        /// <summary>
        /// Security's identifier when querying contract's details or placing orders
        /// </summary>
        public string? SecIdType { get; set; }

        /// <summary>
        /// Identifier of the security type
        /// </summary>
        public string? SecId { get; set; }

        /// <summary>
        /// Description of the contract
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// IssuerId of the contract
        /// </summary>
        public string? IssuerId { get; set; }

        /// <summary>
        /// Description of the combo legs
        /// </summary>
        public string? ComboLegsDescription { get; set; }

        /// <summary>
        /// The legs of a combined contract definition
        /// </summary>
        public List<ComboLeg>? ComboLegs { get; set; }

        /// <summary>
        /// Delta and underlying price for Delta-Neutral combo orders. Underlying (STK or FUT), delta and underlying price goes into this attribute.
        /// </summary>
        public DeltaNeutralContract? DeltaNeutralContract { get; set; }

        /// <summary>
        /// String representation of the contract
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString() => $"{SecType} {Symbol} {Currency} {Exchange}";
    }
}
