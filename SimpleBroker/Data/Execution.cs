namespace SimpleBroker
{
    /// <summary>
    /// Record describing an order's execution.
    /// </summary>
    public record Execution
    {
        /// <summary>
        /// he API client's order Id. May not be unique to an account.
        /// </summary>
        public int OrderId { get; init; }

        /// <summary>
        /// The API client identifier which placed the order which originated this execution.
        /// </summary>
        public int ClientId { get; init; }

        /// <summary>
        /// The execution's identifier. Each partial fill has a separate ExecId. A correction is
        /// indicated by an ExecId which differs from a previous ExecId in only the digits after
        /// the final period, e.g. an ExecId ending in ".02" would be a correction of a previous
        /// execution with an ExecId ending in ".01"
        /// </summary>
        public required string ExecId { get; init; }

        /// <summary>
        /// The execution's server time.
        /// </summary>
        public required string Time { get; init; }

        /// <summary>
        /// The account to which the order was allocated.
        /// </summary>
        public required string AcctNumber { get; init; }

        /// <summary>
        /// The exchange where the execution took place.
        /// </summary>
        public required string Exchange { get; init; }

        /// <summary>
        /// Specifies if the transaction was buy or sale. BOT for bought, SLD for sold
        /// </summary>
        public required string Side { get; init; }

        /// <summary>
        /// The number of shares filled.
        /// </summary>
        public decimal Shares { get; init; }

        /// <summary>
        /// The order's execution price excluding commissions.
        /// </summary>
        public double Price { get; init; }

        /// <summary>
        /// The TWS order identifier. The PermId can be 0 for trades originating outside IB.
        /// </summary>
        public int PermId { get; init; }

        /// <summary>
        /// Identifies whether an execution occurred because of an IB-initiated liquidation.
        /// </summary>
        public int Liquidation { get; init; }

        /// <summary>
        /// Cumulative quantity. Used in regular trades, combo trades and legs of the combo.
        /// </summary>
        public decimal CumQty { get; init; }

        /// <summary>
        /// Average price. Used in regular trades, combo trades and legs of the combo. Does not
        /// include commissions.
        /// </summary>
        public double AvgPrice { get; init; }

        /// <summary>
        /// The OrderRef is a user-customizable string that can be set from the API or TWS and
        /// will be associated with an order for its lifetime.
        /// </summary>
        public string? OrderRef { get; init; }

        /// <summary>
        /// The Economic Value Rule name and the respective optional argument. The two values
        /// should be separated by a colon. For example, aussieBond:YearsToExpiration=3. When
        /// the optional argument is not present, the first value will be followed by a colon.
        /// </summary>
        public string? EvRule { get; init; }

        /// <summary>
        /// Tells you approximately how much the market value of a contract would change if the
        /// price were to change by 1. It cannot be used to get market value by multiplying the
        /// price by the approximate multiplier.
        /// </summary>
        public double EvMultiplier { get; init; }

        /// <summary>
        /// Model code
        /// </summary>
        public string? ModelCode { get; init; }

        /// <summary>
        /// The liquidity type of the execution. Requires TWS 968+ and API v973.05+.
        /// </summary>
        public Liquidity? LastLiquidity { get; init; }

        /// <summary>
        /// Pending price revision
        /// </summary>
        public bool PendingPriceRevision { get; init; }
    }

    /// <summary>
    /// Class describing the liquidity type of an execution.
    /// </summary>
    /// <remarks>
    /// Liquidity type constructor.
    /// </remarks>
    /// <param name="p"></param>
    public class Liquidity(int p)
    {
        /// <summary>
        /// The enum of available liquidity flag types.
        /// <para>0 = Unknown</para>
        /// <para>1 = Added liquidity</para>
        /// <para>2 = Removed liquidity</para>
        /// <para>3 = Liquidity routed out</para>
        /// </summary>
        private static readonly Dictionary<int, string> Values =
            new()
            {
                { 0, "None" },
                { 1, "Added Liquidity" },
                { 2, "Removed Liquidity" },
                { 3, "Liquidity Routed Out" }
            };

        /// <summary>
        /// The value of the liquidity type.
        /// </summary>
        public int Value { get; set; } = Values.ContainsKey(p) ? p : 0;

        /// <summary>
        /// String representation of the liquidity type.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString() => Values[Value];
    }
}
