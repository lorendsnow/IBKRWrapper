namespace IBKRWrapper.Events
{
    public class ContractDetailsEndEventArgs(int reqId) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
    }
}
