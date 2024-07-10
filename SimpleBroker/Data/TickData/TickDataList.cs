using IBKRWrapper;

namespace SimpleBroker
{
    /// <summary>
    /// Base tick data list to receive and hold tick data from IBKR.
    /// </summary>
    public class TickDataList<T>
    {
        /// <summary>
        /// The request ID sent to IBKR when requesting tick data
        /// </summary>
        public required int ReqId { get; init; }

        /// <summary>
        /// The underlying contract
        /// </summary>
        public required Contract Contract { get; init; }

        /// <summary>
        /// The list of tick data
        /// </summary>
        public required List<T> Data { get; set; }

        /// <summary>
        /// Emits a new tick upon receipt.
        /// </summary>
        public event EventHandler<NewTickEventArgs<T>>? NewTickEvent;

        /// <summary>
        /// Overridable method to handle new ticks.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNewTickEvent(NewTickEventArgs<T> e)
        {
            NewTickEvent?.Invoke(this, e);
        }

        /// <summary>
        /// Cancels the tick data subscription.
        /// </summary>
        /// <param name="wrapper"></param>
        public void CancelTickData(Wrapper wrapper)
        {
            wrapper.ClientSocket.clientSocket.cancelTickByTickData(ReqId);
        }

        /// <summary>
        /// ToString override to represent the tick data list.
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return (
                $"TickDataList(ReqId={ReqId}, "
                + $"Contract={ContractString(Contract)}, "
                + $"Data=[{string.Join(", ", Data.Select(x => x?.ToString()).ToArray())}])"
            );
        }

        private static string ContractString(Contract contract)
        {
            return (
                $"Contract(Ticker={contract.Symbol}, "
                + $"Type={contract.SecType}, "
                + $"Exchange={contract.Exchange}, "
                + $"PrimaryExchange={contract.PrimaryExch})"
            );
        }
    }
}
