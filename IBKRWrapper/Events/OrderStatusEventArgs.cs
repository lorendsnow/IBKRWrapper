using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    public class OrderStatusEventArgs(OrderStatus orderStatus) : EventArgs
    {
        public OrderStatus OrderStatus { get; private set; } = orderStatus;
    }
}
