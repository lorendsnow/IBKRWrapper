using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;
using IBKRWrapper.Utils;

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
                HandlerFactory.MakeContractDetailsHandler(contracts, reqId);
            EventHandler<ContractDetailsEndEventArgs> contractDetailsEndHandler =
                HandlerFactory.MakeContractDetailsEndHandler(contracts, reqId, tcs);

            ContractDetailsEvent += contractDetailsHandler;
            ContractDetailsEndEvent += contractDetailsEndHandler;

            tcs.Task.ContinueWith(t =>
            {
                ContractDetailsEvent -= contractDetailsHandler;
                ContractDetailsEndEvent -= contractDetailsEndHandler;
            });

            clientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        public Task<List<Contract>> GetQualifiedOptionContractAsync(
            string symbol,
            string date,
            double strike,
            string right,
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
                    SecType = "OPT",
                    LastTradeDateOrContractMonth = date,
                    Strike = strike,
                    Right = right,
                    Exchange = exchange,
                    Currency = currency
                };

            EventHandler<ContractDetailsEventArgs> contractDetailsHandler =
                HandlerFactory.MakeContractDetailsHandler(contracts, reqId);
            EventHandler<ContractDetailsEndEventArgs> contractDetailsEndHandler =
                HandlerFactory.MakeContractDetailsEndHandler(contracts, reqId, tcs);

            ContractDetailsEvent += contractDetailsHandler;
            ContractDetailsEndEvent += contractDetailsEndHandler;

            tcs.Task.ContinueWith(t =>
            {
                ContractDetailsEvent -= contractDetailsHandler;
                ContractDetailsEndEvent -= contractDetailsEndHandler;
            });

            clientSocket.reqContractDetails(reqId, contract);

            return tcs.Task;
        }

        public Task<OptionsChain> GetOptionsChain(string symbol)
        {
            int reqId;
            lock (_reqIdLock)
            {
                reqId = _reqId++;
            }

            TaskCompletionSource<OptionsChain> tcs = new();
            OptionsChain result = new() { ReqId = reqId, Symbol = symbol };
            EventHandler<OptionsChainEndEventArgs> handler =
                HandlerFactory.MakeOptionsChainEndHandler(tcs, result);
            List<Contract> cons = GetQualifiedStockContractAsync(symbol).Result;
            int conId = cons[0].ConId;

            OptionsChainEvent += result.HandleOptionsChainData;
            OptionsChainEndEvent += handler;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            tcs.Task.ContinueWith(t =>
            {
                OptionsChainEvent -= result.HandleOptionsChainData;
                OptionsChainEndEvent -= handler;
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            clientSocket.reqSecDefOptParams(reqId, symbol, "", "STK", conId);

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

        public event EventHandler<OptionsChainEventArgs>? OptionsChainEvent;

        public void securityDefinitionOptionParameter(
            int reqId,
            string exchange,
            int underlyingConId,
            string tradingClass,
            string multiplier,
            HashSet<string> expirations,
            HashSet<double> strikes
        )
        {
            OptionsChainEvent?.Invoke(
                this,
                new OptionsChainEventArgs(
                    reqId,
                    exchange,
                    underlyingConId,
                    tradingClass,
                    multiplier,
                    expirations,
                    strikes
                )
            );
        }

        public event EventHandler<OptionsChainEndEventArgs>? OptionsChainEndEvent;

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            OptionsChainEndEvent?.Invoke(this, new OptionsChainEndEventArgs(reqId));
        }
    }
}
