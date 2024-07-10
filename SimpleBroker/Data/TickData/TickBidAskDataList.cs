using IBKRWrapper.Events;

namespace SimpleBroker
{
    /// <summary>
    /// Base tick data list to receive and hold tick data from IBKR.
    /// </summary>
    public class TickBidAskDataList : TickDataList<TickBidAsk>
    {
        /// <summary>
        /// Receives and handles new ticks from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        public void HandleTickData(object? sender, TickBidAskEventArgs e)
        {
            TickBidAsk tick =
                new()
                {
                    ReqId = e.ReqId,
                    Time = e.Time,
                    BidPrice = e.BidPrice,
                    AskPrice = e.AskPrice,
                    BidSize = e.BidSize,
                    AskSize = e.AskSize,
                    TickAttribBidAsk =
                        e.TickAttribBidAsk?.ToBrokerTickAttribBidAsk()
                        ?? throw new Exception("Args didn't include the tickattribute"),
                };

            OnNewTickEvent(new(tick));

            Data.Add(tick);
        }
    }
}
