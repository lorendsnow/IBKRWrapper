using IBApi;

namespace IBKRWrapper.Events
{
    public class HistoricalDataEventArgs(int reqId, Bar bar) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public Bar Bar { get; private set; } = bar;
    }

    public class HistoricalDataEndEventArgs(int reqId, string startDate, string endDate) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public string StartDate { get; private set; } = startDate;
        public string EndDate { get; private set; } = endDate;
    }
}
