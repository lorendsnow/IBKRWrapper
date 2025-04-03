namespace IBApi
{
    public class Contract
    {
        public int ConId { get; set; }

        public string Symbol { get; set; }

        public string SecType { get; set; }

        public string LastTradeDateOrContractMonth { get; set; }

        public string LastTradeDate { get; set; }

        public double Strike { get; set; }

        public string Right { get; set; }

        public string Multiplier { get; set; }

        public string Exchange { get; set; }

        public string Currency { get; set; }

        public string LocalSymbol { get; set; }

        public string PrimaryExch { get; set; }

        public string TradingClass { get; set; }

        public bool IncludeExpired { get; set; }

        public string SecIdType { get; set; }

        public string SecId { get; set; }

        public string Description { get; set; }

        public string IssuerId { get; set; }

        public string ComboLegsDescription { get; set; }

        public List<ComboLeg> ComboLegs { get; set; }

        public DeltaNeutralContract DeltaNeutralContract { get; set; }

        public override string ToString() => $"{SecType} {Symbol} {Currency} {Exchange}";
    }
}
