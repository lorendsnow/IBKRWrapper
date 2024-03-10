using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        /// <summary>
        /// Emits an IBKR error if received.
        /// </summary>
        public event EventHandler<IBKRErrorEventArgs>? IBKRErrorEvent;

        public virtual void error(
            int id,
            int errorCode,
            string errorMsg,
            string advancedOrderRejectJson
        )
        {
            IBKRError error =
                new()
                {
                    Id = id,
                    ErrorCode = errorCode,
                    ErrorMsg = errorMsg,
                    AdvancedOrderRejectJson = advancedOrderRejectJson
                };

            IBKRErrorEvent?.Invoke(this, new IBKRErrorEventArgs(error));
        }

        public virtual void error(Exception e)
        {
            IBKRError error = new() { Exception = e };

            IBKRErrorEvent?.Invoke(this, new IBKRErrorEventArgs(error));
        }

        public virtual void error(string str)
        {
            IBKRError error = new() { ErrorMsg = str };

            IBKRErrorEvent?.Invoke(this, new IBKRErrorEventArgs(error));
        }
    }
}
