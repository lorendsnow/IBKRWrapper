using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class TickByTickEventArgs(int reqId, TickData tickData) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public TickData TickData { get; private set; } = tickData;
    }
}
