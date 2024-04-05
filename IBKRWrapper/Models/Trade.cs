using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class Trade(int orderId, Contract contract, Order order, string? status)
    {
        public int OrderId { get; private set; } = orderId;
        public int PermId { get; private set; } = order.PermId;
        public string? Status { get; private set; } = status;
        public Contract Contract { get; private set; } = contract;
        public Order Order { get; private set; } = order;
        public OrderStatus? OrderStatus { get; private set; }
        public List<Execution> Executions { get; private set; } = [];
        public List<CommissionReport> Commissions { get; private set; } = [];

        public void HandleOrderStatus(object? sender, OrderStatusEventArgs e)
        {
            if (e.OrderStatus.OrderId != OrderId && e.OrderStatus.PermId != PermId)
            {
                return;
            }
            OrderStatus = e.OrderStatus;
            Status = e.OrderStatus.Status;
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
            if (Status == null)
            {
                return false;
            }
            return OrderStatus?.Remaining == 0;
        }

        public override string ToString()
        {
            return (
                $"Trade(OrderId={OrderId}, "
                + $"PermId={PermId}, "
                + $"Status={Status}, "
                + $"Contract={ContractString(Contract)}, "
                + $"Order={OrderString(Order)}, "
                + $"OrderStatus={OrderStatus}, "
                + $"Executions=[{string.Join(", ", Executions.Select(x => ExecutionString(x)).ToArray())}], "
                + $"Commissions=[{string.Join(", ", Commissions.Select(x => CommissionString(x)).ToArray())}])"
            );
        }

        private static string ContractString(Contract contract)
        {
            return (
                $"Contract(Ticker={contract.Symbol}, "
                + $"Type={contract.SecType}, "
                + $"Exchange={contract.Exchange}, "
                + $"PrimaryExchange={contract.PrimaryExch})"
            );
        }

        private static string OrderString(Order order)
        {
            return (
                $"Order(Action={order.Action}, "
                + $"Quantity={order.TotalQuantity}, "
                + $"Type={order.OrderType}, "
                + $"LimitPrice={order.LmtPrice}, "
                + $"AuxPrice={order.AuxPrice})"
            );
        }

        private static string ExecutionString(Execution execution)
        {
            return (
                $"Execution(OrderId={execution.OrderId}, "
                + $"ExecId={execution.ExecId}, "
                + $"Time={execution.Time}, "
                + $"Side={execution.Side}, "
                + $"Shares={execution.Shares}, "
                + $"Price={execution.Price}, "
                + $"CumQty={execution.CumQty}, "
                + $"AvgPrice={execution.AvgPrice})"
            );
        }

        private static string CommissionString(CommissionReport commission)
        {
            return $"CommissionReport(ExecId={commission.ExecId}, Commission={commission.Commission})";
        }
    }
}
