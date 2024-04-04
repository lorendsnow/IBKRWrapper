using IBApi;

namespace IBKRWrapper.Events
{
    public class ContractDetailsEventArgs(int reqId, ContractDetails details) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public ContractDetails Details { get; private set; } = details;
    }
}
