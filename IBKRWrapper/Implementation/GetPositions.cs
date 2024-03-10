using IBApi;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private List<(Contract, decimal, double, string)>? _positionsResults = null;
        private Dictionary<string, TaskCompletionSource<List<Position>>> _positionsTcs = [];

        /// <summary>
        /// Gets a list of positions from the TWS server.
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Task<List<Position>> GetPositionsAsync(string account)
        {
            _positionsResults = [];
            TaskCompletionSource<List<Position>> tcs = new();
            _positionsTcs.Add(account, tcs);

            clientSocket.reqPositions();

            return tcs.Task;
        }

        public void position(string account, Contract contract, decimal pos, double avgCost)
        {
            if (_positionsResults != null)
            {
                _positionsResults.Add((contract, pos, avgCost, account));
            }
        }

        public void positionEnd()
        {
            if (_positionsResults is null)
            {
                return;
            }
            List<Position> positions = _positionsResults
                .Where(x => x.Item4 == _positionsTcs.Keys.First())
                .Select(x => PositionBuilder.Build(x.Item1, x.Item2, x.Item3, x.Item4))
                .ToList();

            _positionsTcs[_positionsTcs.Keys.First()].SetResult(positions);
            _positionsResults = null;
            _positionsTcs.Remove(_positionsTcs.Keys.First());
        }
    }
}
