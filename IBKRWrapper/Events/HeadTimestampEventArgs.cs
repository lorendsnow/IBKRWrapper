using IBKRWrapper.Utils;

namespace IBKRWrapper.Events
{
    public class HeadTimestampEventArgs(int reqId, string headTimestamp) : EventArgs
    {
        public int ReqId { get; private set; } = reqId;
        public DateTimeOffset HeadTimestamp { get; private set; } =
            IbDateParser.ParseIBDateTime(headTimestamp);
    }
}
