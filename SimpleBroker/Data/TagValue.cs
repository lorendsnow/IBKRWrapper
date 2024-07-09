namespace SimpleBroker
{
    /// <summary>
    /// Convenience class to define key-value pairs
    /// </summary>
    public class TagValue(string tag, string value)
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Tag { get; set; } = value;

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; } = tag;
    }
}
