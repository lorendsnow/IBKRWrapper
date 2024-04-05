namespace IBKRWrapper.Models
{
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
        public bool Accepted
        {
            get
            {
                List<string> AcceptedStates =
                    new(["PendingSubmit", "PreSubmitted", "Submitted", "Filled"]);
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
