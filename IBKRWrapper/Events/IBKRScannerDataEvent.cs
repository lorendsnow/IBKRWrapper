using IBApi;

namespace IBKRWrapper.Events
{
    public class IBKRScannerDataEventArgs(int reqId, int rank, Contract contract) : EventArgs
    {
        public int ReqId { get; set; } = reqId;
        public int Rank { get; set; } = rank;
        public Contract Contract { get; set; } = contract;
    }
}
