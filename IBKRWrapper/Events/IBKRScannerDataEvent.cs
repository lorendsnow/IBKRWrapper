using IBApi;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds the request ID, rank and <see cref="IBApi.Contract"/> for scanner data results received from IBKR.
    /// </summary>
    public class IBKRScannerDataEventArgs : EventArgs
    {
        public IBKRScannerDataEventArgs(int reqId, int rank, Contract contract)
        {
            ReqId = reqId;
            Rank = rank;
            Contract = contract;
        }

        public int ReqId { get; set; }
        public int Rank { get; set; }
        public Contract Contract { get; set; }
    }
}
