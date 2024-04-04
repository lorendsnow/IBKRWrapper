namespace IBKRWrapper.Events
{
    public class UpdateAccountValueEventArgs(
        string key,
        string value,
        string currency,
        string accountName
    ) : EventArgs
    {
        public string Key { get; private set; } = key;
        public string Value { get; private set; } = value;
        public string Currency { get; private set; } = currency;
        public string AccountName { get; private set; } = accountName;
    }
}
