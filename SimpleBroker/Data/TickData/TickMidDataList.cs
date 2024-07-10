using IBKRWrapper.Events;

namespace SimpleBroker
{
    /// <summary>
    /// Base tick data list to receive and hold tick data from IBKR.
    /// </summary>
    public class TickMidDataList : TickDataList<TickMid>
    {
        /// <summary>
        /// Receives and handles new ticks from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        public void HandleTickData(object? sender, TickMidEventArgs e)
        {
            TickMid tick =
                new()
                {
                    ReqId = e.ReqId,
                    Time = e.Time,
                    Price = e.MidPoint,
                };

            OnNewTickEvent(new(tick));

            Data.Add(tick);
        }
    }
}
