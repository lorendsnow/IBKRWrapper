using IBApi;
using IBKRWrapper.Events;

namespace IBKRWrapper.Models
{
    public class ScannerData(int reqId, ScannerSubscription sub, List<TagValue> filterOptions)
    {
        public int ReqId { get; set; } = reqId;
        public ScannerSubscription Subscription { get; set; } = sub;
        public List<TagValue> FilterOptions { get; set; } = filterOptions;
        public Dictionary<int, Contract> ScannerResults { get; set; } = [];

        public void HandleScannerData(object? sender, IBKRScannerDataEventArgs e)
        {
            if (e.ReqId == ReqId)
            {
                ScannerResults.Add(e.Rank, e.Contract);
            }
        }
    }
}
