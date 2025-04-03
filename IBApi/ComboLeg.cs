namespace IBApi
{
    public class ComboLeg
    {
        public const int SAME = 0;
        public const int OPEN = 1;
        public const int CLOSE = 2;
        public const int UNKNOWN = 3;

        public int ConId { get; set; }

        public int Ratio { get; set; }

        public string Action { get; set; }

        public string Exchange { get; set; }

        public int OpenClose { get; set; }

        public int ShortSaleSlot { get; set; }

        public string DesignatedLocation { get; set; }

        public int ExemptCode { get; set; }

        public ComboLeg()
        {
            Action = string.Empty;
            Exchange = string.Empty;
            DesignatedLocation = string.Empty;
        }

        public ComboLeg(
            int conId,
            int ratio,
            string action,
            string exchange,
            int openClose,
            int shortSaleSlot,
            string designatedLocation,
            int exemptCode
        )
        {
            ConId = conId;
            Ratio = ratio;
            Action = action;
            Exchange = exchange;
            OpenClose = openClose;
            ShortSaleSlot = shortSaleSlot;
            DesignatedLocation = designatedLocation;
            ExemptCode = exemptCode;
        }
    }
}
