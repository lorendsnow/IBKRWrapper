namespace SimpleBroker
{
    /// <summary>
    /// Defines a market scanner request
    /// </summary>
    public class ScannerSubscription
    {
        /// <summary>
        /// The number of rows to be returned for the query
        /// </summary>
        public int NumberOfRows { get; set; } = -1;

        /// <summary>
        /// The instrument's type for the scan. I.e. STK, FUT.HK, etc.
        /// </summary>
        public string? Instrument { get; set; }

        /// <summary>
        /// The request's location (STK.US, STK.US.MAJOR, etc).
        /// </summary>
        public string? LocationCode { get; set; }

        /// <summary>
        /// Same as TWS Market Scanner's "parameters" field, for example: TOP_PERC_GAIN
        /// </summary>
        public string? ScanCode { get; set; }

        /// <summary>
        /// Filters out Contracts which price is below this value
        /// </summary>
        public double AbovePrice { get; set; } = double.MaxValue;

        /// <summary>
        /// Filters out contracts which price is above this value.
        /// </summary>
        public double BelowPrice { get; set; } = double.MaxValue;

        /// <summary>
        /// Filters out Contracts which volume is above this value.
        /// </summary>
        public int AboveVolume { get; set; } = int.MaxValue;

        /// <summary>
        /// Filters out Contracts which option volume is above this value.
        /// </summary>
        public int AverageOptionVolumeAbove { get; set; } = int.MaxValue;

        /// <summary>
        /// Filters out Contracts which market cap is above this value.
        /// </summary>
        public double MarketCapAbove { get; set; } = double.MaxValue;

        /// <summary>
        /// Filters out Contracts which market cap is below this value.
        /// </summary>
        public double MarketCapBelow { get; set; } = double.MaxValue;

        /// <summary>
        /// Filters out Contracts which Moody's rating is below this value.
        /// </summary>
        public string? MoodyRatingAbove { get; set; }

        /// <summary>
        /// Filters out Contracts which Moody's rating is above this value.
        /// </summary>
        public string? MoodyRatingBelow { get; set; }

        /// <summary>
        /// Filters out Contracts with a S+P rating below this value.
        /// </summary>
        public string? SpRatingAbove { get; set; }

        /// <summary>
        /// Filters out Contracts with a S+P rating above this value.
        /// </summary>
        public string? SpRatingBelow { get; set; }

        /// <summary>
        /// Filter out Contracts with a maturity date earlier than this value.
        /// </summary>
        public string? MaturityDateAbove { get; set; }

        /// <summary>
        /// Filter out Contracts with a maturity date older than this value.
        /// </summary>
        public string? MaturityDateBelow { get; set; }

        /// <summary>
        /// Filter out Contracts with a coupon rate lower than this value.
        /// </summary>
        public double CouponRateAbove { get; set; } = double.MaxValue;

        /// <summary>
        /// Filter out Contracts with a coupon rate higher than this value.
        /// </summary>
        public double CouponRateBelow { get; set; } = double.MaxValue;

        /// <summary>
        /// Filters out Convertible bonds
        /// </summary>
        public bool ExcludeConvertible { get; set; }

        /// <summary>
        /// For example, a pairing "Annual, true" used on the "top Option Implied Vol % Gainers"
        /// scan would return annualized volatilities.
        /// </summary>
        public string? ScannerSettingPairs { get; set; }

        /// <summary>
        /// <para>CORP - Corporation</para>
        /// <para>ADE - American Depository Receipt</para>
        /// <para>ETF - Exchange Traded Fund</para>
        /// <para>REIT - Real Estate Investment Trust</para>
        /// <para>CEF - Closed End Fund</para>
        /// </summary>
        public string? StockTypeFilter { get; set; }
    }
}
