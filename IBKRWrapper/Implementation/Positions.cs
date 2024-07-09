using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public event EventHandler<PositionEventArgs>? PositionEvent;

        public void position(string account, Contract contract, decimal pos, double avgCost)
        {
            PositionEvent?.Invoke(this, new PositionEventArgs(account, contract, pos, avgCost));
        }

        public event EventHandler? PositionEndEvent;

        public void positionEnd() => PositionEndEvent?.Invoke(this, new EventArgs());
    }
}
