using IBKRWrapper.Events;

namespace SimpleBroker
{
    /// <summary>
    /// Base tick data list to receive and hold tick data from IBKR.
    /// </summary>
    public class TickLastDataList : TickDataList<TickLast>
    {
        /// <summary>
        /// Receives and handles new ticks from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        public void HandleTickData(object? sender, TickLastEventArgs e)
        {
            TickLast tick =
                new()
                {
                    ReqId = e.ReqId,
                    Time = e.Time,
                    Price = e.LastPrice,
                    Size = e.LastSize,
                    TickAttribLast =
                        e.TickAttribLast?.ToBrokerTickAttribLast()
                        ?? throw new Exception("Args didn't include the tickattribute"),
                    Exchange = e.Exchange,
                    SpecialConditions = e.SpecialConditions
                };

            OnNewTickEvent(new(tick));

            Data.Add(tick);
        }
    }
}
