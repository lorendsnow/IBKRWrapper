using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<HistoricalDataEventArgs>? HistoricalData;

        public void historicalData(int reqId, Bar bar)
        {
            HistoricalData?.Invoke(this, new HistoricalDataEventArgs(reqId, bar));
        }

        public event EventHandler<HistoricalDataEndEventArgs>? HistoricalDataEnd;

        public void historicalDataEnd(int reqId, string startDate, string endDate)
        {
            HistoricalDataEnd?.Invoke(
                this,
                new HistoricalDataEndEventArgs(reqId, startDate, endDate)
            );
        }

        public event EventHandler<HeadTimestampEventArgs>? HeadTimestamp;

        public void headTimestamp(int reqId, string headTimestamp)
        {
            HeadTimestamp?.Invoke(this, new HeadTimestampEventArgs(reqId, headTimestamp));
        }

        public event EventHandler<HistoricalTicksLastEventArgs>? HistoricalTicksLast;

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            HistoricalTicksLast?.Invoke(this, new HistoricalTicksLastEventArgs(reqId, ticks, done));
        }

        public event EventHandler<HistoricalTicksBidAskEventArgs>? HistoricalTicksBidAsk;

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            HistoricalTicksBidAsk?.Invoke(
                this,
                new HistoricalTicksBidAskEventArgs(reqId, ticks, done)
            );
        }

        public event EventHandler<HistoricalTicksMidEventArgs>? HistoricalTicksMid;

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            HistoricalTicksMid?.Invoke(this, new HistoricalTicksMidEventArgs(reqId, ticks, done));
        }
    }
}
