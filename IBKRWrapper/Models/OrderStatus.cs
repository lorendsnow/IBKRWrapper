namespace IBKRWrapper.Models
{
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

        public bool Accepted
        {
            get
            {
                List<string> AcceptedStates = new(["PreSubmitted", "Submitted", "Filled"]);
                return AcceptedStates.Contains(this.Status);
            }
        }

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
