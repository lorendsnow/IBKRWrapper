using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds the request id and <see cref="Models.TickData"/> received from IBKR.
    /// </summary>
    public class IBKRTickEventArgs : EventArgs
    {
        public IBKRTickEventArgs(int reqId, TickData tick)
        {
            ReqId = reqId;
            TickData = tick;
        }

        public int ReqId { get; set; }
        public TickData TickData { get; set; }
    }
}
