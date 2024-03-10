namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="status"></param>
    /// <param name="filled"></param>
    /// <param name="remaining"></param>
    /// <param name="avgFillPrice"></param>
    /// <param name="permId"></param>
    /// <param name="parentId"></param>
    /// <param name="lastFillPrice"></param>
    /// <param name="whyHeld"></param>
    public class OrderStatus(
        int orderId,
        string status,
        decimal filled,
        decimal remaining,
        double avgFillPrice,
        int permId,
        int parentId,
        double lastFillPrice,
        string whyHeld
    )
    {
        public int OrderId { get; set; } = orderId;
        public string Status { get; set; } = status;
        public decimal Filled { get; set; } = filled;
        public decimal Remaining { get; set; } = remaining;
        public double AvgFillPrice { get; set; } = avgFillPrice;
        public int PermId { get; set; } = permId;
        public int ParentId { get; set; } = parentId;
        public double LastFillPrice { get; set; } = lastFillPrice;
        public string WhyHeld { get; set; } = whyHeld;

        /// <summary>
        /// Whether IBKR has accepted the order. Returns true if order status is "PreSubmitted", "Submitted", or "Filled".
        /// </summary>
        public bool Accepted
        {
            get
            {
                List<string> AcceptedStates = new(["PreSubmitted", "Submitted", "Filled"]);
                return AcceptedStates.Contains(this.Status);
            }
        }

        /// <summary>
        /// Whether an order has been canceled. Returns true if order status is "PendingCancel", "ApiCancelled", "Cancelled", or "Inactive".
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
    }
}
