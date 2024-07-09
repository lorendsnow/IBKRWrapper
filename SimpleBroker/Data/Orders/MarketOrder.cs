namespace SimpleBroker
{
    /// <summary>
    /// Market Order
    /// </summary>
    public class MarketOrder : OrderBase
    {
        /// <summary>
        /// Market Order constructor
        /// </summary>
        /// <param name="action">"BUY" or "SELL"</param>
        /// <param name="tif">
        ///     Time in force
        ///     <list type="table">
        ///         <listheader><description><u>Valid Options:</u>:</description></listheader>
        ///         <item>
        ///             <term>DAY</term>
        ///             <description>
        ///                 Valid for the day only
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>GTC</term>
        ///             <description>
        ///                 Good until canceled. The order will continue to work within the system
        ///                 and in the marketplace until it executes or is canceled. GTC orders
        ///                 will be automatically be cancelled under the following conditions:
        ///             <list type="bullet">
        ///                 <item>
        ///                     <description>
        ///                         If a corporate action on a security results in a stock split
        ///                         (forward or reverse), exchange for shares, or distribution of
        ///                         shares.
        ///                     </description>
        ///                 </item>
        ///                 <item>
        ///                     <description>
        ///                         If you do not log into your IB account for 90 days.
        ///                     </description>
        ///                 </item>
        ///                 <item>
        ///                     <description>
        ///                         At the end of the calendar quarter following the current
        ///                         quarter. For example, an order placed during the third quarter
        ///                         of 2011 will be canceled at the end of the first quarter of
        ///                         2012. If the last day is a non-trading day, the cancellation
        ///                         will occur at the close of the final trading day of that
        ///                         quarter. For example, if the last day of the quarter is
        ///                         Sunday, the orders will be cancelled on the preceding Friday.
        ///                     </description>
        ///                 </item>
        ///                 <item>
        ///                     <description>
        ///                         Orders that are modified will be assigned a new “Auto Expire”
        ///                         date consistent with the end of the calendar quarter following
        ///                         the current quarter.
        ///                     </description>
        ///                 </item>
        ///                 <item>
        ///                     <description>
        ///                         Orders submitted to IB that remain in force for more than one
        ///                         day will not be reduced for dividends. To allow adjustment to
        ///                         your order price on ex-dividend date, consider using a
        ///                         Good-Til-Date/Time (GTD) or Good-after-Time/Date (GAT) order
        ///                         type, or a combination of the two.
        ///                     </description>
        ///                 </item>
        ///             </list>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>IOC</term>
        ///             <description>
        ///                 Immediate or Cancel. Any portion that is not filled as soon as it
        ///                 becomes available in the market is canceled.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>GTD</term>
        ///             <description>
        ///                 Good until Date. It will remain working within the system and in the
        ///                 marketplace until it executes or until the close of the market on the
        ///                 date specified
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>OPG</term>
        ///             <description>
        ///                 Use OPG to send a market-on-open (MOO) or limit-on-open (LOO) order.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>FOK</term>
        ///             <description>
        ///                 If the entire Fill-or-Kill order does not execute as soon as it becomes
        ///                 available, the entire order is canceled.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>DTC</term><description> Day until canceled</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="account">The IBKR account for which the order is being placed</param>
        /// <param name="quantity">Order quantity (e.g., shares, contracts, etc.)</param>
        /// <param name="outsideRth">Whether or not to allow the order to be executed outside regular trading hours</param>
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Style",
            "IDE0290:Use primary constructor",
            Justification = "We need to use the SetsRequiredMembers attribute, and we can't use that on a primary constructor"
        )]
        public MarketOrder(
            string action,
            string tif,
            string account,
            int quantity,
            bool outsideRth = false
        )
            : base(action, tif, account, quantity, outsideRth, "MKT") { }
    }
}
