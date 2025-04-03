namespace IBApi
{
    public class TagValue
    {
        public required string Tag { get; set; }

        public required string Value { get; set; }

        public TagValue()
        {
            Tag = string.Empty;
            Value = string.Empty;
        }

        public TagValue(string p_tag, string p_value)
        {
            Tag = p_tag;
            Value = p_value;
        }

        public override bool Equals(object? other)
        {
            if (this == other)
                return true;
            if (other is not TagValue l_theOther)
                return false;
            if (Util.StringCompare(Tag, l_theOther.Tag) != 0)
                return false;
            if (Util.StringCompare(Value, l_theOther.Value) != 0)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 221537429;
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}
