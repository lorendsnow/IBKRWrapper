using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class IBKRTickEventArgs(int reqId, TickData tick) : EventArgs
    {
        public int ReqId { get; set; } = reqId;
        public TickData TickData { get; set; } = tick;
    }
}
