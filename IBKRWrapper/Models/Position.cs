using IBApi;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents a position in an account
    /// </summary>
    public record Position
    {
        /// <summary>
        /// Constructor to allow events to easily create positions.
        /// </summary>
        /// <param name="account">The position's account number</param>
        /// <param name="contract">The underlying IBKR contract</param>
        /// <param name="pos">The number of positions held</param>
        /// <param name="avgCost">Total average cost of all trades for the position</param>
        public Position(string account, Contract contract, decimal pos, double avgCost)
        {
            ConId = contract.ConId;
            Symbol = contract.Symbol;
            SecType = contract.SecType;
            Exchange = contract.Exchange;
            Currency = contract.Currency;
            PrimaryExch = contract.PrimaryExch;
            Positions = pos;
            AvgCost = avgCost;
            Account = account;
        }

        /// <summary>
        /// The contract ID for the underlying IBKR <see cref="IBApi.Contract"/>
        /// </summary>
        public int ConId { get; init; }

        /// <summary>
        /// The position's symbol.
        /// </summary>
        public string Symbol { get; init; }

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
        public string SecType { get; init; }

        /// <summary>
        /// The position's exchange
        /// </summary>
        public string Exchange { get; init; }

        /// <summary>
        /// The position's currency
        /// </summary>
        public string Currency { get; init; }

        /// <summary>
        /// The primary exchange for the position
        /// </summary>
        public string PrimaryExch { get; init; }

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
        public string Account { get; init; }
    }
}
