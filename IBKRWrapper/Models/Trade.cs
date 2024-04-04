using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class Trade(int orderId, Contract contract, Order order)
    {
        public int OrderId { get; set; } = orderId;
        public int PermId { get; set; } = order.PermId;
        public Contract Contract { get; set; } = contract;
        public Order Order { get; set; } = order;
        public OrderStatus? OrderStatus { get; set; }
        public List<Execution> Executions { get; set; } = [];
        public List<CommissionReport> Commissions { get; set; } = [];

        public void HandleOrderStatus(object? sender, OrderStatusEventArgs e)
        {
            if (e.OrderStatus.OrderId != OrderId && e.OrderStatus.PermId != PermId)
            {
                return;
            }
            OrderStatus = e.OrderStatus;
        }

        public void HandleExecution(object? sender, ExecDetailsEventArgs e)
        {
            if (e.Execution.OrderId != OrderId && e.Execution.PermId != PermId)
            {
                return;
            }
            Executions.Add(e.Execution);
        }

        public void HandleCommission(object? sender, CommissionEventArgs e)
        {
            if (!Executions.Select(x => x.ExecId).ToArray().Contains(e.Commission.ExecId))
            {
                return;
            }
            Commissions.Add(e.Commission);
        }

        public bool IsDone()
        {
            if (OrderStatus == null)
            {
                return false;
            }
            return OrderStatus.Remaining == 0;
        }
    }
}
