using IBApi;

namespace IBKRWrapper.Events
{
    public class OpenOrderEventArgs(
        int orderId,
        Contract contract,
        Order order,
        OrderState orderState
    ) : EventArgs
    {
        public int OrderId { get; private set; } = orderId;
        public Contract Contract { get; private set; } = contract;
        public Order Order { get; private set; } = order;
        public OrderState OrderState { get; private set; } = orderState;
    }
}
