using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    /// <summary>
    /// An object representing a placed order, which receives and holds order status and fill information
    /// </summary>
    public class Trade
    {
        /// <summary>
        /// The order id sent to IBKR with the original order
        /// </summary>
        public int OrderId { get; private set; }

        /// <summary>
        /// The permanent order id assigned by IBKR
        /// </summary>
        public int PermId { get; private set; }

        /// <summary>
        /// The status of the order as reported by IBKR
        /// </summary>
        public string? Status { get; private set; }

        /// <summary>
        /// The <see cref="IBApi.Contract"/> for the underlying instrument
        /// </summary>
        public Contract Contract { get; private set; }

        /// <summary>
        /// The <see cref="IBApi.Order"/> object used to place the order with IBKR
        /// </summary>
        public Order Order { get; private set; }

        /// <summary>
        /// An object representing the status of the order as reported by IBKR
        /// </summary>
        public OrderStatus? OrderStatus { get; private set; }

        /// <summary>
        /// A list of fills received for the order
        /// </summary>
        public List<Execution> Executions { get; private set; } = [];

        /// <summary>
        /// A list of commission reports received for the order
        /// </summary>
        public List<CommissionReport> Commissions { get; private set; } = [];

        /// <summary>
        /// Internal constructor for creating a new <see cref="Trade"/> object
        /// </summary>
        /// <param name="orderId">The original request id when the order was sent IBKR</param>
        /// <param name="contract">The <see cref="IBApi.Contract"/> for the underlying instrument</param>
        /// <param name="order">The <see cref="IBApi.Order"/> object used to place the order with IBKR</param>
        /// <param name="status">The status code received from IBKR</param>
        internal Trade(int orderId, Contract contract, Order order, string? status)
        {
            OrderId = orderId;
            Contract = contract;
            Order = order;
            Status = status;
        }

        /// <summary>
        /// Event handler which receives and handles order status updates from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleOrderStatus(object? sender, OrderStatusEventArgs e)
        {
            if (e.OrderStatus.OrderId != OrderId && e.OrderStatus.PermId != PermId)
            {
                return;
            }
            OrderStatus = e.OrderStatus;
            Status = e.OrderStatus.Status;
        }

        /// <summary>
        /// Event handler which receives and handles execution updates from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleExecution(object? sender, ExecDetailsEventArgs e)
        {
            if (e.Execution.OrderId != OrderId && e.Execution.PermId != PermId)
            {
                return;
            }
            Executions.Add(e.Execution);
        }

        /// <summary>
        /// Event handler which receives and handles commission updates from IBKR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HandleCommission(object? sender, CommissionEventArgs e)
        {
            if (!Executions.Select(x => x.ExecId).ToArray().Contains(e.Commission.ExecId))
            {
                return;
            }
            Commissions.Add(e.Commission);
        }

        /// <summary>
        /// Returns true if the order has been completely filled
        /// </summary>
        /// <returns><see cref="bool"/></returns>
        public bool IsDone()
        {
            if (Status == null)
            {
                return false;
            }
            return OrderStatus?.Remaining == 0;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="Trade"/> object
        /// </summary>
        /// <returns><see cref="string"/></returns>
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

        /// <summary>
        /// Internal method for creating a new <see cref="Trade"/> object
        /// </summary>
        /// <param name="orderId">The original request id when the order was sent IBKR</param>
        /// <param name="contract">The <see cref="IBApi.Contract"/> for the underlying instrument</param>
        /// <param name="order">The <see cref="IBApi.Order"/> object used to place the order with IBKR</param>
        /// <param name="status">The status code received from IBKR</param>
        public static Trade New(int orderId, Contract contract, Order order, string? status) =>
            new(orderId, contract, order, status);
    }
}
