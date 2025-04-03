namespace IBApi
{
    public class ScannerSubscription
    {
        public int NumberOfRows { get; set; } = -1;

        public string Instrument { get; set; } = string.Empty;

        public string LocationCode { get; set; } = string.Empty;

        public string ScanCode { get; set; } = string.Empty;

        public double AbovePrice { get; set; } = double.MaxValue;

        public double BelowPrice { get; set; } = double.MaxValue;

        public int AboveVolume { get; set; } = int.MaxValue;

        public int AverageOptionVolumeAbove { get; set; } = int.MaxValue;

        public double MarketCapAbove { get; set; } = double.MaxValue;

        public double MarketCapBelow { get; set; } = double.MaxValue;

        public string MoodyRatingAbove { get; set; } = string.Empty;

        public string MoodyRatingBelow { get; set; } = string.Empty;

        public string SpRatingAbove { get; set; } = string.Empty;

        public string SpRatingBelow { get; set; } = string.Empty;

        public string MaturityDateAbove { get; set; } = string.Empty;

        public string MaturityDateBelow { get; set; } = string.Empty;

        public double CouponRateAbove { get; set; } = double.MaxValue;

        public double CouponRateBelow { get; set; } = double.MaxValue;

        public bool ExcludeConvertible { get; set; }

        public string ScannerSettingPairs { get; set; } = string.Empty;

        public string StockTypeFilter { get; set; } = string.Empty;
    }
}
