using IBApi;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<Events.ErrorEventArgs>? ErrorEvent;

        public void error(int id, int errorCode, string errorMsg, string advancedOrderRejectJson)
        {
            ErrorEvent?.Invoke(
                this,
                new Events.ErrorEventArgs(id, errorCode, errorMsg, advancedOrderRejectJson, null)
            );
        }

        public void error(Exception e)
        {
            ErrorEvent?.Invoke(this, new Events.ErrorEventArgs(null, null, null, null, e));
        }

        public void error(string str)
        {
            ErrorEvent?.Invoke(this, new Events.ErrorEventArgs(null, null, str, null, null));
        }
    }
}
