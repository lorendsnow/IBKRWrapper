using IBKRWrapper.Models;

namespace IBKRWrapper.Events
{
    /// <summary>
    /// Holds an <see cref="Models.OrderStatus"/> update received from IBKR.
    /// </summary>
    public class IBKROrderStatusEventArgs : EventArgs
    {
        public IBKROrderStatusEventArgs(OrderStatus orderStatus)
        {
            OrderStatus = orderStatus;
        }

        public OrderStatus OrderStatus { get; set; }
    }
}
