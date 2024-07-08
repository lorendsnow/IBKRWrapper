using IBApi;

namespace SimpleBroker
{
    /// <summary>
    /// Base order object; mirrors IBKR's Order object
    /// </summary>
    public class OrderBase
    {
        internal static int CUSTOMER = 0;
        internal static int FIRM = 1;
        internal static char OPT_UNKNOWN = '?';
        internal static char OPT_BROKER_DEALER = 'b';
        internal static char OPT_CUSTOMER = 'c';
        internal static char OPT_FIRM = 'f';
        internal static char OPT_ISEMM = 'm';
        internal static char OPT_FARMM = 'n';
        internal static char OPT_SPECIALIST = 'y';
        internal static int AUCTION_MATCH = 1;
        internal static int AUCTION_IMPROVEMENT = 2;
        internal static int AUCTION_TRANSPARENT = 3;
        internal static string EMPTY_STR = "";
        internal static double COMPETE_AGAINST_BEST_OFFSET_UP_TO_MID = double.PositiveInfinity;

        /// <summary>
        /// The API client's order id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// The Solicited field should be used for orders initiated or recommended by the broker
        /// or adviser that were approved by the client (by phone, email, chat, verbally, etc.)
        /// prior to entry. Please note that orders that the adviser or broker placed without
        /// specifically discussing with the client are discretionary orders, not solicited.
        /// </summary>
        public bool Solicited { get; set; }

        /// <summary>
        /// The API client id which placed the order
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// The Host order identifier
        /// </summary>
        public int PermId { get; set; }

        /// <summary>
        /// Identifies the side. Available values are "BUY" or "SELL"
        /// </summary>
        public required string Action { get; set; }

        /// <summary>
        /// The number of positions being bought/sold
        /// </summary>
        public decimal TotalQuantity { get; set; }

        /// <summary>
        /// The order's type
        /// </summary>
        public required string OrderType { get; set; }

        /// <summary>
        /// The order's limit price. Used for limit, stop-limit and relative orders. In all other
        /// cases specify zero. For relative orders with no limit price, also specify zero.
        /// </summary>
        public double LmtPrice { get; set; }

        /// <summary>
        /// Generic field to contain the stop price for <b>STP LMT</b> orders, trailing amount, etc.
        /// </summary>
        public double AuxPrice { get; set; }

        /**
         * @brief The time in force.\n
         * Valid values are: \n
         *      <b>DAY</b> - Valid for the day only.\n
         *      <b>GTC</b> - Good until canceled. The order will continue to work within the system and in the marketplace until it executes or is canceled. GTC orders will be automatically be cancelled under the following conditions: \n
         *          \t\t If a corporate action on a security results in a stock split (forward or reverse), exchange for shares, or distribution of shares.
         *          \t\t If you do not log into your IB account for 90 days. \n
         *          \t\t At the end of the calendar quarter following the current quarter. For example, an order placed during the third quarter of 2011 will be canceled at the end of the first quarter of 2012. If the last day is a non-trading day, the cancellation will occur at the close of the final trading day of that quarter. For example, if the last day of the quarter is Sunday, the orders will be cancelled on the preceding Friday.\n
         *          \t\t Orders that are modified will be assigned a new “Auto Expire” date consistent with the end of the calendar quarter following the current quarter.\n
         *          \t\t Orders submitted to IB that remain in force for more than one day will not be reduced for dividends. To allow adjustment to your order price on ex-dividend date, consider using a Good-Til-Date/Time (GTD) or Good-after-Time/Date (GAT) order type, or a combination of the two.\n
         *      <b>IOC</b> - Immediate or Cancel. Any portion that is not filled as soon as it becomes available in the market is canceled.\n
         *      <b>GTD</b> - Good until Date. It will remain working within the system and in the marketplace until it executes or until the close of the market on the date specified\n
         *      <b>OPG</b> - Use OPG to send a market-on-open (MOO) or limit-on-open (LOO) order.\n
         *      <b>FOK</b> - If the entire Fill-or-Kill order does not execute as soon as it becomes available, the entire order is canceled.\n
         *      <b>DTC</b> - Day until Canceled.
         */

        /// <summary>
        /// <para>Time in force</para>
        /// <list type="table">
        ///     <listheader><description><u>Valid Options:</u>:</description></listheader>
        ///     <item>
        ///         <term>DAY</term>
        ///         <description>
        ///             Valid for the day only
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>GTC</term>
        ///         <description>
        ///             Good until canceled. The order will continue to work within the system and
        ///             in the marketplace until it executes or is canceled. GTC orders will be
        ///             automatically be cancelled under the following conditions:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     If a corporate action on a security results in a stock split
        ///                     (forward or reverse), exchange for shares, or distribution of
        ///                     shares.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     If you do not log into your IB account for 90 days.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     At the end of the calendar quarter following the current quarter.
        ///                     For example, an order placed during the third quarter of 2011
        ///                     will be canceled at the end of the first quarter of 2012. If the
        ///                     last day is a non-trading day, the cancellation will occur at the
        ///                     close of the final trading day of that quarter. For example, if
        ///                     the last day of the quarter is Sunday, the orders will be
        ///                     cancelled on the preceding Friday.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     Orders that are modified will be assigned a new “Auto Expire” date
        ///                     consistent with the end of the calendar quarter following the
        ///                     current quarter.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     Orders submitted to IB that remain in force for more than one day
        ///                     will not be reduced for dividends. To allow adjustment to your
        ///                     order price on ex-dividend date, consider using a
        ///                     Good-Til-Date/Time (GTD) or Good-after-Time/Date (GAT) order type,
        ///                     or a combination of the two.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>IOC</term>
        ///         <description>
        ///             Immediate or Cancel. Any portion that is not filled as soon as it becomes
        ///             available in the market is canceled.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>GTD</term>
        ///         <description>
        ///             Good until Date. It will remain working within the system and in the
        ///             marketplace until it executes or until the close of the market on the date
        ///             specified
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>OPG</term>
        ///         <description>
        ///             Use OPG to send a market-on-open (MOO) or limit-on-open (LOO) order.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>FOK</term>
        ///         <description>
        ///             If the entire Fill-or-Kill order does not execute as soon as it becomes
        ///             available, the entire order is canceled.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>DTC</term><description> Day until canceled</description>
        ///     </item>
        /// </list>
        /// </summary>
        public required string Tif { get; set; }

        /// <summary>
        /// One-Cancels-All group identifier.
        /// </summary>
        public string? OcaGroup { get; set; }

        /// <summary>
        /// Tells how to handle remaining orders in an OCA group when one order or part of an
        /// order executes.
        /// <list type="table">
        ///     <listheader><description><u>Valid Options:</u>:</description></listheader>
        ///     <item>
        ///         <term>1</term>
        ///         <description>
        ///             Cancel all remaining orders with block.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>
        ///             Remaining orders are proportionately reduced in size with block.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>
        ///             Remaining orders are proportionately reduced in size with no block.
        ///         </description>
        ///     </item>
        /// </list>
        /// If you use a value "with block" it gives the order overfill protection. This means
        /// that only one order in the group will be routed at a time to remove the possibility of
        /// an overfill.
        /// </summary>
        public int OcaType { get; set; }

        /// <summary>
        /// The order reference. Intended for institutional customers only, although all customers
        /// may use it to identify the API client that sent the order when multiple API clients
        /// are running.
        /// </summary>
        public string? OrderRef { get; set; }

        /// <summary>
        /// Specifies whether the order will be transmitted by TWS. If set to false, the order
        /// will be created at TWS but will not be sent.
        /// </summary>
        public bool Transmit { get; set; }

        /// <summary>
        /// The order ID of the parent order, used for bracket and auto trailing stop orders.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// If set to true, specifies that the order is an ISE Block order.
        /// </summary>
        public bool BlockOrder { get; set; }

        /// <summary>
        /// If set to true, specifies that the order is a Sweep-to-Fill order.
        /// </summary>
        public bool SweepToFill { get; set; }

        /// <summary>
        /// The publicly disclosed order size, used when placing Iceberg orders.
        /// </summary>
        public int DisplaySize { get; set; }

        /// <summary>
        /// Specifies how Simulated Stop, Stop-Limit and Trailing Stop orders are triggered.
        /// <list type="table">
        ///     <listheader><description><u>Valid Options:</u>:</description></listheader>
        ///     <item>
        ///         <term>0</term>
        ///         <description>
        ///             The default value. The "double bid/ask" function will be used for orders
        ///             for OTC stocks and US options. All other orders will used the "last"
        ///             function.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>1</term>
        ///         <description>
        ///             use "double bid/ask" function, where stop orders are triggered based on
        ///             two consecutive bid or ask prices.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>
        ///             "last" function, where stop orders are triggered based on the last price.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>
        ///             double last function.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>4</term>
        ///         <description>
        ///             bid/ask function.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>7</term>
        ///         <description>
        ///             last or bid/ask function.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>8</term>
        ///         <description>
        ///             mid-point function.
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        public int TriggerMethod { get; set; }

        /// <summary>
        /// If set to true, allows orders to also trigger or fill outside of regular trading hours.
        /// </summary>
        public bool OutsideRth { get; set; }

        /// <summary>
        /// If set to true, the order will not be visible when viewing the market depth. This
        /// option only applies to orders routed to the NASDAQ exchange.
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Specifies the date and time after which the order will be active.
        /// Format: yyyymmdd hh:mm:ss {optional Timezone}
        /// </summary>
        public string? GoodAfterTime { get; set; }

        /// <summary>
        /// The date and time until the order will be active. You must enter GTD as the time in
        /// force to use this string. The trade's "Good Till Date," format "yyyyMMdd HH:mm:ss
        /// (optional time zone)" or UTC "yyyyMMdd-HH:mm:ss"
        /// </summary>
        public string? GoodTillDate { get; set; }

        /// <summary>
        /// Overrides TWS constraints. Precautionary constraints are defined on the TWS Presets
        /// page, and help ensure tha tyour price and size order values are reasonable. Orders
        /// sent from the API are also validated against these safety constraints, and may be
        /// rejected if any constraint is violated. To override validation, set this parameter’s
        /// value to True.
        /// </summary>
        public bool OverridePercentageConstraints { get; set; }

        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Individual = 'I'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Agency = 'A'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             AgentOtherMember = 'W'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             IndividualPTIA = 'J'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             AgencyPTIA = 'U'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             AgentOtherMemberPTIA = 'M'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             IndividualPT = 'K'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             AgencyPT = 'Y'
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             AgentOtherMemberPT = 'N'
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        public string? Rule80A { get; set; }

        /// <summary>
        /// Indicates whether or not all the order has to be filled on a single execution.
        /// </summary>
        public bool AllOrNone { get; set; }

        /// <summary>
        /// Identifies a minimum quantity order type.
        /// </summary>
        public int MinQty { get; set; }

        /// <summary>
        /// The percent offset amount for relative orders.
        /// </summary>
        public double PercentOffset { get; set; }

        /// <summary>
        /// Trail stop price for TRAIL LIMIT orders.
        /// </summary>
        public double TrailStopPrice { get; set; }

        /// <summary>
        /// Specifies the trailing amount of a trailing stop order as a percentage.
        /// <para>
        ///     Observe the following guidelines when using the trailingPercent field:
        /// </para>
        /// <para>
        ///     - This field is mutually exclusive with the existing trailing amount. That is, the
        ///     API client can send one or the other but not both.
        /// </para>
        /// <para>
        ///     - This field is read AFTER the stop price (barrier price) as follows:
        ///     deltaNeutralAuxPrice stopPrice, trailingPercent, scale order attributes.
        /// </para>
        /// <para>
        ///     - The field will also be sent to the API in the openOrder message if the API
        ///     client version is >= 56. It is sent after the stopPrice field as follows:
        ///     stopPrice, trailingPct, basisPoint.
        /// </para>
        /// </summary>
        public double TrailingPercent { get; set; }

        /// <summary>
        /// The Financial Advisor group the trade will be allocated to.
        /// <i>Use an empty string if not applicable.</i>
        /// </summary>
        public string? FaGroup { get; set; }

        /// <summary>
        /// The Financial Advisor allocation method the trade will be allocated to.
        /// <i>Use an empty string if not applicable.</i>
        /// </summary>
        public string? FaMethod { get; set; }

        /// <summary>
        /// The Financial Advisor percentage concerning the trade's allocation.
        /// <i>Use an empty string if not applicable.</i>
        /// </summary>
        public string? FaPercentage { get; set; }

        /// <summary>
        /// For institutional customers only.
        /// </summary>
        public string OpenClose { get; set; }

        /**
         * @brief The order's origin. Same as TWS "Origin" column. Identifies the type of customer from which the order originated. \n
         * Valid values are: \n
         * <b>0</b> - Customer \n
         * <b>1</b> - Firm
         */

        /// <summary>
        /// The order's origin. Same as TWS "Origin" column. Identifies the type of customer from
        /// which the order originated.
        /// <para>Valid values are:</para>
        /// <para>0 - Customer</para>
        /// <para>1 - Firm</para>
        /// </summary>
        public int Origin { get; set; }

        /// <summary>
        /// For institutions only.
        /// </summary>
        public int ShortSaleSlot { get; set; }

        /// <summary>
        /// For institutions only.
        /// </summary>
        public string DesignatedLocation { get; set; }

        /// <summary>
        /// Only available with IB Execution-Only accounts with applicable securities. Mark order
        /// as exempt from short sale uptick rule
        /// </summary>
        public int ExemptCode { get; set; }

        /// <summary>
        /// The amount off the limit price allowed for discretionary orders.
        /// </summary>
        public double DiscretionaryAmt { get; set; }

        /// <summary>
        /// Use to opt out of default SmartRouting for orders routed directly to ASX. This
        /// attribute defaults to false unless explicitly set to true. When set to false, orders
        /// routed directly to ASX will NOT use SmartRouting. When set to true, orders routed
        /// directly to ASX orders WILL use SmartRouting.
        /// </summary>
        public bool OptOutSmartRouting { get; set; }

        /// <summary>
        /// For BOX orders only.
        /// <para>Valid values are:</para>
        /// <para>1 - Match</para>
        /// <para>2 - Improvement</para>
        /// <para>3 - Transparent</para>
        /// </summary>
        public int AuctionStrategy { get; set; }

        /// <summary>
        /// The auction's starting price. <i>For BOX orders only.</i>
        /// </summary>
        public double StartingPrice { get; set; }

        /// <summary>
        /// The stock's reference price. <i>The reference price is used for VOL orders to compute
        /// the limit price sent to an exchange (whether or not Continuous Update is selected),
        /// and for price range monitoring.</i>
        /// </summary>
        public double StockRefPrice { get; set; }

        /// <summary>
        /// The stock's Delta. <i>For orders on BOX only.</i>
        /// </summary>
        public double Delta { get; set; }

        /// <summary>
        /// The lower value for the acceptable underlying stock price range. <i>For price
        /// improvement option orders on BOX and VOL orders with dynamic management.</i>
        /// </summary>
        public double StockRangeLower { get; set; }

        /// <summary>
        /// The upper value for the acceptable underlying stock price range. <i>For price
        /// improvement option orders on BOX and VOL orders with dynamic management.</i>
        /// </summary>
        public double StockRangeUpper { get; set; }

        /// <summary>
        /// The option price in volatility, as calculated by TWS' Option Analytics. This value is
        /// expressed as a percent and is used to calculate the limit price sent to the exchange.
        /// </summary>
        public double Volatility { get; set; }

        /// <summary>
        /// Values include:
        /// <para>1 - Daily Volatility</para>
        /// <para>2 - Annual Volatility</para>
        /// </summary>
        public int VolatilityType { get; set; }

        /// <summary>
        /// Specifies whether TWS will automatically update the limit price of the order as the
        /// underlying price moves. <i>VOL orders only.</i>
        /// </summary>
        public int ContinuousUpdate { get; set; }

        /// <summary>
        /// Specifies how you want TWS to calculate the limit price for options, and for stock
        /// range price monitoring. <i>VOL orders only. </i>
        /// <para>Valid values include:</para>
        /// <para>1 - Average of NBBO</para>
        /// <para>2 - NBB or the NBO depending on the action and right.</para>
        /// </summary>
        public int ReferencePriceType { get; set; }

        /// <summary>
        /// Enter an order type to instruct TWS to submit a delta neutral trade on full or partial
        /// execution of the VOL order. <i>VOL orders only. For no hedge delta order to be sent,
        /// specify NONE.</i>
        /// </summary>
        public string DeltaNeutralOrderType { get; set; }

        /// <summary>
        /// Use this field to enter a value if the value in the deltaNeutralOrderType field is an
        /// order type that requires an Aux price, such as a REL order. <i>VOL orders only.</i>
        /// </summary>
        public double DeltaNeutralAuxPrice { get; set; }

        /// <summary>
        /// The unique contract identifier specifying the security in Delta Neutral order.
        /// </summary>
        public int DeltaNeutralConId { get; set; }

        /// <summary>
        /// Indicates the firm which will settle the Delta Neutral trade. <i>Institutions only.</i>
        /// </summary>
        public string DeltaNeutralSettlingFirm { get; set; }

        /// <summary>
        /// Specifies the beneficiary of the Delta Neutral order.
        /// </summary>
        public string DeltaNeutralClearingAccount { get; set; }

        /// <summary>
        /// Specifies where the clients want their shares to be cleared at. <i>Must be specified
        /// by execution-only clients.</i>
        /// <para>Valied values are: IB, Away, and PTA (post trade allocation)</para>
        /// </summary>
        public string DeltaNeutralClearingIntent { get; set; }

        /// <summary>
        /// Specifies whether the order is an Open or a Close order and is used when the hedge
        /// involves a CFD and and the order is clearing away.
        /// </summary>
        public string DeltaNeutralOpenClose { get; set; }

        /// <summary>
        /// Used when the hedge involves a stock and indicates whether or not it is sold short.
        /// </summary>
        public bool DeltaNeutralShortSale { get; set; }

        /// <summary>
        /// Indicates a short sale Delta Neutral order. Has a value of 1 (the clearing broker
        /// holds shares) or 2 (delivered from a third party). If you use 2, then you must specify
        /// a deltaNeutralDesignatedLocation.
        /// </summary>
        public int DeltaNeutralShortSaleSlot { get; set; }

        /// <summary>
        /// Identifies third party order origin. Used only when deltaNeutralShortSaleSlot = 2.
        /// </summary>
        public string DeltaNeutralDesignatedLocation { get; set; }

        /// <summary>
        /// Specifies Basis Points for EFP order. The values increment in 0.01% = 1 basis point.
        /// <i>For EFP orders only.</i>
        /// </summary>
        public double BasisPoints { get; set; }

        /// <summary>
        /// Specifies the increment of the Basis Points. <i>For EFP orders only.</i>
        /// </summary>
        public int BasisPointsType { get; set; }

        /// <summary>
        /// Defines the size of the first, or initial, order component.
        /// <i>For Scale orders only.</i>
        /// </summary>
        public int ScaleInitLevelSize { get; set; }

        /// <summary>
        /// Defines the order size of the subsequent scale order components. <i>For Scale orders
        /// only. Used in conjunction with scaleInitLevelSize().</i>
        /// </summary>
        public int ScaleSubsLevelSize { get; set; }

        /// <summary>
        /// Defines the price increment between scale components. <i>For Scale orders only. This
        /// value is compulsory.</i>
        /// </summary>
        public double ScalePriceIncrement { get; set; }

        /// <summary>
        /// Modifies the value of the Scale order. <i>For extended Scale orders.</i>
        /// </summary>
        public double ScalePriceAdjustValue { get; set; }

        /// <summary>
        /// Specifies the interval when the price is adjusted. <i>For extended Scale orders.</i>
        /// </summary>
        public int ScalePriceAdjustInterval { get; set; }

        /// <summary>
        /// Specifies the offset when to adjust profit. <i>For extended scale orders.</i>
        /// </summary>
        public double ScaleProfitOffset { get; set; }

        /// <summary>
        /// Restarts the Scale series if the order is cancelled. <i>For extended scale orders.</i>
        /// </summary>
        public bool ScaleAutoReset { get; set; }

        /// <summary>
        /// The initial position of the Scale order. <i>For extended scale orders.</i>
        /// </summary>
        public int ScaleInitPosition { get; set; }

        /// <summary>
        /// Specifies the initial quantity to be filled. <i>For extended scale orders.</i>
        /// </summary>
        public int ScaleInitFillQty { get; set; }

        /// <summary>
        /// Defines the random percent by which to adjust the position. <i>For extended scale
        /// orders.</i>
        /// </summary>
        public bool ScaleRandomPercent { get; set; }

        /// <summary>
        /// For hedge orders.
        /// <para>Possible values include:</para>
        /// <para>D - Delta</para>
        /// <para>B - Beta</para>
        /// <para>F - FX</para>
        /// <para>P - Pair</para>
        /// </summary>
        public string? HedgeType { get; set; }

        /// <summary>
        /// For hedge orders. Beta = x for Beta hedge orders, ratio = y for Pair hedge order
        /// </summary>
        public string? HedgeParam { get; set; }

        /// <summary>
        /// The account the trade will be allocated to.
        /// </summary>
        public required string Account { get; set; }

        /// <summary>
        /// Indicates the firm which will settle the trade. <i>Institutions only.</i>
        /// </summary>
        public string? SettlingFirm { get; set; }

        /// <summary>
        /// Specifies the true beneficiary of the order. For IBExecution customers. This value is
        /// required for FUT/FOP orders for reporting to the exchange.
        /// </summary>
        public string? ClearingAccount { get; set; }

        /// <summary>
        /// For execution-only clients to know where do they want their shares to be cleared at.
        /// <para>Valid values are IB, Away, and PTA (post trade allocation)</para>
        /// </summary>
        public string? ClearingIntent { get; set; }

        /**
         * @brief The algorithm strategy.\n
         * As of API verion 9.6, the following algorithms are supported:\n
         *      <b>ArrivalPx</b> - Arrival Price \n
         *      <b>DarkIce</b> - Dark Ice \n
         *      <b>PctVol</b> - Percentage of Volume \n
         *      <b>Twap</b> - TWAP (Time Weighted Average Price) \n
         *      <b>Vwap</b> - VWAP (Volume Weighted Average Price) \n
         * <b>For more information about IB's API algorithms, refer to https://interactivebrokers.github.io/tws-api/ibalgos.html</b>
         */

        /// <summary>
        /// The algorithm strategy.
        /// <list type="table">
        ///     <listheader><description><u>Valid Options:</u>:</description></listheader>
        ///     <item>
        ///         <term>ArrivalPx</term>
        ///         <description>
        ///             Arrival Price.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>DarkIce</term>
        ///         <description>
        ///             Dark Ice.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>PctVol</term>
        ///         <description>
        ///             Percentage of Volume
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Twap</term>
        ///         <description>
        ///             TWAP (Time Weighted Average Price)
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>Vwap</term>
        ///         <description>
        ///             VWAP (Volume Weighted Average Price)
        ///         </description>
        ///     </item>
        /// </list>
        /// For more information about IB's API algorithms, refer to
        /// <see href="https://interactivebrokers.github.io/tws-api/ibalgos.html"/>
        /// </summary>
        public string? AlgoStrategy { get; set; }

        /// <summary>
        ///     The list of parameters for the IB algorithm. For more information about IB's API
        ///     algorithms, refer to
        ///     <see href="https://interactivebrokers.github.io/tws-api/ibalgos.html"/>
        /// </summary>
        public List<TagValue>? AlgoParams { get; set; }

        /// <summary>
        /// Allows to retrieve the commissions and margin information. When placing an order with
        /// this attribute set to true, the order will not be placed as such. Instead it will used
        /// to request the commissions and margin information that would result from this order.
        /// </summary>
        public bool WhatIf { get; set; }

        /// <summary>
        /// Identifies orders generated by algorithmic trading.
        /// </summary>
        public string? AlgoId { get; set; }

        /// <summary>
        /// Orders routed to IBDARK are tagged as “post only” and are held in IB's order book,
        /// where incoming SmartRouted orders from other IB customers are eligible to trade
        /// against them. For IBDARK orders only.
        /// </summary>
        public bool NotHeld { get; set; }

        /// <summary>
        /// Advanced parameters for Smart combo routing. These features are for both guaranteed
        /// and nonguaranteed combination orders routed to Smart, and are available based on combo
        /// type and order type. SmartComboRoutingParams is similar to AlgoParams in that it makes
        /// use of tag/value pairs to add parameters to combo orders. Make sure that you fully
        /// understand how Advanced Combo Routing works in TWS itself first:
        /// <see href=
        /// "https://www.interactivebrokers.com/en/software/tws/usersguidebook/specializedorderentry/advanced_combo_routing.htm"
        /// />
        /// <para>The parameters cover the following capabilities:</para>
        /// <para>
        ///     - Non-Guaranteed - Determine if the combo order is Guaranteed or Non-Guaranteed.
        /// </para>
        /// <para>Tag = NonGuaranteed</para>
        /// <para>Value = 0: The order is guaranteed</para>
        /// <para>Value = 1: The order is non-guaranteed</para>
        /// <para>
        ///     - Select Leg to Fill First - User can specify which leg to be executed first.
        /// </para>
        /// <para>Tag = LeginPrio</para>
        /// <para>Value = -1: No priority is assigned to either combo leg</para>
        /// <para>
        ///     Value = 0: Priority is assigned to the first leg being added to the comboLeg
        /// </para>
        /// <para>
        ///     Value = 1: Priority is assigned to the second leg being added to the comboLeg
        /// </para>
        /// <para>Note: The LeginPrio parameter can only be applied to two-legged combo.</para>
        /// <para>
        ///     - Maximum Leg-In Combo Size - Specify the maximum allowed leg-in size per segment
        /// </para>
        /// <para>Tag = MaxSegSize</para>
        /// <para>Value = Unit of combo size</para>
        /// <para>
        ///     - Do Not Start Next Leg-In if Previous Leg-In Did Not Finish - Specify whether or
        ///     not the system should attempt to fill the next segment before the current segment
        ///     fills.
        /// </para>
        /// <para>Tag = DontLeginNext</para>
        /// <para>Value = 0: Start next leg-in even if previous leg-in did not finish</para>
        /// <para>Value = 1: Do not start next leg-in if previous leg-in did not finish</para>
        /// <para>
        ///     - Price Condition - Combo order will be rejected or cancelled if the leg market
        ///     price is outside of the specified price range [CondPriceMin, CondPriceMax]
        /// </para>
        /// <para>
        ///     Tag = PriceCondConid: The ContractID of the combo leg to specify price condition on
        /// </para>
        /// <para>Value = The ContractID</para>
        /// <para>Tag = CondPriceMin: The lower price range of the price condition</para>
        /// <para>Value = The lower price</para>
        /// <para>Tag = CondPriceMax: The upper price range of the price condition</para>
        /// <para>Value = The upper price</para>
        /// </summary>
        public List<TagValue>? SmartComboRoutingParams { get; set; }

        /// <summary>
        /// List of Per-leg price following the same sequence combo legs are added. The combo
        /// price must be left unspecified when using per-leg prices.
        /// </summary>
        public List<OrderComboLeg> OrderComboLegs { get; set; } = [];

        /// <summary>
        /// <i>For internal IBKR use only. Use the default value XYZ.</i>
        /// </summary>
        public List<TagValue> OrderMiscOptions { get; set; } = [];

        /// <summary>
        /// Defines the start time of GTC orders.
        /// </summary>
        public string ActiveStartTime { get; set; }

        /// <summary>
        /// Defines the stop time of GTC orders.
        /// </summary>
        public string ActiveStopTime { get; set; }

        /// <summary>
        /// The list of scale orders. <i>Used for scale orders.</i>
        /// </summary>
        public string ScaleTable { get; set; }

        /// <summary>
        /// Is used to place an order to a model. For example, "Technology" model can be used for
        /// tech stocks first created in TWS.
        /// </summary>
        public string? ModelCode { get; set; }

        /// <summary>
        /// This is a regulartory attribute that applies to all US Commodity (Futures) Exchanges,
        /// provided to allow client to comply with CFTC Tag 50 Rules
        /// </summary>
        public string ExtOperator { get; set; }

        /// <summary>
        /// The native cash quantity.
        /// </summary>
        public double CashQty { get; set; }

        /// <summary>
        /// Identifies a person as the responsible party for investment decisions within the firm.
        /// Orders covered by MiFID 2 (Markets in Financial Instruments Directive 2) must include
        /// either Mifid2DecisionMaker or Mifid2DecisionAlgo field (but not both). <i>Requires TWS
        /// 969+.</i>
        /// </summary>
        public string Mifid2DecisionMaker { get; set; }

        /// <summary>
        /// Identifies the algorithm responsible for investment decisions within the firm. Orders
        /// covered under MiFID 2 must include either Mifid2DecisionMaker or Mifid2DecisionAlgo,
        /// but cannot have both. <i>Requires TWS 969+.</i>
        /// </summary>
        public string Mifid2DecisionAlgo { get; set; }

        /// <summary>
        /// For MiFID 2 reporting; identifies a person as the responsible party for the execution
        /// of a transaction within the firm. <i>Requires TWS 969+.</i>
        /// </summary>
        public string Mifid2ExecutionTrader { get; set; }

        /// <summary>
        /// For MiFID 2 reporting; identifies the algorithm responsible for the execution of a
        /// transaction within the firm. <i>Requires TWS 969+.</i>
        /// </summary>
        public string Mifid2ExecutionAlgo { get; set; }

        /// <summary>
        /// Don't use auto price for hedge
        /// </summary>
        public bool DontUseAutoPriceForHedge { get; set; }

        /// <summary>
        /// Specifies the date to auto cancel the order.
        /// </summary>
        public string AutoCancelDate { get; set; }

        /// <summary>
        /// Specifies the initial order quantity to be filled.
        /// </summary>
        public decimal FilledQuantity { get; set; }

        /// <summary>
        /// Identifies the reference future conId.
        /// </summary>
        public int RefFuturesConId { get; set; }

        /// <summary>
        /// Cancels the parent order if child order was cancelled.
        /// </summary>
        public bool AutoCancelParent { get; set; }

        /// <summary>
        /// Identifies the Shareholder.
        /// </summary>
        public string Shareholder { get; set; }

        /// <summary>
        /// Used to specify <i>"imbalance only open orders"</i> or
        /// <i>"imbalance only closing orders".</i>
        /// </summary>
        public bool ImbalanceOnly { get; set; }

        /// <summary>
        /// Routes market order to Best Bid Offer.
        /// </summary>
        public bool RouteMarketableToBbo { get; set; }

        /// <summary>
        /// Parent order Id.
        /// </summary>
        public long ParentPermId { get; set; }

        /// <summary>
        /// Accepts a list with parameters obtained from advancedOrderRejectJson.
        /// </summary>
        public string AdvancedErrorOverride { get; set; }

        /// <summary>
        /// Used by brokers and advisors when manually entering, modifying or cancelling orders at
        /// the direction of a client. <i>Only used when allocating orders to specific groups or
        /// accounts. Excluding "All" group.</i>
        /// </summary>
        public string ManualOrderTime { get; set; }

        /// <summary>
        /// Defines the minimum trade quantity to fill. <i>For IBKRATS orders.</i>
        /// </summary>
        public int MinTradeQty { get; set; }

        /// <summary>
        /// Defines the minimum size to compete. <i>For IBKRATS orders.</i>
        /// </summary>
        public int MinCompeteSize { get; set; }

        /// <summary>
        /// Specifies the offset Off The Midpoint that will be applied to the order.
        /// <i>For IBKRATS orders.</i>
        /// </summary>
        public double CompeteAgainstBestOffset { get; set; }

        /// <summary>
        /// This offset is applied when the spread is an even number of cents wide. This offset
        /// must be in whole-penny increments or zero. <i>For IBKRATS orders.</i>
        /// </summary>
        public double MidOffsetAtWhole { get; set; }

        /// <summary>
        /// This offset is applied when the spread is an odd number of cents wide. This offset
        /// must be in half-penny increments. <i>For IBKRATS orders.</i>
        /// </summary>
        public double MidOffsetAtHalf { get; set; }

        /// <summary>
        /// Customer account
        /// </summary>
        public string CustomerAccount { get; set; }

        /// <summary>
        /// Professional customer
        /// </summary>
        public bool ProfessionalCustomer { get; set; }

        /// <summary>
        /// Bond accrued interest
        /// </summary>
        public string BondAccruedInterest { get; set; }

        /// <summary>
        /// External User Id
        /// </summary>
        public string ExternalUserId { get; set; }

        /// <summary>
        /// Manual Order Indicator
        /// </summary>
        public int ManualOrderIndicator { get; set; }

        /// <summary>
        /// Constructor which defaults certain fields to their maximum values or empty strings.
        /// </summary>
        public OrderBase()
        {
            LmtPrice = double.MaxValue;
            AuxPrice = double.MaxValue;
            ActiveStartTime = EMPTY_STR;
            ActiveStopTime = EMPTY_STR;
            OutsideRth = false;
            OpenClose = EMPTY_STR;
            Origin = CUSTOMER;
            Transmit = true;
            DesignatedLocation = EMPTY_STR;
            ExemptCode = -1;
            MinQty = int.MaxValue;
            PercentOffset = double.MaxValue;
            OptOutSmartRouting = false;
            StartingPrice = double.MaxValue;
            StockRefPrice = double.MaxValue;
            Delta = double.MaxValue;
            StockRangeLower = double.MaxValue;
            StockRangeUpper = double.MaxValue;
            Volatility = double.MaxValue;
            VolatilityType = int.MaxValue;
            DeltaNeutralOrderType = EMPTY_STR;
            DeltaNeutralAuxPrice = double.MaxValue;
            DeltaNeutralConId = 0;
            DeltaNeutralSettlingFirm = EMPTY_STR;
            DeltaNeutralClearingAccount = EMPTY_STR;
            DeltaNeutralClearingIntent = EMPTY_STR;
            DeltaNeutralOpenClose = EMPTY_STR;
            DeltaNeutralShortSale = false;
            DeltaNeutralShortSaleSlot = 0;
            DeltaNeutralDesignatedLocation = EMPTY_STR;
            ReferencePriceType = int.MaxValue;
            TrailStopPrice = double.MaxValue;
            TrailingPercent = double.MaxValue;
            BasisPoints = double.MaxValue;
            BasisPointsType = int.MaxValue;
            ScaleInitLevelSize = int.MaxValue;
            ScaleSubsLevelSize = int.MaxValue;
            ScalePriceIncrement = double.MaxValue;
            ScalePriceAdjustValue = double.MaxValue;
            ScalePriceAdjustInterval = int.MaxValue;
            ScaleProfitOffset = double.MaxValue;
            ScaleAutoReset = false;
            ScaleInitPosition = int.MaxValue;
            ScaleInitFillQty = int.MaxValue;
            ScaleRandomPercent = false;
            ScaleTable = EMPTY_STR;
            WhatIf = false;
            NotHeld = false;
            Conditions = [];
            TriggerPrice = double.MaxValue;
            LmtPriceOffset = double.MaxValue;
            AdjustedStopPrice = double.MaxValue;
            AdjustedStopLimitPrice = double.MaxValue;
            AdjustedTrailingAmount = double.MaxValue;
            ExtOperator = EMPTY_STR;
            Tier = new SoftDollarTier(EMPTY_STR, EMPTY_STR, EMPTY_STR);
            CashQty = double.MaxValue;
            Mifid2DecisionMaker = EMPTY_STR;
            Mifid2DecisionAlgo = EMPTY_STR;
            Mifid2ExecutionTrader = EMPTY_STR;
            Mifid2ExecutionAlgo = EMPTY_STR;
            DontUseAutoPriceForHedge = false;
            AutoCancelDate = EMPTY_STR;
            FilledQuantity = decimal.MaxValue;
            RefFuturesConId = int.MaxValue;
            AutoCancelParent = false;
            Shareholder = EMPTY_STR;
            ImbalanceOnly = false;
            RouteMarketableToBbo = false;
            ParentPermId = long.MaxValue;
            UsePriceMgmtAlgo = null;
            Duration = int.MaxValue;
            PostToAts = int.MaxValue;
            AdvancedErrorOverride = EMPTY_STR;
            ManualOrderTime = EMPTY_STR;
            MinTradeQty = int.MaxValue;
            MinCompeteSize = int.MaxValue;
            CompeteAgainstBestOffset = double.MaxValue;
            MidOffsetAtWhole = double.MaxValue;
            MidOffsetAtHalf = double.MaxValue;
            CustomerAccount = EMPTY_STR;
            ProfessionalCustomer = false;
            BondAccruedInterest = EMPTY_STR;
            ExternalUserId = EMPTY_STR;
            ManualOrderIndicator = int.MaxValue;
        }

        /// <summary>
        /// Randomizes the order's size.
        /// <i>Only for Volatility and Pegged to Volatility orders.</i>
        /// </summary>
        public bool RandomizeSize { get; set; }

        /// <summary>
        /// Randomizes the order's price.
        /// <i>Only for Volatility and Pegged to Volatility orders.</i>
        /// </summary>
        public bool RandomizePrice { get; set; }

        /// <summary>
        /// Pegged-to-benchmark orders: this attribute will contain the conId of the contract
        /// against which the order will be pegged.
        /// </summary>
        public int ReferenceContractId { get; set; }

        /// <summary>
        /// Pegged-to-benchmark orders: indicates whether the order's pegged price should increase
        /// or decreases.
        /// </summary>
        public bool IsPeggedChangeAmountDecrease { get; set; }

        /// <summary>
        /// Pegged-to-benchmark orders: amount by which the order's pegged price should move.
        /// </summary>
        public double PeggedChangeAmount { get; set; }

        /// <summary>
        /// Pegged-to-benchmark orders: the amount the reference contract needs to move to adjust
        /// the pegged order.
        /// </summary>
        public double ReferenceChangeAmount { get; set; }

        /// <summary>
        /// Pegged-to-benchmark orders: the exchange against which we want to observe the
        /// reference contract.
        /// </summary>
        public string? ReferenceExchange { get; set; }

        /// <summary>
        /// Adjusted Stop orders: the parent order will be adjusted to the given type when the
        /// adjusted trigger price is penetrated.
        /// </summary>
        public string? AdjustedOrderType { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies the trigger price to execute.
        /// </summary>
        public double TriggerPrice { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies the price offset for the stop to move in increments.
        /// </summary>
        public double LmtPriceOffset { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies the stop price of the adjusted (STP) parent
        /// </summary>
        public double AdjustedStopPrice { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies the stop limit price of the adjusted (STPL LMT) parent
        /// </summary>
        public double AdjustedStopLimitPrice { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies the trailing amount of the adjusted (TRAIL) parent
        /// </summary>
        public double AdjustedTrailingAmount { get; set; }

        /// <summary>
        /// Adjusted Stop orders: specifies where the trailing unit is an amount (set to 0) or a
        /// percentage (set to 1)
        /// </summary>
        public int AdjustableTrailingUnit { get; set; }

        /// <summary>
        /// Conditions determining when the order will be activated or canceled
        /// </summary>
        public List<OrderCondition> Conditions { get; set; }

        /// <summary>
        /// Indicates whether or not conditions will also be valid outside Regular Trading Hours
        /// </summary>
        public bool ConditionsIgnoreRth { get; set; }

        /// <summary>
        /// Conditions can determine if an order should become active or canceled.
        /// </summary>
        public bool ConditionsCancelOrder { get; set; }

        /// <summary>
        /// Define the Soft Dollar Tier used for the order. Only provided for registered
        /// professional advisors and hedge and mutual funds.
        /// </summary>
        public SoftDollarTier Tier { get; set; }

        /// <summary>
        /// Set to true to create tickets from API orders when TWS is used as an OMS
        /// </summary>
        public bool IsOmsContainer { get; set; }

        /// <summary>
        /// Set to true to convert order of type 'Primary Peg' to 'D-Peg'
        /// </summary>
        public bool DiscretionaryUpToLimitPrice { get; set; }

        /// <summary>
        /// Specifies wether to use Price Management Algo. <i>CTCI users only.</i>
        /// </summary>
        public bool? UsePriceMgmtAlgo { get; set; }

        /// <summary>
        /// Specifies the duration of the order. Format: yyyymmdd hh:mm:ss TZ.
        /// <i>For GTD orders.</i>
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Value must be positive, and it is number of seconds that SMART order would be parked
        /// for at IBKRATS before being routed to exchange.
        /// </summary>
        public int PostToAts { get; set; }
    }
}
