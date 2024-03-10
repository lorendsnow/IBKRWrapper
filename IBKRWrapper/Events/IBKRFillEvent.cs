using IBApi;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds a <see cref="IBApi.Contract"/> and <see cref="IBApi.Execution"/> received from IBKR.
    /// </summary>
    public class IBKRFillEventArgs : EventArgs
    {
        public IBKRFillEventArgs(Contract contract, Execution execution)
        {
            Contract = contract;
            Execution = execution;
        }

        public Contract Contract { get; set; }
        public Execution Execution { get; set; }
    }
}
