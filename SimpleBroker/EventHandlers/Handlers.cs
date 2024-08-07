﻿using IBKRWrapper.Events;

namespace SimpleBroker.EventHandlers
{
    internal static class Handlers
    {
        internal static EventHandler<AccountDownloadEndEventArgs> AccountDownloadEndHandler(
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

        internal static EventHandler<AccountDownloadEndEventArgs> AccountDownloadEndHandler(
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

        internal static EventHandler<ContractDetailsEventArgs> ContractDetailsHandler(
            List<Contract> contracts,
            int reqId
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    contracts.Add(e.Details.Contract.ToBrokerContract());
                }
            };
        }

        internal static EventHandler<ContractDetailsEndEventArgs> ContractDetailsEndHandler(
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

        internal static EventHandler<IBKRWrapper.Events.ErrorEventArgs> ErrorHandler<T>(
            int reqId,
            TaskCompletionSource<T> tcs
        )
        {
            return (sender, e) =>
            {
                if (e.Id == reqId)
                {
                    tcs.SetException(new Exception(e.ErrorMsg));
                }
            };
        }

        internal static EventHandler<HeadTimestampEventArgs> HeadTimestampHandler(
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

        internal static EventHandler<HistoricalDataEndEventArgs> HistoricalDataEndHandler(
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

        internal static EventHandler<HistoricalDataEventArgs> HistoricalDataHandler(
            List<Bar> bars,
            int reqId,
            string symbol
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId == reqId)
                {
                    bars.Add(e.Bar.ToBrokerBar(symbol));
                }
            };
        }

        internal static EventHandler<HistoricalTicksBidAskEventArgs> HistoricalTicksBidAskHandler(
            List<HistoricalTickBidAsk> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTickBidAsk>> tcs,
            string symbol
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks.Select(x => x.ToBrokerHistoricalTickBidAsk(symbol)));

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        internal static EventHandler<HistoricalTicksLastEventArgs> HistoricalTicksLastHandler(
            List<HistoricalTickLast> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTickLast>> tcs,
            string symbol
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks.Select(x => x.ToBrokerHistoricalTickLast(symbol)));

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        internal static EventHandler<HistoricalTicksMidEventArgs> HistoricalTicksMidHandler(
            List<HistoricalTick> ticks,
            int reqId,
            TaskCompletionSource<List<HistoricalTick>> tcs,
            string symbol
        )
        {
            return (sender, e) =>
            {
                if (e.ReqId != reqId)
                {
                    return;
                }

                ticks.AddRange(e.Ticks.Select(x => x.ToBrokerHistoricalTick(symbol)));

                if (e.Done)
                {
                    tcs.SetResult(ticks);
                }
            };
        }

        internal static EventHandler OpenOrderEndHandler(
            List<OpenOrderEventArgs> orders,
            TaskCompletionSource<List<OpenOrderEventArgs>> tcs
        )
        {
            return (sender, e) => tcs.SetResult(orders);
        }

        internal static EventHandler<OpenOrderEventArgs> OpenOrderHandler(List<Trade> trades)
        {
            return (sender, e) =>
                trades.Add(
                    Trade.New(e.OrderId, e.Contract.ToBrokerContract(), e.Order.ToBrokerOrder())
                );
        }

        internal static EventHandler OpenOrderEndHandler(
            List<Trade> trades,
            TaskCompletionSource<List<Trade>> tcs
        )
        {
            return (sender, e) => tcs.SetResult(trades);
        }

        internal static EventHandler<OptionsChainEndEventArgs> OptionsChainEndHandler(
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

        internal static EventHandler<OrderStatusEventArgs> OrderStatusHandler(
            int orderId,
            TaskCompletionSource<Trade> tcs,
            Trade trade
        )
        {
            return (sender, e) =>
            {
                if (e.OrderId == orderId && e.Accepted)
                {
                    tcs.SetResult(trade);
                }
                else if (e.OrderId == orderId && e.Canceled)
                {
                    tcs.SetCanceled();
                }
            };
        }

        internal static EventHandler<PositionEventArgs> PositionHandler(List<Position> positions)
        {
            return (sender, e) =>
                positions.Add(
                    new Position()
                    {
                        ConId = e.Contract.ConId,
                        Symbol = e.Contract.Symbol,
                        SecType = e.Contract.SecType,
                        Exchange = e.Contract.Exchange,
                        Currency = e.Contract.Currency,
                        PrimaryExch = e.Contract.PrimaryExch,
                        Positions = e.Position,
                        AvgCost = e.AverageCost,
                        Account = e.Account
                    }
                );
        }

        internal static EventHandler PositionEndHandler(
            List<Position> positions,
            TaskCompletionSource<List<Position>> tcs
        )
        {
            return (sender, e) =>
            {
                tcs.SetResult(positions);
            };
        }

        internal static EventHandler<ScannerParametersEventArgs> ScannerParametersHandler(
            TaskCompletionSource<string> tcs
        )
        {
            return (sender, e) => tcs.SetResult(e.XML);
        }

        internal static EventHandler<UpdateAccountValueEventArgs> UpdateAccountValueHandler(
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

        internal static EventHandler<UpdatePortfolioEventArgs> UpdatePortfolioHandler(
            List<PortfolioPosition> positions
        )
        {
            return (sender, e) =>
                positions.Add(
                    new()
                    {
                        Ticker = e.Contract.Symbol,
                        SecType = e.Contract.SecType,
                        Exchange = e.Contract.Exchange,
                        PrimaryExch = e.Contract.PrimaryExch,
                        Quantity = e.Position,
                        MarketPrice = e.MarketPrice,
                        MarketValue = e.MarketValue,
                        AverageCost = e.AverageCost,
                        UnrealizedPNL = e.UnrealizedPNL,
                        RealizedPNL = e.RealizedPNL,
                        Account = e.Account
                    }
                );
        }
    }
}
