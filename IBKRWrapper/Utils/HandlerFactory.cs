using System.Diagnostics;
using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper.Utils
{
    public static class HandlerFactory
    {
        public static EventHandler<UpdatePortfolioEventArgs> MakeUpdatePortfolioHandler(
            List<PortfolioPosition> positions
        )
        {
            return (sender, e) => positions.Add(e.Position);
        }

        public static EventHandler<AccountDownloadEndEventArgs> MakeAccountDownloadEndHandler(
            string account,
            TaskCompletionSource<List<PortfolioPosition>> tcs,
            List<PortfolioPosition> positions
        )
        {
            return (sender, e) =>
            {
                if (e.Account == account)
                {
                    tcs.SetResult(positions);
                }
            };
        }

        public static EventHandler<UpdateAccountValueEventArgs> MakeUpdateAccountValueHandler(
            string account,
            Dictionary<string, string> accountValues
        )
        {
            return (sender, e) =>
            {
                if (e.AccountName == account && e.Currency == "USD")
                {
                    accountValues.Add(e.Key, e.Value);
                }
            };
        }

        public static EventHandler<AccountDownloadEndEventArgs> MakeAccountDownloadEndHandler(
            string account,
            TaskCompletionSource<Dictionary<string, string>> tcs,
            Dictionary<string, string> accountValues
        )
        {
            return (sender, e) =>
            {
                if (e.Account == account)
                {
                    tcs.SetResult(accountValues);
                }
            };
        }

        public static EventHandler<ContractDetailsEventArgs> MakeContractDetailsHandler(
            List<Contract> contracts,
            int reqId
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    contracts.Add(e.Details.Contract);
                }
            };
        }

        public static EventHandler<ContractDetailsEndEventArgs> MakeContractDetailsEndHandler(
            List<Contract> contracts,
            int reqId,
            TaskCompletionSource<List<Contract>> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    tcs.SetResult(contracts);
                }
            };
        }

        public static EventHandler<HistoricalDataEventArgs> MakeHistoricalDataHandler(
            List<Bar> bars,
            int reqId
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    bars.Add(e.Bar);
                }
            };
        }

        public static EventHandler<HistoricalDataEndEventArgs> MakeHistoricalDataEndHandler(
            List<Bar> bars,
            int reqId,
            TaskCompletionSource<List<Bar>> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    tcs.SetResult(bars);
                }
            };
        }

        public static EventHandler<HistoricalTicksLastEventArgs> MakeHistoricalTicksLastHandler(
            List<HistoricalTickLast> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTickLast>> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks);

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        public static EventHandler<HistoricalTicksBidAskEventArgs> MakeHistoricalTicksBidAskHandler(
            List<HistoricalTickBidAsk> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTickBidAsk>> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks);

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        public static EventHandler<HistoricalTicksMidEventArgs> MakeHistoricalTicksMidHandler(
            List<HistoricalTick> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTick>> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks);

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        public static EventHandler<HeadTimestampEventArgs> MakeHeadTimestampHandler(
            TaskCompletionSource<DateTimeOffset> tcs,
            int reqId
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    tcs.SetResult(e.HeadTimestamp);
                }
            };
        }

        public static EventHandler<PositionEventArgs> MakePositionHandler(List<Position> positions)
        {
            return (sender, e) => positions.Add(e.Position);
        }

        public static EventHandler MakePositionEndHandler(
            List<Position> positions,
            TaskCompletionSource<List<Position>> tcs
        )
        {
            return (sender, e) =>
            {
                tcs.SetResult(positions);
            };
        }

        public static EventHandler<ScannerParametersEventArgs> MakeScannerParametersHandler(
            TaskCompletionSource<string> tcs
        )
        {
            return (sender, e) => tcs.SetResult(e.XML);
        }

        public static EventHandler<OrderStatusEventArgs> MakeOrderStatusHandler(
            int orderId,
            TaskCompletionSource<Trade> tcs,
            Trade trade
        )
        {
            return (sender, e) =>
            {
                if (e.OrderStatus.OrderId == orderId && e.OrderStatus.Accepted)
                {
                    tcs.SetResult(trade);
                }
                else if (e.OrderStatus.OrderId == orderId && e.OrderStatus.Canceled)
                {
                    tcs.SetCanceled();
                }
            };
        }

        public static EventHandler<OpenOrderEventArgs> MakeOpenOrderHandler(
            List<OpenOrderEventArgs> orders
        )
        {
            return (sender, e) => orders.Add(e);
        }

        public static EventHandler MakeOpenOrderEndHandler(
            List<OpenOrderEventArgs> orders,
            TaskCompletionSource<List<OpenOrderEventArgs>> tcs
        )
        {
            return (sender, e) => tcs.SetResult(orders);
        }

        public static EventHandler<OpenOrderEventArgs> MakeOpenOrderAsTradesHandler(
            List<Trade> trades
        )
        {
            return (sender, e) =>
                trades.Add(new(e.OrderId, e.Contract, e.Order, e.OrderState.Status));
        }

        public static EventHandler MakeOpenOrderAsTradeEndHandler(
            List<Trade> trades,
            TaskCompletionSource<List<Trade>> tcs
        )
        {
            return (sender, e) => tcs.SetResult(trades);
        }

        public static EventHandler<OptionsChainEndEventArgs> MakeOptionsChainEndHandler(
            TaskCompletionSource<OptionsChain> tcs,
            OptionsChain chain
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != chain.ReqId)
                {
                    return;
                }
                tcs.SetResult(chain);
            };
        }
    }
}
