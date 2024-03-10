using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds the request id and <see cref="Models.RealTimeBar"/> received from IBKR.
    /// </summary>
    public class IBKRrtBarEventArgs : EventArgs
    {
        public IBKRrtBarEventArgs(int reqId, RealTimeBar bar)
        {
            ReqId = reqId;
            RealTimeBar = bar;
        }

        public int ReqId { get; set; }
        public RealTimeBar RealTimeBar { get; set; }
    }
}
