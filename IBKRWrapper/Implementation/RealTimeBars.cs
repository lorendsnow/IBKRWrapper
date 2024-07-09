using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<RealTimeBarEventArgs>? RealTimeBarEvent;

        public virtual void realtimeBar(
            int reqId,
            long time,
            double open,
            double high,
            double low,
            double close,
            decimal volume,
            decimal WAP,
            int count
        )
        {
            RealTimeBarEvent?.Invoke(
                this,
                new RealTimeBarEventArgs(reqId, time, open, high, low, close, volume, WAP, count)
            );
        }
    }
}
