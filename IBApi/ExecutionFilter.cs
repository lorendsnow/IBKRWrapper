namespace IBApi
{
    public class ExecutionFilter
    {
        public int ClientId { get; set; }

        public string AcctCode { get; set; }

        public string Time { get; set; }

        public string Symbol { get; set; }

        public string SecType { get; set; }

        public string Exchange { get; set; }

        public string Side { get; set; }

        public int LastNDays { get; set; }

        public List<int> SpecificDates { get; set; }

        public ExecutionFilter()
        {
            ClientId = 0;
            LastNDays = int.MaxValue;
            AcctCode = string.Empty;
            Time = string.Empty;
            Symbol = string.Empty;
            SecType = string.Empty;
            Exchange = string.Empty;
            Side = string.Empty;
            SpecificDates = [];
        }

        public ExecutionFilter(
            int clientId,
            string acctCode,
            string time,
            string symbol,
            string secType,
            string exchange,
            string side,
            int lastNDays,
            List<int> specificDates
        )
        {
            ClientId = clientId;
            AcctCode = acctCode;
            Time = time;
            Symbol = symbol;
            SecType = secType;
            Exchange = exchange;
            Side = side;
            LastNDays = lastNDays;
            SpecificDates = specificDates;
        }

        public override bool Equals(object? other)
        {
            bool l_bRetVal;
            if (!(other is ExecutionFilter l_theOther))
            {
                l_bRetVal = false;
            }
            else if (this == other)
            {
                l_bRetVal = true;
            }
            else
            {
                if (!Util.VectorEqualsUnordered(SpecificDates, l_theOther.SpecificDates))
                {
                    return false;
                }

                l_bRetVal =
                    ClientId == l_theOther.ClientId
                    && string.Equals(
                        AcctCode,
                        l_theOther.AcctCode,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && string.Equals(
                        Time,
                        l_theOther.Time,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && string.Equals(
                        Symbol,
                        l_theOther.Symbol,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && string.Equals(
                        SecType,
                        l_theOther.SecType,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && string.Equals(
                        Exchange,
                        l_theOther.Exchange,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && string.Equals(
                        Side,
                        l_theOther.Side,
                        System.StringComparison.OrdinalIgnoreCase
                    )
                    && LastNDays == l_theOther.LastNDays;
            }
            return l_bRetVal;
        }

        public override int GetHashCode()
        {
            var hashCode = 82934527;
            hashCode *= -1521134295 + ClientId.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(AcctCode);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Time);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Symbol);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(SecType);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Exchange);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Side);
            hashCode *= -1521134295 + LastNDays.GetHashCode();
            return hashCode;
        }
    }
}
