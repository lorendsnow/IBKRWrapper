using System.Collections.Concurrent;
using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private Dictionary<OrderKey, Trade> _trades = [];
        private List<Trade>? _openOrdersResults = null;
        private Dictionary<string, TaskCompletionSource<List<Trade>>> _openOrdersTcs = [];
        private List<(Contract, Execution)>? _execDetailsResults = null;
        private Dictionary<int, TaskCompletionSource<List<(Contract, Execution)>>> _execDetailsTcs =
            [];
        public event EventHandler<IBKRFillEventArgs>? IBKRFillEvent;
        public event EventHandler<IBKROrderStatusEventArgs>? IBKROrderStatusEvent;
        public event EventHandler<IBKRCommissionEventArgs>? IBKRCommissionEvent;

        /// <summary>
        /// Place an order with IBKR.
        /// </summary>
        /// <param name="contract">IBKR contract object.</param>
        /// <param name="order">IBKR order object.</param>
        /// <returns><see cref="Trade"/></returns>
        public Trade PlaceOrder(Contract contract, Order order)
        {
            clientSocket.reqIds(1);
            while (!OrderIdIsValid) { }
            int orderId = NextOrderId;

            Trade trade = new(orderId, contract, order);
            IBKRFillEvent += trade.HandleFill;
            IBKROrderStatusEvent += trade.HandleOrderStatus;
            IBKRCommissionEvent += trade.HandleCommission;

            OrderKey key =
                new()
                {
                    ClientId = order.ClientId,
                    OrderId = orderId,
                    PermId = order.PermId
                };
            _trades.Add(key, trade);

            clientSocket.placeOrder(orderId, contract, order);
            OrderIdIsValid = false;
            return trade;
        }

        /// <summary>
        /// Cancel a pending order with IBKR.
        /// </summary>
        /// <param name="order">Order to be canceled.</param>
        public void CancelOrder(Order order)
        {
            clientSocket.cancelOrder(order.OrderId, DateTimeOffset.Now.ToString());
        }

        /// <summary>
        /// Get open orders from IBKR.
        /// </summary>
        /// <returns>List of <see cref="Trade"/> objects.</returns>
        public Task<List<Trade>> GetOpenOrdersAsync()
        {
            _openOrdersResults = [];
            TaskCompletionSource<List<Trade>> tcs = new();
            _openOrdersTcs.Add("openOrders", tcs);

            clientSocket.reqOpenOrders();

            return tcs.Task;
        }

        /// <summary>
        /// Get executions for this session from IBKR.
        /// </summary>
        /// <returns>List of tuples of <see cref="Contract"/> and <see cref="Execution"/> objects.</returns>
        public Task<List<(Contract, Execution)>> GetExecutionsAsync()
        {
            int reqId = _reqId++;
            _execDetailsResults = [];
            TaskCompletionSource<List<(Contract, Execution)>> tcs = new();
            _execDetailsTcs.Add(reqId, tcs);

            clientSocket.reqExecutions(reqId, new ExecutionFilter());

            return tcs.Task;
        }

        public void orderStatus(
            int orderId,
            string status,
            decimal filled,
            decimal remaining,
            double avgFillPrice,
            int permId,
            int parentId,
            double lastFillPrice,
            int clientId,
            string whyHeld,
            double mktCapPrice
        )
        {
            OnIBKROrderStatusEvent(
                new IBKROrderStatusEventArgs(
                    new OrderStatus(
                        orderId,
                        status,
                        filled,
                        remaining,
                        avgFillPrice,
                        permId,
                        parentId,
                        lastFillPrice,
                        whyHeld
                    )
                )
            );
        }

        public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            OrderKey key =
                new()
                {
                    ClientId = order.ClientId,
                    OrderId = orderId,
                    PermId = order.PermId
                };

            Trade trade = new(orderId, contract, order);

            if (!_trades.TryAdd(key, trade))
            {
                _trades[key].Order.PermId = order.PermId;
                _trades[key].Order.TotalQuantity = order.TotalQuantity;
                _trades[key].Order.LmtPrice = order.LmtPrice;
                _trades[key].Order.AuxPrice = order.AuxPrice;
                _trades[key].Order.OrderType = order.OrderType;
                _trades[key].Order.OrderRef = order.OrderRef;
            }
            IBKROrderStatusEvent += trade.HandleOrderStatus;
            IBKRFillEvent += trade.HandleFill;
            IBKRCommissionEvent += trade.HandleCommission;

            _openOrdersResults?.Add(_trades[key]);
        }

        public void openOrderEnd()
        {
            if (_openOrdersResults is null)
            {
                return;
            }
            List<Trade> results = _openOrdersResults;
            _openOrdersTcs["openOrders"].SetResult(results);
            _openOrdersResults = null;
            _openOrdersTcs.Clear();
        }

        public void commissionReport(CommissionReport commissionReport)
        {
            OnIBKRCommissionEvent(new IBKRCommissionEventArgs(commissionReport));
        }

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            if (_execDetailsTcs.ContainsKey(reqId) && _execDetailsResults is not null)
            // We're responding to GetExecutionsAsync so save the results
            {
                _execDetailsResults.Add((contract, execution));
            }

            OnIBKRFillEvent(new IBKRFillEventArgs(contract, execution));
        }

        public void execDetailsEnd(int reqId)
        {
            if (_execDetailsTcs.ContainsKey(reqId) && _execDetailsResults is not null)
            {
                _execDetailsTcs[reqId].SetResult(_execDetailsResults);
                _execDetailsResults = null;
                _execDetailsTcs.Remove(reqId);
            }
        }

        public void OnIBKRFillEvent(IBKRFillEventArgs e)
        {
            IBKRFillEvent?.Invoke(this, e);
        }

        public void OnIBKROrderStatusEvent(IBKROrderStatusEventArgs e)
        {
            IBKROrderStatusEvent?.Invoke(this, e);
        }

        public void OnIBKRCommissionEvent(IBKRCommissionEventArgs e)
        {
            IBKRCommissionEvent?.Invoke(this, e);
        }
    }
}
