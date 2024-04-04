namespace IBKRWrapper.Events
{
    public class AccountDownloadEndEventArgs(string account) : EventArgs
    {
        public string Account { get; private set; } = account;
    }
}
