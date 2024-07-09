using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<TickLastEventArgs>? TickLastEvent;
        public event EventHandler<TickBidAskEventArgs>? TickBidAskEvent;
        public event EventHandler<TickMidEventArgs>? TickMidEvent;

        public void tickByTickAllLast(
            int reqId,
            int tickType,
            long time,
            double price,
            decimal size,
            TickAttribLast tickAttribLast,
            string exchange,
            string specialConditions
        )
        {
            TickLastEvent?.Invoke(
                this,
                new TickLastEventArgs(
                    reqId,
                    tickType,
                    time,
                    price,
                    size,
                    tickAttribLast,
                    exchange,
                    specialConditions
                )
            );
        }

        public void tickByTickBidAsk(
            int reqId,
            long time,
            double bidPrice,
            double askPrice,
            decimal bidSize,
            decimal askSize,
            TickAttribBidAsk tickAttribBidAsk
        )
        {
            TickBidAskEvent?.Invoke(
                this,
                new TickBidAskEventArgs(
                    reqId,
                    time,
                    bidPrice,
                    askPrice,
                    bidSize,
                    askSize,
                    tickAttribBidAsk
                )
            );
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            TickMidEvent?.Invoke(this, new TickMidEventArgs(reqId, time, midPoint));
        }
    }
}
