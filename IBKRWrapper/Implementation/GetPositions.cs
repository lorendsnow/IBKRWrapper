using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;
using IBKRWrapper.Utils;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private readonly object positionsLock = new();

        public Task<List<Position>> GetPositionsAsync()
        {
            TaskCompletionSource<List<Position>> tcs = new();

            lock (positionsLock)
            {
                List<Position> positions = [];

                EventHandler<PositionEventArgs> positionHandler =
                    HandlerFactory.MakePositionHandler(positions);
                EventHandler positionEnd = HandlerFactory.MakePositionEndHandler(positions, tcs);

                PositionEvent += positionHandler;
                PositionEndEvent += positionEnd;

                tcs.Task.ContinueWith(t =>
                {
                    PositionEvent -= positionHandler;
                    PositionEndEvent -= positionEnd;
                });

                clientSocket.reqPositions();

                return tcs.Task;
            }
        }

        public event EventHandler<PositionEventArgs>? PositionEvent;

        public void position(string account, Contract contract, decimal pos, double avgCost)
        {
            PositionEvent?.Invoke(
                this,
                new PositionEventArgs(new Position(account, contract, pos, avgCost))
            );
        }

        public event EventHandler? PositionEndEvent;

        public void positionEnd() => PositionEndEvent?.Invoke(this, new EventArgs());
    }
}
