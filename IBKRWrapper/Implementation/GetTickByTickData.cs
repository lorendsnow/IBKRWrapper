using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        /// <summary>
        /// Emits tick data when received from IBKR.
        /// </summary>
        public event EventHandler<IBKRTickEventArgs>? IBKRTickEvent;

        /// <summary>
        /// Request streaming tick-by-tick "Last" data for a given contract.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns><see cref="TickDataList"/> - holds request information and a list of ticks received from IBKR.</returns>
        public TickDataList GetLastTicks(Contract contract)
        {
            int reqId = _reqId++;
            TickDataList tickDataList =
                new()
                {
                    ReqId = reqId,
                    Contract = contract,
                    TickType = "Last",
                    Data = []
                };

            IBKRTickEvent += tickDataList.HandleNewTick;

            clientSocket.reqTickByTickData(reqId, contract, "Last", 0, false);

            return tickDataList;
        }

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
            DateTimeOffset convertedTime = DateTimeOffset.FromUnixTimeSeconds(time);

            TickData tick =
                new()
                {
                    Time = convertedTime,
                    Last = price,
                    LastSize = size,
                    Exchange = exchange,
                    SpecialConditions = specialConditions
                };

            OnIBKRTickEvent(new IBKRTickEventArgs(reqId, tick));
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
            DateTimeOffset convertedTime = DateTimeOffset.FromUnixTimeSeconds(time);

            TickData tick =
                new()
                {
                    Time = convertedTime,
                    Bid = bidPrice,
                    Ask = askPrice,
                    BidSize = bidSize,
                    AskSize = askSize
                };

            OnIBKRTickEvent(new IBKRTickEventArgs(reqId, tick));
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            DateTimeOffset convertedTime = DateTimeOffset.FromUnixTimeSeconds(time);

            TickData tick = new() { Time = convertedTime, Mid = midPoint };

            OnIBKRTickEvent(new IBKRTickEventArgs(reqId, tick));
        }

        public void OnIBKRTickEvent(IBKRTickEventArgs e)
        {
            IBKRTickEvent?.Invoke(this, e);
        }
    }
}
