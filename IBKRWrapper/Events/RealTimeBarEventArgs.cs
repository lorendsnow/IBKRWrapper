using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class RealTimeBarEventArgs(int reqId, RealTimeBar bar) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public RealTimeBar RealTimeBar { get; private set; } = bar;
    }
}
