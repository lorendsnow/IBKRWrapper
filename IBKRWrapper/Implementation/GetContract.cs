using IBApi;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private Dictionary<int, List<Contract>> _contractDetailsResults = [];
        private Dictionary<int, TaskCompletionSource<List<Contract>>> _contractDetailsTcs = [];

        /// <summary>
        /// Gets a fully qualified stock contract from the TWS server.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="exchange"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Task<List<Contract>> GetQualifiedStockContractAsync(
            string symbol,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            int reqId = _reqId++;
            _contractDetailsResults[reqId] = [];
            TaskCompletionSource<List<Contract>> tcs = new();
            _contractDetailsTcs.Add(reqId, tcs);

            Contract contract =
                new()
                {
                    Symbol = symbol,
                    SecType = "STK",
                    Exchange = exchange,
                    Currency = currency
                };

            clientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            _contractDetailsResults[reqId].Add(contractDetails.Contract);
        }

        public void contractDetailsEnd(int reqId)
        {
            _contractDetailsTcs[reqId].SetResult(_contractDetailsResults[reqId]);
            _contractDetailsResults.Remove(reqId);
            _contractDetailsTcs.Remove(reqId);
        }
    }
}
