using IBApi;

namespace IBKRWrapper.Events
{
    public class CommissionEventArgs(CommissionReport report) : EventArgs
    {
        public CommissionReport Commission { get; private set; } = report;
    }
}
