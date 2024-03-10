using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// Trade object representing an open order. Receives and holds order status, fill and commission events from IBKR.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="contract"></param>
    /// <param name="order"></param>
    public class Trade(int orderId, Contract contract, Order order)
    {
        public int OrderId { get; set; } = orderId;
        public int PermId { get; set; } = order.PermId;
        public Contract Contract { get; set; } = contract;
        public Order Order { get; set; } = order;
        public OrderStatus? OrderStatus { get; set; }
        public Dictionary<string, Fill> Fills { get; set; } = [];

        public void HandleOrderStatus(object? sender, IBKROrderStatusEventArgs e)
        {
            if (e.OrderStatus.OrderId != OrderId && e.OrderStatus.PermId != PermId)
            {
                return;
            }
            OrderStatus = e.OrderStatus;
        }

        public void HandleFill(object? sender, IBKRFillEventArgs e)
        {
            if (e.Execution.OrderId != OrderId && e.Execution.PermId != PermId)
            {
                return;
            }
            Fills.Add(e.Execution.ExecId, new Fill(e.Contract, e.Execution));
        }

        public void HandleCommission(object? sender, IBKRCommissionEventArgs e)
        {
            if (!Fills.ContainsKey(e.CommissionReport.ExecId))
            {
                return;
            }
            Fills[e.CommissionReport.ExecId].Commission = e.CommissionReport.Commission;
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
