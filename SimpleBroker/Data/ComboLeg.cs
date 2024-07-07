namespace SimpleBroker
{
    /// <summary>
    /// Class representing a leg within combo orders.
    /// </summary>
    public class ComboLeg
    {
        /// <summary>
        /// The Contract's IB's unique id
        /// </summary>
        public int ConId { get; set; }

        /// <summary>
        /// Select the relative number of contracts for the leg you are constructing
        /// </summary>
        public int Ratio { get; set; }

        /// <summary>
        /// The side (BUY or SELL) of the leg
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// The destination exchange to which the order will be routed
        /// </summary>
        public string Exchange { get; set; } = string.Empty;

        /// <summary>
        /// Specifies whether an order is an open or closing order. Retail customers can only use 0, which refers to same as the parent security, so this value is hardcoded to 0.
        /// </summary>
        public int OpenClose { get; } = 0;

        /// <summary>
        /// For stock legs when doing short selling. Set to 1 = clearing broker, 2 = third party
        /// </summary>
        public int ShortSaleSlot { get; set; }

        /// <summary>
        /// When ShortSaleSlot is 2, this field shall contain the designated location.
        /// </summary>
        public string DesignatedLocation { get; set; } = string.Empty;

        /// <summary>
        /// Mark order as exempt from short sale uptick rule. 0 does not apply the rule, -1 applies the short sale uptick rule.
        /// </summary>
        public int ExemptCode { get; set; }
    }
}
