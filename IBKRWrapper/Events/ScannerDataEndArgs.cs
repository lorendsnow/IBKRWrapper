namespace IBKRWrapper.Events
{
    public class ScannerDataEndArgs(int reqId) : EventArgs
    {
        public int ReqId { get; init; } = reqId;
    }
}
