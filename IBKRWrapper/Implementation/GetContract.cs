using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        public Task<List<Contract>> GetQualifiedStockContractAsync(
            string symbol,
            string exchange = "SMART",
            string currency = "USD"
        )
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            TaskCompletionSource<List<Contract>> tcs = new();
            List<Contract> contracts = [];

            Contract contract =
                new()
                {
                    Symbol = symbol,
                    SecType = "STK",
                    Exchange = exchange,
                    Currency = currency
                };

            EventHandler<ContractDetailsEventArgs> contractDetailsHandler =
                new(
                    (sender, e) =>
                    {
                        contracts.Add(e.Details.Contract);
                    }
                );
            EventHandler<ContractDetailsEndEventArgs> contractDetailsEndHandler =
                new(
                    (sender, e) =>
                    {
                        if (e.ReqId == reqId)
                        {
                            tcs.SetResult(contracts);
                        }
                    }
                );

            tcs.Task.ContinueWith(t =>
            {
                ContractDetailsEvent -= contractDetailsHandler;
                ContractDetailsEndEvent -= contractDetailsEndHandler;
            });

            clientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        public event EventHandler<ContractDetailsEventArgs>? ContractDetailsEvent;

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            ContractDetailsEvent?.Invoke(
                this,
                new ContractDetailsEventArgs(reqId, contractDetails)
            );
        }

        public event EventHandler<ContractDetailsEndEventArgs>? ContractDetailsEndEvent;

        public void contractDetailsEnd(int reqId)
        {
            ContractDetailsEndEvent?.Invoke(this, new ContractDetailsEndEventArgs(reqId));
        }
    }
}
