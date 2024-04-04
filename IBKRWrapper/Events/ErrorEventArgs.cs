namespace IBKRWrapper.Events
{
    public class ErrorEventArgs(
        int? id,
        int? errorCode,
        string? errorMsg,
        string? advancedOrderRejectJson,
        Exception? e
    ) : EventArgs
    {
        public int? Id { get; private set; } = id;
        public int? ErrorCode { get; private set; } = errorCode;
        public string? ErrorMsg { get; private set; } = errorMsg;
        public string? AdvancedOrderRejectJson { get; private set; } = advancedOrderRejectJson;
        public Exception? Exception { get; private set; } = e;
    }
}
