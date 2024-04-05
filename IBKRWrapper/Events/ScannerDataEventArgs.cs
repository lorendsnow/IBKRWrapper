using IBApi;

namespace IBKRWrapper.Events
{
    public class ScannerDataEventArgs(int reqId, int rank, Contract contract) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public int Rank { get; private set; } = rank;
        public Contract Contract { get; private set; } = contract;
    }
}
