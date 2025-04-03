namespace IBApi
{
    public class OrderCancel
    {
        public string ManualOrderCancelTime { get; set; }

        public string ExtOperator { get; set; }

        public int ManualOrderIndicator { get; set; }

        public OrderCancel()
        {
            ManualOrderCancelTime = string.Empty;
            ExtOperator = string.Empty;
            ManualOrderIndicator = int.MaxValue;
        }

        public override bool Equals(object? p_other)
        {
            if (this == p_other)
                return true;

            if (!(p_other is OrderCancel l_theOther))
                return false;

            if (ManualOrderIndicator != l_theOther.ManualOrderIndicator)
            {
                return false;
            }
            if (
                Util.StringCompare(ManualOrderCancelTime, l_theOther.ManualOrderCancelTime) != 0
                || Util.StringCompare(ExtOperator, l_theOther.ExtOperator) != 0
            )
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 1040337091;
            hashCode *=
                -1521134295 + EqualityComparer<string>.Default.GetHashCode(ManualOrderCancelTime);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(ExtOperator);
            hashCode *= -1521134295 + ManualOrderIndicator.GetHashCode();

            return hashCode;
        }
    }
}
