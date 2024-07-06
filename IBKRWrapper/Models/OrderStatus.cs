namespace IBKRWrapper.Models
{
    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    /// <param name="OrderId"></param>
    /// <param name="Status"></param>
    /// <param name="Filled"></param>
    /// <param name="Remaining"></param>
    /// <param name="AvgFillPrice"></param>
    /// <param name="PermId"></param>
    /// <param name="ParentId"></param>
    /// <param name="LastFillPrice"></param>
    /// <param name="WhyHeld"></param>
    public record OrderStatus(
        int OrderId,
        string Status,
        decimal Filled,
        decimal Remaining,
        double AvgFillPrice,
        int PermId,
        int ParentId,
        double LastFillPrice,
        string WhyHeld
    )
    {
        /// <summary>
        /// Indivates whether the order has been accepted or rejected
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
