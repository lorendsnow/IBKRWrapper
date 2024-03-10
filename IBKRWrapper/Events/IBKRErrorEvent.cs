namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds error information received from IBKR.
    /// </summary>
    public class IBKRErrorEventArgs : EventArgs
    {
        public IBKRErrorEventArgs(IBKRError error)
        {
            Error = error;
        }

        public IBKRError Error { get; set; }
    }

    public record IBKRError
    {
        public int? Id { get; init; }
        public int? ErrorCode { get; init; }
        public string? ErrorMsg { get; init; }
        public string? AdvancedOrderRejectJson { get; init; }
        public Exception? Exception { get; init; }
    }
}
