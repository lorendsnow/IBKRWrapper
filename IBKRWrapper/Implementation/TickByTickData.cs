using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public TickDataList GetTickByTickData(Contract contract, string tickType)
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            TickDataList tickDataList = new(reqId, contract, tickType);

            TickByTickEvent += tickDataList.HandleNewTick;

            clientSocket.reqTickByTickData(reqId, contract, tickType, 0, false);

            return tickDataList;
        }

        public event EventHandler<TickByTickEventArgs>? TickByTickEvent;

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
            TickData tickData =
                new()
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds(time),
                    Last = price,
                    LastSize = size,
                    Exchange = exchange,
                    SpecialConditions = specialConditions
                };
            TickByTickEvent?.Invoke(this, new TickByTickEventArgs(reqId, tickData));
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
            TickData tickData =
                new()
                {
                    Time = DateTimeOffset.FromUnixTimeSeconds(time),
                    Bid = bidPrice,
                    Ask = askPrice,
                    BidSize = bidSize,
                    AskSize = askSize
                };

            TickByTickEvent?.Invoke(this, new TickByTickEventArgs(reqId, tickData));
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            TickData tickData =
                new() { Time = DateTimeOffset.FromUnixTimeSeconds(time), Mid = midPoint };

            TickByTickEvent?.Invoke(this, new TickByTickEventArgs(reqId, tickData));
        }
    }
}
