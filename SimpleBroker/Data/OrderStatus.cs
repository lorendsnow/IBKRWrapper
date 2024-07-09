using IBKRWrapper.Events;

namespace SimpleBroker
{
    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    public record OrderStatus
    {
        /// <summary>
        /// The order ID assigned by IBKR
        /// </summary>
        public int OrderId { get; init; }

        /// <summary>
        /// The order status
        /// </summary>
        public string Status { get; init; }

        /// <summary>
        /// Quantity filled
        /// </summary>
        public decimal Filled { get; init; }

        /// <summary>
        /// Quantity remaining to be filled
        /// </summary>
        public decimal Remaining { get; init; }

        /// <summary>
        /// Average fill price
        /// </summary>
        public double AvgFillPrice { get; init; }

        /// <summary>
        /// Perm ID assigned by IBKR
        /// </summary>
        public int PermId { get; init; }

        /// <summary>
        /// Parent ID assigned by IBKR
        /// </summary>
        public int ParentId { get; init; }

        /// <summary>
        /// Last filled price
        /// </summary>
        public double LastFillPrice { get; init; }

        /// <summary>
        /// Client ID
        /// </summary>
        public int ClientId { get; init; }

        /// <summary>
        /// Why held
        /// </summary>
        public string WhyHeld { get; init; }

        /// <summary>
        /// If an order has been capped, this indicates the current capped price. Requires TWS
        /// 967+ and API v973.04+.
        /// </summary>
        public double MarketCapPrice { get; init; }

        /// <summary>
        /// Indicates whether the order has been accepted or rejected
        /// </summary>
        public bool Accepted
        {
            get
            {
                List<string> AcceptedStates =
                    new(["PendingSubmit", "PreSubmitted", "Submitted", "Filled"]);
                return AcceptedStates.Contains(this.Status);
            }
        }

        /// <summary>
        /// Indicates whether the order has been canceled
        /// </summary>
        public bool Canceled
        {
            get
            {
                List<string> CanceledStates =
                    new(["PendingCancel", "ApiCancelled", "Cancelled", "Inactive"]);
                return CanceledStates.Contains(this.Status);
            }
        }

        internal OrderStatus(OrderStatusEventArgs args)
        {
            OrderId = args.OrderId;
            Status = args.Status;
            Filled = args.Filled;
            Remaining = args.Remaining;
            AvgFillPrice = args.AvgFillPrice;
            PermId = args.PermId;
            ParentId = args.ParentId;
            LastFillPrice = args.LastFillPrice;
            ClientId = args.ClientId;
            WhyHeld = args.WhyHeld;
            MarketCapPrice = args.MktCapPrice;
        }
    }
}
