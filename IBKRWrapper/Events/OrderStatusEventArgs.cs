namespace IBKRWrapper.Events
{
    public class OrderStatusEventArgs(
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
    ) : EventArgs
    {
        public int OrderId { get; private set; } = orderId;
        public string Status { get; private set; } = status;
        public decimal Filled { get; private set; } = filled;
        public decimal Remaining { get; private set; } = remaining;
        public double AvgFillPrice { get; private set; } = avgFillPrice;
        public int PermId { get; private set; } = permId;
        public int ParentId { get; private set; } = parentId;
        public double LastFillPrice { get; private set; } = lastFillPrice;
        public int ClientId { get; private set; } = clientId;
        public string WhyHeld { get; private set; } = whyHeld;
        public double MktCapPrice { get; private set; } = mktCapPrice;

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
    }
}
