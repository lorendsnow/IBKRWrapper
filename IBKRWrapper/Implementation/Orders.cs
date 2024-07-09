using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper
{
    public partial class Wrapper : EWrapper
    {
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
                    orderId,
                    status,
                    filled,
                    remaining,
                    avgFillPrice,
                    permId,
                    parentId,
                    lastFillPrice,
                    clientId,
                    whyHeld,
                    mktCapPrice
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
