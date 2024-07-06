using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

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
            DateTimeOffset convertedTime = DateTimeOffset.FromUnixTimeSeconds(time);
            RealTimeBar bar = new(convertedTime, open, high, low, close, volume, WAP, count);

            RealTimeBarEvent?.Invoke(this, new RealTimeBarEventArgs(reqId, bar));
        }
    }
}
