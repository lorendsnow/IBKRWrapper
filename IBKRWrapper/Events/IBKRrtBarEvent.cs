using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class IBKRrtBarEventArgs(int reqId, RealTimeBar bar) : EventArgs
    {
        public int ReqId { get; set; } = reqId;
        public RealTimeBar RealTimeBar { get; set; } = bar;
    }
}
