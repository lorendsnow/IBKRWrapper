using IBApi;

namespace IBKRWrapper.Events
{
    public class ExecDetailsEventArgs(int reqId, Contract contract, Execution execution) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public Contract Contract { get; private set; } = contract;
        public Execution Execution { get; private set; } = execution;
    }
}
