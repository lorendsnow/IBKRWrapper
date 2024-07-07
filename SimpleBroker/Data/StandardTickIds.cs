namespace SimpleBroker
{
    /// <summary>
    /// IBKR tick IDs for standard tick types
    /// </summary>
    public enum StandardTickIds
    {
        /// <summary>
        /// Bid size
        /// </summary>
        BidSize = 0,

        /// <summary>
        /// Bid price
        /// </summary>
        BidPrice = 1,

        /// <summary>
        /// Ask price
        /// </summary>
        AskPrice = 2,

        /// <summary>
        /// Ask size
        /// </summary>
        AskSize = 3,

        /// <summary>
        /// Last trade price
        /// </summary>
        LastPrice = 4,

        /// <summary>
        /// Last trade size
        /// </summary>
        LastSize = 5,

        /// <summary>
        /// High price of the day
        /// </summary>
        High = 6,

        /// <summary>
        /// Low price of the day
        /// </summary>
        Low = 7,

        /// <summary>
        /// Volume for the day
        /// </summary>
        Volume = 8,

        /// <summary>
        /// Closing price for the day
        /// </summary>
        ClosePrice = 9,

        /// <summary>
        /// Bid option computation
        /// </summary>
        BidOptionComputation = 10,

        /// <summary>
        /// Ask option computation
        /// </summary>
        AskOptionComputation = 11,

        /// <summary>
        /// Last option computation
        /// </summary>
        LastOptionComputation = 12,

        /// <summary>
        /// Model option computation
        /// </summary>
        ModelOptionComputation = 13,

        /// <summary>
        /// Opening price for the day
        /// </summary>
        OpenTick = 14,

        /// <summary>
        /// Bid exchange
        /// </summary>
        BidExchange = 32,

        /// <summary>
        /// Ask exchange
        /// </summary>
        AskExchange = 33,

        /// <summary>
        /// Last timestamp
        /// </summary>
        LastTimestamp = 45,

        /// <summary>
        /// Halted
        /// </summary>
        Halted = 49,

        /// <summary>
        /// Last exchange
        /// </summary>
        LastExchange = 84,
    }
}
