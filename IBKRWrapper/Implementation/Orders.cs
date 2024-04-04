using System.Collections.Concurrent;
using IBApi;
using IBKRWrapper.Events;
using IBKRWrapper.Models;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
        private readonly object openOrderLock = new();
        private readonly object orderIdLock = new();

        public Task<Trade> PlaceOrderAsync(Contract contract, Order order)
        {
            TaskCompletionSource<Trade> tcs = new();

            lock (orderIdLock)
            {
                while (!ValidOrderId) { }
                ;
                int orderId = NextOrderId;
                ValidOrderId = false;
                Trade trade = new(orderId, contract, order);

                OrderStatusEvent += trade.HandleOrderStatus;
                ExecDetailsEvent += trade.HandleExecution;
                CommissionEvent += trade.HandleCommission;

                EventHandler<OrderStatusEventArgs> statusUpdate =
                    new(
                        (sender, e) =>
                        {
                            if (e.OrderStatus.Accepted && e.OrderStatus.OrderId == orderId)
                            {
                                tcs.SetResult(trade);
                            }
                            else if (e.OrderStatus.Canceled && e.OrderStatus.OrderId == orderId)
                            {
                                tcs.SetCanceled();
                            }
                        }
                    );

                OrderStatusEvent += statusUpdate;

                tcs.Task.ContinueWith(t => OrderStatusEvent -= statusUpdate);
            }
            clientSocket.reqIds(1);
            return tcs.Task;
        }

        public void CancelOrder(Order order)
        {
            clientSocket.cancelOrder(order.OrderId, DateTimeOffset.Now.ToString());
        }

        public Task<List<OpenOrderEventArgs>> GetOpenOrdersAsync()
        {
            lock (openOrderLock)
            {
                List<OpenOrderEventArgs> orders = [];
                TaskCompletionSource<List<OpenOrderEventArgs>> tcs = new();
                EventHandler<OpenOrderEventArgs> addOrder =
                    new(
                        (sender, e) =>
                        {
                            orders.Add(e);
                        }
                    );
                EventHandler orderEnd =
                    new(
                        (sender, e) =>
                        {
                            tcs.SetResult(orders);
                        }
                    );

                OpenOrderEvent += addOrder;
                OpenOrderEndEvent += orderEnd;

                tcs.Task.ContinueWith(t =>
                {
                    OpenOrderEvent -= addOrder;
                    OpenOrderEndEvent -= orderEnd;
                });

                clientSocket.reqAllOpenOrders();

                return tcs.Task;
            }
        }

        public event EventHandler<OrderStatusEventArgs>? OrderStatusEvent;

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
            OrderStatusEvent?.Invoke(
                this,
                new OrderStatusEventArgs(
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

        public event EventHandler<OpenOrderEventArgs>? OpenOrderEvent;

        public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            OpenOrderEvent?.Invoke(
                this,
                new OpenOrderEventArgs(orderId, contract, order, orderState)
            );
        }

        public event EventHandler? OpenOrderEndEvent;

        public void openOrderEnd()
        {
            OpenOrderEndEvent?.Invoke(this, new EventArgs());
        }

        public event EventHandler<CommissionEventArgs>? CommissionEvent;

        public void commissionReport(CommissionReport commissionReport)
        {
            CommissionEvent?.Invoke(this, new CommissionEventArgs(commissionReport));
        }

        public event EventHandler<ExecDetailsEventArgs>? ExecDetailsEvent;

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            ExecDetailsEvent?.Invoke(this, new ExecDetailsEventArgs(reqId, contract, execution));
        }

        public event EventHandler<ExecDetailsEndEventArgs>? ExecDetailsEndEvent;

        public void execDetailsEnd(int reqId)
        {
            ExecDetailsEndEvent?.Invoke(this, new ExecDetailsEndEventArgs(reqId));
        }
    }
}
