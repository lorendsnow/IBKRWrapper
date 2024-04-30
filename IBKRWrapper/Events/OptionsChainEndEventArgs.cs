namespace IBKRWrapper.Events
{
    public class OptionsChainEndEventArgs(int reqId) : EventArgs
    {
        public int ReqId { get; init; } = reqId;
    }
}
