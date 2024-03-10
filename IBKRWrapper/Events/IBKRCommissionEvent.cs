using IBApi;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds a <see cref="IBApi.CommissionReport"/> received from IBKR.
    /// </summary>
    public class IBKRCommissionEventArgs : EventArgs
    {
        public IBKRCommissionEventArgs(CommissionReport commissionReport)
        {
            CommissionReport = commissionReport;
        }

        public CommissionReport CommissionReport { get; set; }
    }
}
